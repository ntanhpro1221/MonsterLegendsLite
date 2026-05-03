using UnityEngine;

namespace MonsterLegendsLite.Data {
    public interface IShopBuildingData :  IShopItemData {
        Vector2Int Size { get; }
    }
}