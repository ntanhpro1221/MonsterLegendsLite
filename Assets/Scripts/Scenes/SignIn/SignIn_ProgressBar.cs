using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class SignIn_ProgressBar : MonoBehaviourExt {
        [SerializeField, Required]
        private Image fillImg;
        
        [SerializeField, Required]
        private TextMeshProUGUI progressTxt;
        
        public void SetVisible(bool visible) {
            gameObject.SetActive(visible);
        }

        public void SetProgress(float progress) {
            fillImg.fillAmount = progress;
            progressTxt.text   = (int)(progress * 100) + "%";
        }
    }
}