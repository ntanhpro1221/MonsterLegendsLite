using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BuildingLocDefData : IShopBuildingLocData {
        [Required, PreviewField]
        public Sprite ShopAvatar;

        Sprite IShopItemLocData.ShopAvatar => ShopAvatar;
    }
}