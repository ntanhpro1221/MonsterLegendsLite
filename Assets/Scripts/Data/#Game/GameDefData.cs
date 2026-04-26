using System;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class GameDefData {
        public UserDefData User;
        public EnumMap<MonsterRankId, MonsterRankData> MonsterRank;
        public EnumMap<MonsterId, MonsterDefData> Monster;
        public EnumMap<HabitatId, HabitatDefData> Habitat;
        public EnumMap<FarmId, FarmDefData> Farm;

        [FoldoutGroup("Home Scene")]
        public float Home_MonsterSpeed;

        [FoldoutGroup("Home Scene")]
        public Vector2 Home_MonsterIdleTime;
    }
}