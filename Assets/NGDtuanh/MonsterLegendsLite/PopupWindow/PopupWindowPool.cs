using System;
using DG.Tweening;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace NGDtuanh.MonsterLegendsLite {
    [RequireComponent(typeof(RectTransform))]
    public class PopupWindowPool : SceneSingleton<PopupWindowPool> {
        [SerializeField, Required]
        private CanvasGroup blurLayer;
        
        [SerializeField, Required]
        private EnumMap<PopupWindowId, PopupWindow> prefabs;

        private readonly EnumMap<PopupWindowId, ObjectPool<PopupWindow>> pools = new();

        private uint activeWindowCnt;

        protected override void Initialize() {
            base.Initialize();

            foreach (var key in pools.Keys) {
                pools[key] = new(
                    createFunc: () => CreateFunc(key)
                  , actionOnGet: ActionOnGet
                  , actionOnRelease: ActionOnRelease);
            }
        }

        private PopupWindow CreateFunc(PopupWindowId id) {
            return Instantiate(prefabs[id], TF);
        }

        private void ActionOnGet(PopupWindow obj) {
            obj.gameObject.SetActive(true);
            obj.RectTF.SetAsLastSibling();
            if (++activeWindowCnt == 1) SetVisibleBlurLayer(true);
        }

        private void ActionOnRelease(PopupWindow obj) {
            obj.gameObject.SetActive(false);
            if (--activeWindowCnt == 0) SetVisibleBlurLayer(false);
        }

        internal PopupWindow Show(PopupWindowId id, string title, string content, Action onClose) {
            Action<PopupWindow> wrapOnClose = pools[id].Release;
            if (onClose != null) wrapOnClose += _ => onClose.Invoke();
            return pools[id].Get().Initialize(title, content, wrapOnClose);
        }

        private void SetVisibleBlurLayer(bool active) {
            blurLayer.DOKill();
            blurLayer.DOFade(active ? 1 : 0, PopupWindow.SHOW_HIDE_DURATION);
        }
    }
}