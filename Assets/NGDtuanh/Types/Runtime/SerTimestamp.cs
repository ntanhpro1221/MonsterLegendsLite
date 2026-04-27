using System;

namespace NGDtuanh.Types {
    /// <summary>
    /// Serializable timestamp in unix format.
    /// </summary>
    [Serializable]
    public struct SerTimestamp {
        public long value;

        public static SerTimestamp GetCurTimestamp() {
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