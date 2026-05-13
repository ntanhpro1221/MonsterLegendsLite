using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Shop_MonsterList : Shop_ItemList {
        protected override IEnumerable<(IShopItemData, IShopItemLocData)> IEAllItemData() {
            var datas    = DataManager.Ins.GameDefData.Monsters;
            var locDatas = DataManager.Ins.GameLocDefData.Monsters;
            foreach (var (id, data) in datas) yield return (data, locDatas[id]);
        }

        protected override void DoBuyLogic(IShopItemData data, IShopItemLocData locData) {
            Home_BootData.InsAutoSpawn.SetData_BuyMonster(DataManager.Ins.GameDefData.Monsters.First(i => i.Value == data).Key);
            Shop_SceneManager.Ins.BackToHomeScene();
        }

        protected override bool IsCanBuy(IShopItemData data, IShopItemLocData locData, out string blockReason) {
            if (!DataManager.Ins.IsAnyHabitatCanAcceptNewMonster(new MonsterInsData(DataManager.Ins.GameDefData.Monsters.First(i => i.Value == data).Key))) {
                blockReason = $"You dont have any habitat that can accept {data.Name}";
                return false;
            }

            return base.IsCanBuy(data, locData, out blockReason);
        }
    }
}