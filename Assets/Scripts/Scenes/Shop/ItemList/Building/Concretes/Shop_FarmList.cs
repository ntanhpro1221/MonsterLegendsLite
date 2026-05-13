using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Shop_FarmList : Shop_BuildingList {
        protected override IEnumerable<(IShopBuildingData, IShopBuildingLocData)> IEAllBuildingData() {
            var datas    = DataManager.Ins.GameDefData.Farms;
            var locDatas = DataManager.Ins.GameLocDefData.Farms;
            foreach (var (id, data) in datas) yield return (data, locDatas[id]);
        }

        protected override void DoBuyLogic(IShopItemData data, IShopItemLocData locData) {
            Home_BootData.InsAutoSpawn.SetData_BuyFarm(DataManager.Ins.GameDefData.Farms.First(i => i.Value == data).Key);
            Shop_SceneManager.Ins.BackToHomeScene();
        }
    }
}