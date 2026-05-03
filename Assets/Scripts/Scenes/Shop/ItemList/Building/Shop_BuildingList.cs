using System.Collections.Generic;
using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public abstract class Shop_BuildingList : Shop_ItemList {
        [field: SerializeField, Required]
        protected Shop_BuildingListSharedData SharedDataBuilding { get; private set; }

        protected abstract IEnumerable<(IShopBuildingData data, IShopBuildingLocData locData)> IEAllBuildingData();

        protected sealed override IEnumerable<(IShopItemData, IShopItemLocData)> IEAllItemData() {
            foreach (var item in IEAllBuildingData()) yield return (item.data, item.locData);
        }
    }
}