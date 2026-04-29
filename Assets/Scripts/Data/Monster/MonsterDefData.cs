using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterDefData {
        public string Name;
        public string Description;
        public MonsterStats<int> StatsBase;
        public MonsterStats<AnimationCurve> StatsGrowth;
        public List<MonsterSkillData> Skills;
        public List<ElementId> Element;
        public MonsterRankId Rank;
        public int Cost;

        public MonsterStats<int> CalculateStats(MonsterInsData insData) {
            MonsterStats<int> result = new();
            foreach (var key in result.Keys) result[key] = CalculateStat(insData, key);

            return result;
        }
        
        public int CalculateStat(MonsterInsData insData, MonsterStatId statId) {
            return StatsBase[statId] + Mathf.FloorToInt(StatsGrowth[statId].Evaluate(insData.Level));
        }
    }
}