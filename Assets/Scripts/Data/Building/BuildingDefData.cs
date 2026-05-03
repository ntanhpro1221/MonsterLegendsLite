using System;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BuildingDefData : IShopBuildingData {
        public string Name;
        public string Description;
        public Vector2Int Size;
        public int Cost;

        string IShopItemData.Name => Name;
        string IShopItemData.Description => Description;
        int IShopItemData.Cost => Cost;
        Vector2Int IShopBuildingData.Size => Size;
    }
}