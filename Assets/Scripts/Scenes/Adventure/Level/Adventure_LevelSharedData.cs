using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Adventure_LevelSharedData : MonoBehaviourExt {
        [field: SerializeField, Required]
        public AdventureLevelDetailWindow PrefabAdventureLevelDetailWindow { get; private set; }

        [field: SerializeField, Required]
        public SpriteRenderer BackgroundSpr { get; private set; }

        [field: SerializeField, Required, PreviewField]
        public EnumMap<Adventure_LevelState, Sprite> BackgroundVariants { get; private set; }

        [field: SerializeField, Required]
        public TextMeshPro IndexTxt { get; private set; }
        
        [field: SerializeField, Required]
        public SpriteRenderer IndicatorImg { get; private set; }
    }
}