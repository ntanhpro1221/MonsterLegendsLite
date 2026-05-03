using MonsterLegendsLite.Data;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Farm : Home_Building<FarmInsData> {
        public long CalculateCurTotalFood() {
            return DataManager.Ins.GameDefData.Farm[InsData.Id].CalculateFood(InsData);
        }

        protected override void UpdateData_BuyBuilding(Vector2Int pos, out int cost, out string insId) {
            DataManager.Ins.UpdateData_BuyFarm(InsData.Id, pos, out cost, out insId);
        }

        protected override Home_Building GetBuildingFromInsId(string insId) {
            return Home_SceneManager.Ins.Farms[insId];
        }
    }
}