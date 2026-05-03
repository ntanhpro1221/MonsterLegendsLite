using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public abstract class Shop_ItemList : MonoBehaviourExt {
        [field: SerializeField, Required]
        protected Shop_ItemListSharedData SharedData { get; private set; }

        protected abstract IEnumerable<(IShopItemData data, IShopItemLocData locData)> IEAllItemData();
        protected abstract void DoBuyLogic(IShopItemData data, IShopItemLocData locData);

        protected virtual bool IsCanBuy(IShopItemData data, IShopItemLocData locData, out string blockReason) {
            blockReason = default;
            return true;
        }

        public void Initialize() {
            BuildList();
        }

        private void BuildList() {
            foreach (var item in IEAllItemData()) SpawnItem(item.data, item.locData);

            LayoutRebuilder.ForceRebuildLayoutImmediate(SharedData.ItemRoot);
        }

        private void SpawnItem(IShopItemData data, IShopItemLocData locData) {
            var item = Instantiate(SharedData.PrefabItem, SharedData.ItemRoot);
            item.SetTitle(data.Name);
            item.SetAvatar(locData.ShopAvatar);
            item.SetBuyBtn(data.Cost, () => {
                if (DataManager.Ins.UserInsData.Gold < data.Cost) {
                    NotificationWindow.Show(
                        title: item.GetFailBuyWindowTitle()
                      , content: $"You don't have enough gold to buy {data.Name} ({data.Cost} gold is missing)");
                    return;
                }

                if (!IsCanBuy(data, locData, out var blockReason)) {
                    NotificationWindow.Show(
                        title: item.GetFailBuyWindowTitle()
                      , content: blockReason);
                    return;
                }

                DoBuyLogic(data, locData);
            });
        }
    }
}