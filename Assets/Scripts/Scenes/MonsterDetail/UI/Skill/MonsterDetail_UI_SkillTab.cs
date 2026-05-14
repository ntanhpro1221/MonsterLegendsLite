using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class MonsterDetail_UI_SkillTab : MonoBehaviourExt {
        [SerializeField, Required]
        private Toggle toggle;
        
        [SerializeField, Required]
        private Image elementImg;

        [SerializeField, Required]
        private TextMeshProUGUI nameTxt;

        private UnityAction turnOnCallback;

        public bool IsOn => toggle.isOn;
        
        private void Awake() {
            toggle.onValueChanged.AddListener(isOn => {
                if (isOn) turnOnCallback?.Invoke();
            });
        }

        public void SetElement(Sprite element) {
            elementImg.sprite = element;
            utils.SetAlpha(elementImg, element != null ? 1 : 0);
        }

        public void SetName(string name) {
            nameTxt.text = name;
        }

        public void SetTurnOnCallback(UnityAction callback) {
            turnOnCallback = callback;
        }
    }
}