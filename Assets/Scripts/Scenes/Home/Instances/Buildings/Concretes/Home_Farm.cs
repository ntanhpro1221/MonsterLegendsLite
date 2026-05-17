using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Farm : Home_Building<FarmInsData> {
        public long CalculateCurTotalFood() {
            return DataManager.Ins.GameDef.Farms[InsData.Id].CalculateFood(InsData);
        }

        protected override void UpdateData_BuyBuilding(Vector2Int pos, out int cost, out string insId) {
            DataManager.Ins.UpdateData_BuyFarm(InsData.Id, pos, out cost, out insId);
        }

        protected override Home_Building GetBuildingFromInsId(string insId) {
            return Home_SceneManager.Ins.Farms[insId];
        }

        protected override bool IsShouldCollectBtnActive(out Sprite sprite) {
            sprite = null;

            float minToShowCollectBtn =
                DataManager.Ins.GameDef.Home_ShowCollectResourceBtnThreshold
              * DataManager.Ins.GameDef.Farms[InsData.Id].MaxFood;

            return CalculateCurTotalFood() >= minToShowCollectBtn;
        }

        protected override void DoClickCollectBtn() {
            DoCollectFood(this);
        }

        public static void DoCollectFood(Home_Farm farm) {
            var food = farm.CalculateCurTotalFood();
            if (food <= 0) return;
                
            DataManager.Ins.UpdateData_CollectFood(farm.InsData);

            FloatingTextPool.Ins.ShowAtWorld(FloatingTextId.FoodChange, farm.TF.position).SetTextChange(food);
                
            EventDispatcher.PostEvent(EventId.UserFoodChanged);
        }
    }
}