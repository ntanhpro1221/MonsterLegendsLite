using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class ElementLocData {
        [Required, PreviewField]
        public Sprite Icon;

        [Required, PreviewField]
        public Sprite SkillButtonBG;
        
        [Required, PreviewField]
        public Sprite ElementButton;
    }
}