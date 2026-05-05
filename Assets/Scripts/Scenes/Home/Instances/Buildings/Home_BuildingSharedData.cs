using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace MonsterLegendsLite {
    public class Home_BuildingSharedData : MonoBehaviourExt {
        [SerializeField, Required]
        public SortingGroup sortingGroup;
        
        [SerializeField, Required]
        public Transform selectWrapper;
        
        [SerializeField, Required]
        public SpriteRenderer selectOutline;

        [SerializeField, Required]
        public SpriteRendererAnchorer[] arrowAnchors;

        [SerializeField, Required]
        public SpriteRenderer validPlaceSpr, invalidPlaceSpr;
    }
}