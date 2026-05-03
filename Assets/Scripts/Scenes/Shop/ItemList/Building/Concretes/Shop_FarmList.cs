using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Shop_FarmList : Shop_BuildingList {
        protected override IEnumerable<(IShopBuildingData, IShopBuildingLocData)> IEAllBuildingData() {
            var datas    = DataManager.Ins.GameDefData.Farm;
            var locDatas = DataManager.Ins.GameLocDefData.Farm;
            foreach (var (id, data) in datas) yield return (data, locDatas[id]);
        }

        protected override void DoBuyLogic(IShopItemData data, IShopItemLocData locData) {
            Home_BootData.InsAutoSpawn.SetData_BuyFarm(DataManager.Ins.GameDefData.Farm.First(i => i.Value == data).Key);
            Shop_SceneManager.Ins.BackToHomeScene();
        }
    }
}