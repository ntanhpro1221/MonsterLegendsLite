using System;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterDefData {
        public string Name;
        public string Description;
        public MonsterStats<float> StatsBase;
        public MonsterStats<AnimationCurve> StatsGrowth;
        public MonsterRankId Rank;
        public int Cost;
    }
}