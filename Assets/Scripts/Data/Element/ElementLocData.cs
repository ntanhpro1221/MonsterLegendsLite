using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class ElementLocData {
        [Required]
        public Sprite Icon;

        [Required]
        public Sprite SkillButtonBG;
        
        [Required]
        public Sprite ElementButton;
    }
}