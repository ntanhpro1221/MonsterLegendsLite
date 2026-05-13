using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace NGDtuanh.MonsterLegendsLite {
    public class UtilFuncs : NGDtuanh.Utils.UtilFuncs {
        public static readonly UtilFuncs Ins = new();
        
        protected override WaitForSeconds GetWaitForSeconds(float second) => WaitForSecondCache.Get(second);

        public string ToStrResource(long num) {
            return num.ToString("N0", CultureInfo.InvariantCulture);
        }

        public Vector2 GetPointerPos(int pointerId) {
            foreach (var touch in Touch.activeTouches)
                if (touch.touchId == pointerId)
                    return touch.screenPosition;

            return Mouse.current.position.value;
        }

        public void SetListener(UnityEvent target, UnityAction callback) {
            target.RemoveAllListeners();
            target.AddListener(callback);
        }

        public void SetListener(Button target, UnityAction callback) {
            SetListener(target.onClick, callback);
        }

        public void SetListener<T>(UnityEvent<T> target, UnityAction<T> callback) {
            target.RemoveAllListeners();
            target.AddListener(callback);
        }

        [Conditional("UNITY_EDITOR")]
        public void SaveSO(params ScriptableObject[] targets) {
            #if UNITY_EDITOR

            foreach (var target in targets) UnityEditor.EditorUtility.SetDirty(target);
            UnityEditor.AssetDatabase.SaveAssets();

            #endif
        }
    }
}