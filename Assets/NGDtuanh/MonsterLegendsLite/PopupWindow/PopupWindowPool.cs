using System;
using System.Collections.Generic;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace NGDtuanh.MonsterLegendsLite {
    [RequireComponent(typeof(RectTransform))]
    public class PopupWindowPool : SceneSingleton<PopupWindowPool> {
        [SerializeField, Required]
        private EnumMap<PopupWindowId, PopupWindow> prefabs;

        private readonly Dictionary<PopupWindow, ObjectPool<PopupWindow>> pools = new();

        private ObjectPool<PopupWindow> GetPool(PopupWindow prefab) {
            if (pools.TryGetValue(prefab, out var result)) return result;
            return pools[prefab] = new(
                createFunc: () => CreateFunc(prefab)
              , actionOnGet: ActionOnGet
              , actionOnRelease: ActionOnRelease);
        }

        private PopupWindow CreateFunc(PopupWindow prefab) {
            return Instantiate(prefab, TF);
        }

        private void ActionOnGet(PopupWindow obj) {
            obj.gameObject.SetActive(true);
            obj.RectTF.SetAsLastSibling();
        }

        private void ActionOnRelease(PopupWindow obj) {
            obj.gameObject.SetActive(false);
        }

        internal TPopupWindow Show<TPopupWindow>(PopupWindowId id, string title, string content, Action onClose) where TPopupWindow : PopupWindow {
            return Show((TPopupWindow)prefabs[id], title, content, onClose);
        }

        public TPopupWindow Show<TPopupWindow>(TPopupWindow prefab, string title, string content, Action onClose) where TPopupWindow : PopupWindow {
            var pool        = GetPool(prefab);
            var wrapOnClose = new Action<PopupWindow>(pool.Release);

            if (onClose != null) wrapOnClose += _ => onClose.Invoke();

            return (TPopupWindow)pool.Get().Initialize(title, content, wrapOnClose);
        }
    }
}