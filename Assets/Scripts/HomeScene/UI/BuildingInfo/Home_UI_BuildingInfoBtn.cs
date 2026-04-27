using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_BuildingInfoBtn : MonoBehaviourExt {
        [SerializeField, Required]
        private Button button;

        [SerializeField, Required]
        private Image iconImg;

        [SerializeField, Required]
        private TextMeshProUGUI titleTxt;

        [SerializeField, Required]
        private TextMeshProUGUI infoTxt;

        public void SetCallback(UnityAction callback) {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }

        public void SetIcon(Sprite icon) {
            iconImg.sprite = icon;
        }

        public void SetTitle(string title) {
            titleTxt.text = title;
        }

        public void SetInfo(string info) {
            infoTxt.text = info;
        }
    }
}