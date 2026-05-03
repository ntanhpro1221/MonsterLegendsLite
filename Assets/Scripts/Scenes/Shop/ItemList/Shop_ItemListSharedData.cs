using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Shop_ItemListSharedData : MonoBehaviourExt {
        [field: SerializeField, Required]
        public RectTransform ItemRoot { get; private set; }
        
        [field: SerializeField, Required]
        public Shop_Item PrefabItem { get; private set; }
    }
}