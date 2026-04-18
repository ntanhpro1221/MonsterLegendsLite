using System.Collections.Generic;
using UnityEngine;

namespace NGDtuanh.MonsterLegends {
    public static class WaitForSecondCache {
        private static readonly Dictionary<float, WaitForSeconds> cache = new();

        public static WaitForSeconds Get(float time) {
            if (!cache.ContainsKey(time)) cache[time] = new WaitForSeconds(time);
            return cache[time];
        }
    }
}