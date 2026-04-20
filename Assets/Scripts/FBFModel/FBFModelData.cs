using NGDtuanh.MonsterLegends;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class FBFModelData : MonoBehaviourExt {
        [field: SerializeField, Required]
        public Animator Animator { get; private set; }
        
        [field: SerializeField, Required]
        public SpriteRenderer Sprite { get; private set; }
    }
}