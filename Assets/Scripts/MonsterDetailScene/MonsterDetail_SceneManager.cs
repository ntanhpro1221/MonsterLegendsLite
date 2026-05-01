using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class MonsterDetail_SceneManager : SceneSingleton<MonsterDetail_SceneManager> {
        [SerializeField, Required]
        private Canvas canvas;
        
        [SerializeField, Required]
        private MonsterDetail_UI_Info uiInfo;
        
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
            UpdateUI_Monster();

            EventDispatcher.RegisterEvent(EventId.MonsterLevelChangedInMonsterDetail, UpdateUI_Info, this);
            EventDispatcher.RegisterEvent(EventId.MonsterFeedInMonsterDetail, UpdateUI_Monster, this);
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(EventId.MonsterLevelChangedInMonsterDetail, UpdateUI_Info, this);
            EventDispatcher.UnregisterEvent(EventId.MonsterFeedInMonsterDetail, UpdateUI_Monster, this);
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