using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace NGDtuanh.MonsterLegendsLite {
    [RequireComponent(typeof(RectTransform))]
    public class FloatingTextPool : SceneSingleton<FloatingTextPool> {
        [SerializeField, Required]
        private EnumMap<FloatingTextId, FloatingText> prefabs;

        private Canvas canvas;

        private Camera cam;
        private Camera CachedCam => cam != null ? cam : cam = Camera.main;

        private readonly EnumMap<FloatingTextId, ObjectPool<FloatingText>> pools = new();

        protected override void Initialize() {
            base.Initialize();

            canvas = GetComponentInParent<Canvas>(includeInactive: true);

            foreach (var key in pools.Keys) {
                pools[key] = new(
                    createFunc: () => CreateFunc(key)
                  , actionOnGet: ActionOnGet
                  , actionOnRelease: ActionOnRelease);
            }
        }

        private FloatingText CreateFunc(FloatingTextId id) {
            return Instantiate(prefabs[id], TF);
        }

        private void ActionOnGet(FloatingText obj) {
            obj.gameObject.SetActive(true);
            obj.RectTF.SetAsLastSibling();
        }

        private void ActionOnRelease(FloatingText obj) {
            obj.gameObject.SetActive(false);
        }

        public FloatingText ShowAtWorld(FloatingTextId id, Vector2 worldPos) {
            var text = ShowAtScreen(id, CachedCam.WorldToScreenPoint(worldPos));
            text.StartCoroutine(text.IEPinToWorldPos(worldPos, canvas, CachedCam));
            return text;
        }

        public FloatingText ShowAtCenterScreen(FloatingTextId id) {
            return ShowAtScreen(id, new Vector2(Screen.width / 2f, Screen.height / 2f));
        }

        public FloatingText ShowAtScreen(FloatingTextId id, Vector2 screenPos) {
            pools[id].Get(out var text);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                RectTF, screenPos
              , canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera
              , out var locPos);
            text.RectTF.localPosition = locPos;
            text.StartFloat(pools[id].Release);
            return text;
        }
    }
}