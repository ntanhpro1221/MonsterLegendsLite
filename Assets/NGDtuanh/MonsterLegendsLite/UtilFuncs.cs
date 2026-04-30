using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace NGDtuanh.MonsterLegendsLite {
    public class UtilFuncs : NGDtuanh.Utils.UtilFuncs {
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
    }
}