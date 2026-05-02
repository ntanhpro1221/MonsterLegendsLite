using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class UI_SpecStat_TextIcon : UI_SpecStat {
        [SerializeField, Required]
        private TextMeshProUGUI mainTxt;

        [SerializeField, Required]
        private Image iconImg;

        public void SetText(string text) {
            mainTxt.text = text;
        }

        public void SetIcon(Sprite sprite) {
            iconImg.sprite = sprite;
        }
    }
}