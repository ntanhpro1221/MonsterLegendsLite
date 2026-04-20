using System;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterDefData {
        public string Name;
        public string Description;
        public MonsterStats<int> StatsBase;
        public MonsterStats<AnimationCurve> StatsGrowth;
        public MonsterRankId Rank;
        public int Cost;

        public MonsterStats<int> CalculateStats(MonsterInsData insData) {
            MonsterStats<int> result = new();
            foreach (var key in result.Keys) {
                result[key] = StatsBase[key] + Mathf.FloorToInt(StatsGrowth[key].Evaluate(insData.Level));
            }

            return result;
        }
    }
}