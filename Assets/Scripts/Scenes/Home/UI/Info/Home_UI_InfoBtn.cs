using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_InfoBtn : MonoBehaviourExt {
        [SerializeField, Required]
        private CanvasGroup canvasGroup;
        
        [SerializeField, Range(0, 1)]
        private float alphaNonInteractable;
        
        [SerializeField, Required]
        private Button button;

        [SerializeField, Required]
        private Image iconImg;

        [SerializeField, Required]
        private TextMeshProUGUI titleTxt;

        [SerializeField, Required]
        private TextMeshProUGUI infoTxt;

        public void SetCallback(UnityAction callback) {
            utils.SetListener(button, callback);
        }

        public void SetInteractable(bool interactable) {
            button.interactable = interactable;
            canvasGroup.alpha = interactable ? 1 : alphaNonInteractable;
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