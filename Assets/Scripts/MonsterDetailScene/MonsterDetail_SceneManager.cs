using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class MonsterDetail_SceneManager : Singleton<MonsterDetail_SceneManager> {
        [SerializeField, Required]
        private MonsterDetail_UI_Info uiInfo;
        
        [SerializeField, Required]
        private MonsterDetail_UI_Monster uiMonster;

        [SerializeField, Required]
        private Transform monsterSlot;
        
        [ShowInInspector, ReadOnly, PropertyOrder(-1)]
        private MonsterDetail_Monster monster;

        private void Start() {
            LoadBootDataThenDelete();
            
            UpdateUI_Info();
            UpdateUI_Monster();
        }

        private void LoadBootDataThenDelete() {
            var bootData   = MonsterDetail_BootData.Ins;
            var gameLocDef = DataManager.Ins.GameLocDefData;

            var ins = Instantiate(gameLocDef.Monster[bootData.monster.Id].PrefabMonsterDetailScene, monsterSlot);
            ins.Initialize(bootData.monster);

            monster = ins;
            
            Destroy(bootData);
        }

        private void UpdateUI_Info() {
            uiInfo.SetStats(monster.CalculateStats());
            uiInfo.SetDescription(monster.defData.Description);
        }

        private void UpdateUI_Monster() {
            uiMonster.SetRankIcon(DataManager.Ins.GameLocDefData.MonsterRank[monster.defData.Rank].Icon);
            uiMonster.SetCustomName(monster.insData.CustomName);
            uiMonster.SetName(monster.defData.Name);
            uiMonster.SetLevel(monster.insData.Level, DataManager.Ins.GameDefData.MonsterRank[monster.defData.Rank].MaxLevel);
            uiMonster.SetExp(100, monster.CalculateStats()[MonsterStatId.FoodCost]);
            uiMonster.SetFoodRequired(monster.CalculateStats()[MonsterStatId.FoodCost]);
        }
    }
}