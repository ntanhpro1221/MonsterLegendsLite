using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NGDtuanh.Types {
    /// <summary>
    /// Serializable timestamp in unix format.
    /// </summary>
    [Serializable]
    public struct SerTimestamp {
        [SerializeField]
        [JsonProperty]
        internal long value;

        [OnInspectorInit]
        private void OnInit() {
            if (value == 0) value = Now().value;
        }

        public static SerTimestamp Now() {
            return new() { value = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
        }

        public static long DeltaSeconds(SerTimestamp left, SerTimestamp right) {
            return Math.Abs(left.value - right.value);
        }
        
        public static float DeltaMinutes(SerTimestamp left, SerTimestamp right) {
            return DeltaSeconds(left, right) / 60f;
        }
    }
}