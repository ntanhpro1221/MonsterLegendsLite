using System;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Battle_UI_Turn : MonoBehaviourExt {
        [Serializable]
        public class OutlineSet {
            [SerializeField, Required]
            public Image outline, outlineGlow;

            private readonly UtilFuncs utils = new();

            public void SetVisible(bool visible) {
                float alpha = visible ? 1 : 0;
                utils.SetAlpha(outline, alpha);
                utils.SetAlpha(outlineGlow, alpha);
            }

            public void SetGlow(bool glow) {
                outline.enabled     = !glow;
                outlineGlow.enabled = glow;
            }
        }

        [SerializeField, Required]
        private RectTransform offsetLayer;
        
        [SerializeField, Required]
        private LayoutElement layoutElement;
        
        [SerializeField, Required]
        private Image avatar;

        [SerializeField]
        private OutlineSet ally, enemy;

        public RectTransform OffsetLayer => offsetLayer;

        public void SetAvatar(Sprite avatar) {
            this.avatar.sprite = avatar;
        }

        public void SetTeam(bool isAlly) {
            ally.SetVisible(isAlly);
            enemy.SetVisible(!isAlly);
        }

        public void SetGlow(bool isGlow) {
            ally.SetGlow(isGlow);
            enemy.SetGlow(isGlow);
        }

        public float GetMinHeight() => layoutElement.minHeight;
        public void SetMinHeight(float value) => layoutElement.minHeight = value;
    }
}