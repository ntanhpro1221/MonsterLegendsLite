using System.Collections.Generic;
using NGDtuanh.Types;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace MonsterLegendsLite {
    /// <summary>
    /// Source: Dark magic known to everyone
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class Home_CameraController : SceneSingleton<Home_CameraController> {
        [Header("Control Toggle")]
        [Tooltip("Bật/tắt khả năng điều khiển camera từ bên ngoài")]
        [SerializeField]
        private bool isControlEnabled = true;

        [Header("Map Bounds")]
        [Tooltip("Khu vực chữ nhật giới hạn map")]
        [SerializeField]
        private Rect mapBounds = new Rect(-10, -10, 20, 20);

        [Header("Zoom Settings")]
        [SerializeField]
        private float minZoom = 2f;

        [Tooltip("Độ zoom tối đa mong muốn (Sẽ được tự động cắt giảm nếu map quá nhỏ)")]
        [SerializeField]
        private float maxZoom = 10f;

        [SerializeField]
        private float mouseZoomSensitivity = 1f;

        [Header("Pan & Inertia Settings")]
        [SerializeField]
        private float inertiaFriction = 5f;

        [SerializeField]
        private float maxVelocity = 50f;

        private Camera cam;
        private Vector3 velocity;
        private bool isDragging;
        private Vector2 lastMousePos;
        private bool isBlockedByUI;

        private PointerEventData cachedPointerEventData;
        private readonly List<RaycastResult> cachedRaycastResults = new();

        protected override void Initialize() {
            base.Initialize();

            cam = GetComponent<Camera>();

            // Chặn luôn ngay từ đầu lúc khởi chạy game để tránh trường hợp size mặc định của cam quá lớn
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, GetRealMaxZoom());
            ClampCamera();
        }

        private void OnEnable() {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable() {
            EnhancedTouchSupport.Disable();
        }

        public static void SetControl(bool isEnabled) {
            Ins.isControlEnabled = isEnabled;
            if (!isEnabled) {
                Ins.isDragging    = false;
                Ins.isBlockedByUI = false;
            }
        }

        private void LateUpdate() {
            if (!isControlEnabled) {
                ApplyInertia();
                ClampCamera();
                return;
            }

            bool isInteracting = false;

            if (Touch.activeTouches.Count == 0 && (Mouse.current == null || !Mouse.current.leftButton.isPressed)) {
                isBlockedByUI = false;
            }

            if (Touch.activeTouches.Count > 0) {
                foreach (var touch in Touch.activeTouches) {
                    if (touch.phase == TouchPhase.Began && IsPointerOverUI(touch.screenPosition)) {
                        isBlockedByUI = true;
                    }
                }

                if (!isBlockedByUI) {
                    if (Touch.activeTouches.Count == 2) {
                        HandlePinchAndDrag();
                        isInteracting = true;
                    } else if (Touch.activeTouches.Count == 1) {
                        HandleSingleTouch(Touch.activeTouches[0]);
                        isInteracting = true;
                    }
                }
            } else if (Mouse.current != null) {
                Vector2 mousePos = Mouse.current.position.ReadValue();

                if (Mouse.current.leftButton.wasPressedThisFrame) {
                    isBlockedByUI = IsPointerOverUI(mousePos);
                }

                if (!isBlockedByUI) {
                    float scroll = Mouse.current.scroll.y.ReadValue();
                    if (Mathf.Abs(scroll) > 0.01f && !IsPointerOverUI(mousePos)) {
                        HandleMouseZoom(scroll);
                    }

                    if (Mouse.current.leftButton.isPressed) {
                        HandleMouseDrag();
                        isInteracting = true;
                    }
                }
            }

            if (!isInteracting) {
                isDragging = false;
                ApplyInertia();
            }

            ClampCamera();
        }

        /// <summary>
        /// Tính toán giới hạn Zoom tối đa dựa trên tỉ lệ màn hình thực tế và kích thước Bounds.
        /// </summary>
        private float GetRealMaxZoom() {
            // Giới hạn zoom theo chiều dọc
            float maxHeight = mapBounds.height / 2f;

            // Giới hạn zoom theo chiều ngang (phụ thuộc vào tỉ lệ màn hình)
            float maxWidth = (mapBounds.width / 2f) / cam.aspect;

            // Lấy con số nhỏ nhất giữa MaxZoom cứng cài trên Inspector và giới hạn dọc/ngang
            return Mathf.Max(minZoom, Mathf.Min(maxZoom, maxHeight, maxWidth));
        }

        private bool IsPointerOverUI(Vector2 screenPosition) {
            if (cachedPointerEventData == null)
                cachedPointerEventData = new PointerEventData(EventSystem.current);

            cachedPointerEventData.position = screenPosition;
            cachedRaycastResults.Clear();

            EventSystem.current.RaycastAll(cachedPointerEventData, cachedRaycastResults);

            for (int i = 0; i < cachedRaycastResults.Count; i++) {
                if (cachedRaycastResults[i].module is GraphicRaycaster)
                    return true;
            }

            return false;
        }

        private void HandleSingleTouch(Touch touch) {
            if (touch.phase == TouchPhase.Began) {
                isDragging = true;
                velocity   = Vector3.zero;
            } else if (touch.phase == TouchPhase.Moved) {
                Vector2 prevScreenPos = touch.screenPosition - touch.delta;
                Vector3 worldDelta    = cam.ScreenToWorldPoint(prevScreenPos) - cam.ScreenToWorldPoint(touch.screenPosition);

                transform.position += worldDelta;
                velocity           =  Vector3.ClampMagnitude(worldDelta / Time.deltaTime, maxVelocity);
            }
        }

        private void HandlePinchAndDrag() {
            Touch t0 = Touch.activeTouches[0];
            Touch t1 = Touch.activeTouches[1];

            Vector2 prevPos0 = t0.screenPosition - t0.delta;
            Vector2 prevPos1 = t1.screenPosition - t1.delta;

            float a = Vector2.Distance(prevPos0, prevPos1);
            float b = Vector2.Distance(t0.screenPosition, t1.screenPosition);

            Vector2 prevMid = (prevPos0 + prevPos1) / 2f;
            Vector2 currMid = (t0.screenPosition + t1.screenPosition) / 2f;

            Vector3 worldPrevMid = cam.ScreenToWorldPoint(prevMid);
            Vector3 worldCurrMid = cam.ScreenToWorldPoint(currMid);
            transform.position += (worldPrevMid - worldCurrMid);

            if (a > 0 && b > 0 && Mathf.Abs(a - b) > 1f) {
                Vector3 pointToZoom = cam.ScreenToWorldPoint(currMid);

                float newSize = cam.orthographicSize * (a / b);

                // DÙNG REAL MAX ZOOM TẠI ĐÂY
                cam.orthographicSize = Mathf.Clamp(newSize, minZoom, GetRealMaxZoom());

                Vector3 pointAfterZoom = cam.ScreenToWorldPoint(currMid);
                transform.position += (pointToZoom - pointAfterZoom);
            }

            velocity   = Vector3.zero;
            isDragging = false;
        }

        private void HandleMouseDrag() {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();

            if (!isDragging) {
                lastMousePos = currentMousePos;
                isDragging   = true;
                velocity     = Vector3.zero;
            } else {
                Vector3 worldDelta = cam.ScreenToWorldPoint(lastMousePos) - cam.ScreenToWorldPoint(currentMousePos);
                transform.position += worldDelta;

                if (Time.deltaTime > 0)
                    velocity = Vector3.ClampMagnitude(worldDelta / Time.deltaTime, maxVelocity);

                lastMousePos = currentMousePos;
            }
        }

        private void HandleMouseZoom(float scrollValue) {
            Vector2 mousePos    = Mouse.current.position.ReadValue();
            Vector3 pointToZoom = cam.ScreenToWorldPoint(mousePos);

            float zoomAmount = Mathf.Sign(scrollValue) * mouseZoomSensitivity;

            // DÙNG REAL MAX ZOOM TẠI ĐÂY
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomAmount, minZoom, GetRealMaxZoom());

            Vector3 pointAfterZoom = cam.ScreenToWorldPoint(mousePos);
            transform.position += (pointToZoom - pointAfterZoom);
        }

        private void ApplyInertia() {
            if (velocity.sqrMagnitude > 0.001f) {
                transform.position += velocity * Time.deltaTime;
                velocity           =  Vector3.Lerp(velocity, Vector3.zero, inertiaFriction * Time.deltaTime);
            } else {
                velocity = Vector3.zero;
            }
        }

        private void ClampCamera() {
            float camHeight = cam.orthographicSize;
            float camWidth  = camHeight * cam.aspect;

            float minX = mapBounds.xMin + camWidth;
            float maxX = mapBounds.xMax - camWidth;
            float minY = mapBounds.yMin + camHeight;
            float maxY = mapBounds.yMax - camHeight;

            // Nếu max < min, điều đó có nghĩa là camera lớn hơn khu vực được phép ở trục đó
            // Nó sẽ tự động ép về tọa độ trung tâm
            if (maxX < minX) {
                minX = mapBounds.center.x;
                maxX = mapBounds.center.x;
            }

            if (maxY < minY) {
                minY = mapBounds.center.y;
                maxY = mapBounds.center.y;
            }

            Vector3 pos = transform.position;
            pos.x              = Mathf.Clamp(pos.x, minX, maxX);
            pos.y              = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }

        private void OnDrawGizmosSelected() {
            // 1. Vẽ khung viền (WireCube)
            Gizmos.color = Color.red;
            Vector3 center = new Vector3(mapBounds.center.x, mapBounds.center.y, 0);
            Vector3 size   = new Vector3(mapBounds.size.x, mapBounds.size.y, 0.1f);
            Gizmos.DrawWireCube(center, size);

            // 2. Vẽ 4 nút ở 4 góc
            Gizmos.color = new Color(1f, 0.5f, 0f); // Màu cam cho các nút dễ nhìn hơn
            float nodeRadius = 0.5f;                // Kích thước của nút (bạn có thể tự chỉnh to/nhỏ)

            // Vẽ nút (dạng hình cầu)
            Gizmos.DrawSphere(new Vector3(mapBounds.xMin, mapBounds.yMin), nodeRadius);
            Gizmos.DrawSphere(new Vector3(mapBounds.xMin, mapBounds.yMax), nodeRadius);
            Gizmos.DrawSphere(new Vector3(mapBounds.xMax, mapBounds.yMax), nodeRadius);
            Gizmos.DrawSphere(new Vector3(mapBounds.xMax, mapBounds.yMin), nodeRadius);
        }
    }
}