using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterLocDefData {
        [Required]
        public Sprite Avatar; 
        
        [Required]
        public Home_Monster PrefabHomeScene;

        [Required]
        public Battle_Monster PrefabBattleScene;

        [Required]
        public MonsterDetail_Monster PrefabMonsterDetailScene;
    }
}