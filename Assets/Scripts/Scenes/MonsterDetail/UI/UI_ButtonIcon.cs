using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class UI_ButtonIcon : MonoBehaviourExt {
        [SerializeField, Required]
        private Button button;

        [SerializeField, Required]
        private TextMeshProUGUI text;
        
        [SerializeField, Required]
        private Image icon;

        [SerializeField, Required]
        private AspectRatioFitterElement iconRatio;

        public void SetCallback(UnityAction callback) {
            utils.SetListener(button, callback);
        }

        public void SetText(string text) {
            this.text.text = text;
        }

        public void SetIcon(Sprite icon) {
            this.icon.sprite = icon;
            iconRatio.AspectRatio = icon.rect.width / icon.rect.height;
        }
    }
}