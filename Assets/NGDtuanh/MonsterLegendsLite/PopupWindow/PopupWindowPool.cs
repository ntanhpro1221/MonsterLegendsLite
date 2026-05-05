using System.Collections.Generic;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace NGDtuanh.MonsterLegendsLite {
    [RequireComponent(typeof(RectTransform))]
    public class PopupWindowPool : SceneSingleton<PopupWindowPool> {
        [SerializeField, Required]
        private EnumMap<PopupWindowId, PopupWindow> prefabs;

        private readonly Dictionary<PopupWindow, ObjectPool<PopupWindow>> pools = new();

        private ObjectPool<PopupWindow> GetPool(PopupWindow prefab) {
            if (pools.TryGetValue(prefab, out var result)) return result;
            return pools[prefab] = new(() => CreateFunc(prefab));
        }

        private PopupWindow CreateFunc(PopupWindow prefab) {
            var window = Instantiate(prefab, TF);
            window.Initialize();
            return window;
        }

        internal TPopupWindow Show<TPopupWindow>(
            PopupWindowId id
          , string title
          , string content
          , UnityAction onDoneClose) 
            where TPopupWindow : PopupWindow {
            return Show((TPopupWindow)prefabs[id], title, content, onDoneClose);
        }

        public TPopupWindow Show<TPopupWindow>(
            TPopupWindow prefab
          , string title
          , string content
          , UnityAction onDoneClose) 
            where TPopupWindow : PopupWindow {
            var pool        = GetPool(prefab);
            var wrapOnClose = new UnityAction<PopupWindow>(pool.Release);

            if (onDoneClose != null) wrapOnClose += _ => onDoneClose.Invoke();

            return (TPopupWindow)pool.Get().Show(title, content, wrapOnClose);
        }
    }
}