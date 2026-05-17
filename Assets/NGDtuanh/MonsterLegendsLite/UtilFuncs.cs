using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace NGDtuanh.MonsterLegendsLite {
    public class UtilFuncs : NGDtuanh.Utils.UtilFuncs {
        public static readonly UtilFuncs Ins = new();

        private static readonly (long inSeconds, string symbol)[] timeUnits = {
            (60 * 60 * 24 * 365, "y")
          , (60 * 60 * 24 * 30, "mo")
          , (60 * 60 * 24 * 7, "w")
          , (60 * 60 * 24, "d")
          , (60 * 60, "h")
          , (60, "m")
          , (1, "s")
        };
        
        protected override WaitForSeconds GetWaitForSeconds(float second) => WaitForSecondCache.Get(second);

        public string ToStr_Resource(long num) {
            return num.ToString("N0", CultureInfo.InvariantCulture);
        }

        public string ToStr_TimeAmount(long seconds) {
            for (int i = 0; i < timeUnits.Length; ++i) {
                var (inSeconds, symbol) = timeUnits[i];

                if (seconds < inSeconds) continue;

                var mainValue = seconds / inSeconds;
                var result    = mainValue + symbol;

                if (i + 1 == timeUnits.Length) return result;

                var remainder = seconds % inSeconds;
                if (remainder == 0) return result;

                var nextUnit = timeUnits[i + 1];

                return result + Math.Ceiling((double)remainder / nextUnit.inSeconds) + nextUnit.symbol;
            }

            return "0s";
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

        public void LogExceptionWithWindow(Exception e) {
            Debug.LogException(e);
            NotificationWindow.Show(
                title: "SOME THING WENT WRONG"
              , content: e.Message);
        }

        public int LerpSortingOrder(Vector2 range, float value, uint padding = 0) {
            return (int)Mathf.Lerp(
                short.MaxValue - padding
              , short.MinValue + padding
              , Mathf.InverseLerp(range.x, range.y, value));
        }
    }
}