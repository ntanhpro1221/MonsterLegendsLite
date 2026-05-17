using System;
using System.Collections.Generic;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class GameDefData {
        public UserDefData User;
        public EnumMap<ElementId, HabitatDefData> Habitats;
        public EnumMap<FarmId, FarmDefData> Farms;
        public EnumMap<BreedingPlaceId, BreedingPlaceDefData> BreedingPlaces;
        public EnumMap<MonsterId, MonsterDefData> Monsters;
        public EnumMap<ElementId, ElementData> Elements;
        public EnumMap<MonsterRankId, MonsterRankData> MonsterRanks;
        public EnumMap<MonsterSkillTargetId, MonsterSkillTargetData> MonsterSkillTargets;
        public List<AdventureLevelData> AdventureLevels;

        [FoldoutGroup("Constants")]
        public float SellRatio_Monster;
        
        [FoldoutGroup("Constants")]
        public float SellRatio_Building;
        
        [FoldoutGroup("Constants")]
        public float Home_MonsterSpeed;

        [FoldoutGroup("Constants")]
        public Vector2 Home_MonsterIdleTime;

        [FoldoutGroup("Constants"), Range(0, 1)]
        public float Home_ShowCollectResourceBtnThreshold;
        
        [FoldoutGroup("Constants"), MinValue(0)]
        public long DefaultBreedingDuration;
    }
}