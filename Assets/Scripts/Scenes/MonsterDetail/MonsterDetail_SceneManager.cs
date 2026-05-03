using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class MonsterDetail_SceneManager : SceneSingleton<MonsterDetail_SceneManager> {
        [SerializeField, Required]
        private NewSkillAvailableWindow newSkillAvailableWindowPrefab;
        
        [SerializeField, Required]
        private Canvas canvas;
        
        [SerializeField, Required]
        private MonsterDetail_UI_Info uiInfo;

        [SerializeField, Required]
        private MonsterDetail_UI_Skill uiSkill;
        
        [SerializeField, Required]
        private MonsterDetail_UI_Monster uiMonster;

        [SerializeField, Required]
        private Transform monsterSlot;
        
        [ShowInInspector, ReadOnly, PropertyOrder(-1)]
        private MonsterDetail_Monster monster;

        protected override void Initialize() {
            base.Initialize();
            
            LoadBootDataThenDelete();

            UpdateUI_Info();
            UpdateUI_Skill();
            UpdateUI_Monster();

            EventDispatcher.RegisterEvent(EventId.MonsterLevelChangedInMonsterDetail, TryNotifyNewWindowAvailable, this);
            EventDispatcher.RegisterEvent(EventId.MonsterLevelChangedInMonsterDetail, UpdateUI_Info, this);
            EventDispatcher.RegisterEvent(EventId.MonsterFeedInMonsterDetail, UpdateUI_Monster, this);
            EventDispatcher.RegisterEvent(EventId.MonsterSkillListChanged, UpdateUI_Skill, this);
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(EventId.MonsterLevelChangedInMonsterDetail, TryNotifyNewWindowAvailable, this);
            EventDispatcher.UnregisterEvent(EventId.MonsterLevelChangedInMonsterDetail, UpdateUI_Info, this);
            EventDispatcher.UnregisterEvent(EventId.MonsterFeedInMonsterDetail, UpdateUI_Monster, this);
            EventDispatcher.UnregisterEvent(EventId.MonsterSkillListChanged, UpdateUI_Skill, this);
        }

        private void LoadBootDataThenDelete() {
            var bootData   = MonsterDetail_BootData.Ins;
            var gameLocDef = DataManager.Ins.GameLocDefData;

            var ins = Instantiate(gameLocDef.Monster[bootData.Monster.Id].PrefabMonsterDetailScene, monsterSlot);
            ins.Initialize(bootData.Monster);

            monster = ins;
            
            Destroy(bootData.gameObject);
        }

        private void TryNotifyNewWindowAvailable() {
            foreach (var skill in monster.defData.Skills) {
                if (skill.UnlockAtLevel != monster.insData.Level) continue;
                NewSkillAvailableWindow.Show(
                    newSkillAvailableWindowPrefab
                  , skill
                  , DataManager.Ins.GameLocDefData.Element[skill.Element].ElementButton);
            }
        }

        private void UpdateUI_Info() {
            uiInfo.SetStats(monster.CalculateStats());
            uiInfo.SetElements(monster.defData.Elements.Select(static i => DataManager.Ins.GameLocDefData.Element[i].ElementButton).ToArray());
            uiInfo.SetRevenue(monster.CalculateStat(MonsterStatId.GoldPerMin));
            uiInfo.SetDescription(monster.defData.Description);
            uiInfo.SetMoveBtnCallback(static () => {
                if (!DataManager.Ins.IsAnyHabitatCanAcceptNewMonster(Ins.monster.insData)) {
                    NotificationWindow.Show(
                        title: "NO VALID HABITAT"
                      , content: $"You dont have any habitat that can accept {Ins.monster.defData.GetCustomNameIfPossible(Ins.monster.insData)}");
                    return;
                }

                Home_BootData.InsAutoSpawn.SetData_MoveMonster(Ins.monster.insData);
                Ins.BackToHomeScene();
            });
            uiInfo.SetSellBtnCallback(static () => {
                var sellValue = (int)(Ins.monster.defData.Cost * DataManager.Ins.GameDefData.SellRatio_Monster);

                YesNoWindow.Show(
                    title: "SELL MONSTER"
                  , content: $"Are you sure you want to sell {Ins.monster.defData.GetCustomNameIfPossible(Ins.monster.insData)} for {sellValue} gold?"
                  , yesCallback: () => {
                        DataManager.Ins.UpdateData_SellMonster(Ins.monster.insData);

                        EventDispatcher.PostEvent(EventId.UserGoldChanged);
                        EventDispatcher.PostEvent(EventId.UserMonsterListChanged);
                        
                        Home_BootData.InsAutoSpawn.SetData_FloatingGold(sellValue);
                        Ins.BackToHomeScene();
                    });
            });
        }

        private void UpdateUI_Skill() {
            uiSkill.SetData(monster.insData);
        }
        
        private void UpdateUI_Monster() {
            uiMonster.SetRankIcon(DataManager.Ins.GameLocDefData.MonsterRank[monster.defData.Rank].Icon);
            uiMonster.SetCustomName(monster.insData.CustomName);
            uiMonster.SetCustomNameChangedCallback(static newName => DataManager.Ins.UpdateData_MonsterCustomName(Ins.monster.insData, newName));
            uiMonster.SetName(monster.defData.Name);
            uiMonster.SetLevel(monster.insData.Level, DataManager.Ins.GameDefData.MonsterRank[monster.defData.Rank].MaxLevel);

            var foodCost     = monster.CalculateStat(MonsterStatId.FoodCost);
            var foodCostDiv4 = Mathf.CeilToInt(foodCost / 4f);
            uiMonster.SetExp(monster.insData.Exp, foodCost);
            uiMonster.SetFeedAmount(foodCostDiv4);
            uiMonster.SetFeedCallback(() => {
                if (DataManager.Ins.UserInsData.Food < foodCostDiv4) {
                    NotificationWindow.Show(
                        title: "NOT ENOUGH FOOD"
                      , content: "Grow some food bro");
                    return;
                }

                FloatingTextPool.Ins
                    .ShowAtScreen(FloatingTextId.FoodChange, GetScreenPointOfRectCenter(uiMonster.FeedBtnRect))
                    .SetTextChange(-foodCostDiv4);

                DataManager.Ins.UpdateData_FeedMonster(monster.insData, foodCostDiv4, out var levelChanged);

                EventDispatcher.PostEvent(EventId.UserFoodChanged);
                EventDispatcher.PostEvent(EventId.MonsterFeedInMonsterDetail);
                if (levelChanged) EventDispatcher.PostEvent(EventId.MonsterLevelChangedInMonsterDetail);
            });
        }

        public Vector2 GetScreenPointOfRectCenter(RectTransform rect) {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            return RectTransformUtility.WorldToScreenPoint(
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera
              , (corners[0] + corners[2]) / 2f);
        }

        public void BackToHomeScene() {
            SceneManager.LoadScene("HomeScene");
        }
    }
}