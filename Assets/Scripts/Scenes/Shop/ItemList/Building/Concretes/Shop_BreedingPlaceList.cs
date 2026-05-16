using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Shop_BreedingPlaceList : Shop_BuildingList {
        protected override IEnumerable<(IShopBuildingData, IShopBuildingLocData)> IEAllBuildingData() {
            var datas    = DataManager.Ins.GameDef.BreedingPlaces;
            var locDatas = DataManager.Ins.GameLocDef.BreedingPlaces;
            foreach (var (id, data) in datas) yield return (data, locDatas[id]);
        }

        protected override void DoBuyLogic(IShopItemData data, IShopItemLocData locData) {
            Home_BootData.InsAutoSpawn.SetData_BuyBreedingPlace(DataManager.Ins.GameDef.BreedingPlaces.First(i => i.Value == data).Key);
            Shop_SceneManager.Ins.BackToHomeScene();
        }
    }
}