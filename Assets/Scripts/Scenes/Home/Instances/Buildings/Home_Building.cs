using System;
using System.Collections;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilFuncs = NGDtuanh.Utils.UtilFuncs;

namespace MonsterLegendsLite {
    public abstract class Home_Building<TBuildingInsData> : Home_Building where TBuildingInsData : BuildingInsData {
        public TBuildingInsData InsData => (TBuildingInsData)InsDataWeak;

        public sealed override void Initialize(BuildingInsData insData, bool isBuySample) {
            base.Initialize(insData, isBuySample);
            Initialize(InsData, isBuySample);
        }

        protected virtual void Initialize(TBuildingInsData insData, bool isBuySample) { }
    }
    
    public abstract class Home_Building
        : MonoBehaviourExt
        , ISelectableTarget
        , IPointerClickHandler
        , IPointerDownHandler
        , IPointerUpHandler
        , IPointerMoveHandler {
        [ShowInInspector, ReadOnly, PropertyOrder(-100)]
        public BuildingInsData InsDataWeak { get; private set; }

        [field: SerializeField, Required]
        protected Home_BuildingSharedData SharedData { get; private set; }

        private bool isBuySample;
        private bool isSelected, isPntDown, isPntMoved;
        private int pntId;
        private Vector2 pntDownOffset;
        private int idleSortingOrder;

        private bool isActiveCollectBtn;

        public event Action<bool> onPlaceableChanged;

        public T To<T>() where T : Home_Building {
            return (T)this;
        }

        public virtual void Initialize(BuildingInsData insData, bool isBuySample) {
            InsDataWeak      = insData;
            this.isBuySample = isBuySample;

            var size = DataManager.Ins.GetBuildingDefData(insData).Size;
            SharedData.selectOutline.size   = size;
            SharedData.validPlaceSpr.size   = size;
            SharedData.invalidPlaceSpr.size = size;
            foreach (var arrowAnchor in SharedData.arrowAnchors) arrowAnchor.UpdatePosFromAnchor();

            SharedData.selectWrapper.gameObject.SetActive(false);

            SetVisibleValidPlace(false);
            
            CheckAndUpdateCollectBtnActive();

            if (isBuySample) {
                OnSelect();
                isPntMoved = true;
                Home_SceneManager.Ins.OnMove_Building(this);
                StartCoroutine(IEMoveWithPointer());
            } else {
                SharedData.collectBtn.onPointerClickRaw += OnPointerClick;
                
                EventDispatcher.RegisterEvent(EventId.UserBuildingListChanged, DestroyIfNotExistInDatabase, this);
            }
        }

        protected virtual void OnDestroy() {
            SharedData.collectBtn.onPointerClickRaw -= OnPointerClick;
            
            EventDispatcher.UnregisterEvent(EventId.UserBuildingListChanged, DestroyIfNotExistInDatabase, this);
        }

        protected virtual void Update() {
            CheckAndUpdateCollectBtnActive();
        }

        private void DestroyIfNotExistInDatabase() {
            if (DataManager.Ins.IsHaveBuilding(InsDataWeak)) return;
            Destroy(gameObject);
        }

        public void SetIdleSortingOrder(int order) {
            SharedData.sortingGroup.sortingOrder = idleSortingOrder = order;
        }

        public void OnSelect() {
            isSelected = true;
            SharedData.selectWrapper.gameObject.SetActive(true);

            SharedData.sortingGroup.sortingOrder = short.MaxValue;

            TF.position = utils.With(TF.position, UtilFuncs.VecAxis.Z, -1);
        }

        public void OnDeselect() {
            isSelected = false;
            SharedData.selectWrapper.gameObject.SetActive(false);

            SharedData.sortingGroup.sortingOrder = idleSortingOrder;

            TF.position = utils.With(TF.position, UtilFuncs.VecAxis.Z, 0);
        }

        private void CheckAndUpdateCollectBtnActive() {
            isActiveCollectBtn = IsShouldCollectBtnActive(out var sprite);
            SharedData.collectBtn.SetActive(isActiveCollectBtn);
            if (sprite != null) SharedData.collectBtn.SetIcon(sprite);
        }

        public void OnPointerClick(PointerEventData eventData) {
            var deltaPos = Vector2.Distance(eventData.pressPosition, eventData.position);
            if (deltaPos > EventSystem.current.pixelDragThreshold) return;
            
            CheckAndUpdateCollectBtnActive();
            Home_SceneManager.Ins.OnClicked_Building(this, isActiveCollectBtn, out var isClickCollectBtn);

            if (isClickCollectBtn) {
                DoClickCollectBtn();
                CheckAndUpdateCollectBtnActive();
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (!isSelected) return;

            isPntDown     = true;
            isPntMoved    = false;
            pntId         = eventData.pointerId;
            pntDownOffset = Home_SceneManager.Ins.Cam.ScreenToWorldPoint(eventData.position) - TF.position;

            StartCoroutine(IEMoveWithPointer());
            
            Home_CameraController.SetControl(false);
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (pntId != eventData.pointerId) return;

            isPntDown = false;
            
            Home_CameraController.SetControl(true);
        }

        public void OnPointerMove(PointerEventData eventData) {
            if (isPntMoved) return;
            if (!isPntDown) return;
            if (pntId != eventData.pointerId) return;

            isPntMoved = true;
            Home_SceneManager.Ins.OnMove_Building(this);
        }

        private IEnumerator IEMoveWithPointer() {
            UpdateFromExpectedPos(TF.position);

            while (isPntDown && isSelected) {
                if (isPntMoved) {
                    var expectedPos = (Vector2)Home_SceneManager.Ins.Cam.ScreenToWorldPoint(utils.GetPointerPos(pntId)) - pntDownOffset;
                    UpdateFromExpectedPos(expectedPos);
                }

                yield return null;
            }

            void UpdateFromExpectedPos(Vector2 expectedPos) {
                var nearestTilePos = Home_MapManager.Ins.GetNearestTilePos(expectedPos);
                var oldZPos        = TF.position.z;
                TF.position = utils.With(Home_MapManager.Ins.GetWorldPos(nearestTilePos), UtilFuncs.VecAxis.Z, oldZPos);

                var isPlaceable = Home_MapManager.Ins.IsPlaceable(this);
                SharedData.validPlaceSpr.enabled   = isPlaceable;
                SharedData.invalidPlaceSpr.enabled = !isPlaceable;

                onPlaceableChanged?.Invoke(isPlaceable);
            }
        }

        public void SetVisibleValidPlace(bool isOn) {
            SharedData.validPlaceSpr.gameObject.SetActive(isOn);
            SharedData.invalidPlaceSpr.gameObject.SetActive(isOn);
        }

        public void OnMoveDiscarded() {
            if (isBuySample) {
                gameObject.SetActive(false); // Something bad is going to happen if destroy this :))

                Home_SceneManager.Ins.TryHideMoveBuildingInfo();
            } else {
                TF.position = Home_MapManager.Ins.GetWorldPos(InsDataWeak.Position);
                
                Home_SceneManager.Ins.ForceShowBuildingInfo(this);
            }
        }

        public void OnMoveConfirmed() {
            if (isBuySample) {
                gameObject.SetActive(false); // Something bad is going to happen if destroy this :))
                
                UpdateData_BuyBuilding(Home_MapManager.Ins.GetNearestTilePos(TF.position), out int cost, out string insId);
                
                FloatingTextPool.Ins.ShowAtCenterScreen(FloatingTextId.GoldChange).SetTextChange(-cost);
                
                EventDispatcher.PostEvent(EventId.UserBuildingListChanged);
                EventDispatcher.PostEvent(EventId.UserGoldChanged);
                
                Home_SceneManager.Ins.ForceShowBuildingInfo(GetBuildingFromInsId(insId));
            } else {
                DataManager.Ins.UpdateData_MoveBuilding(InsDataWeak, Home_MapManager.Ins.GetNearestTilePos(TF.position));
                
                EventDispatcher.PostEvent(EventId.HomeMapChanged);
                
                Home_SceneManager.Ins.ForceShowBuildingInfo(this);
            }
        }

        protected abstract void UpdateData_BuyBuilding(Vector2Int pos, out int cost, out string insId);
        protected abstract Home_Building GetBuildingFromInsId(string insId);
        protected abstract bool IsShouldCollectBtnActive(out Sprite sprite);
        protected abstract void DoClickCollectBtn();
    }
}