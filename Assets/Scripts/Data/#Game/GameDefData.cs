using System;
using System.Collections.Generic;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class GameDefData {
        public UserDefData User;
        public EnumMap<ElementId, ElementData> Elements;
        public EnumMap<MonsterRankId, MonsterRankData> MonsterRanks;
        public EnumMap<MonsterSkillTargetId, MonsterSkillTargetData> MonsterSkillTargets;
        public EnumMap<MonsterId, MonsterDefData> Monsters;
        public EnumMap<ElementId, HabitatDefData> Habitats;
        public EnumMap<FarmId, FarmDefData> Farms;
        public List<AdventureLevelData> AdventureLevels;

        [FoldoutGroup("Constants")]
        public float SellRatio_Monster;
        
        [FoldoutGroup("Constants")]
        public float SellRatio_Building;
        
        [FoldoutGroup("Constants")]
        public float Home_MonsterSpeed;

        [FoldoutGroup("Constants")]
        public Vector2 Home_MonsterIdleTime;
    }
}