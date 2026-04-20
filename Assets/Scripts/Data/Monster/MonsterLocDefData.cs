using System;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterLocDefData {
        [Required]
        public Home_Monster PrefabHomeScene;

        [Required]
        public Battle_Monster PrefabBattleScene;
    }
}