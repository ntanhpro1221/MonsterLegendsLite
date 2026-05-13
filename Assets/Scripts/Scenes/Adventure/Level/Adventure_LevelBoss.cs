using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Adventure_LevelBoss : Adventure_Level {
        [SerializeField, Required]
        private Transform monsterLayer;

        public override void SetAllData(AdventureLevelData levelData, int levelIndex) {
            base.SetAllData(levelData, levelIndex);

            Instantiate(DataManager.Ins.GameLocDefData.Monsters[levelData.Monsters[0].Id].PrefabMonsterAdventureScene, monsterLayer);
        }
    }
}