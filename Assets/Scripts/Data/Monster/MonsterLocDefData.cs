using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterLocDefData : IShopItemLocData {
        [Required, PreviewField]
        public Sprite Avatar; 
        
        [Required, PreviewField]
        public Sprite ShopAvatar; 
        
        [Required]
        public Home_Monster PrefabHomeScene;

        [Required]
        public Battle_Monster PrefabBattleScene;

        [Required]
        public MonsterDetail_Monster PrefabMonsterDetailScene;

        [Required]
        public Adventure_Monster PrefabMonsterAdventureScene;

        Sprite IShopItemLocData.ShopAvatar => ShopAvatar;
    }
}