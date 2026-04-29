using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class MonsterDetail_UI_Monster : MonoBehaviourExt {
        [SerializeField, Required]
        private Image rankIconImg;
        
        [SerializeField, Required]
        private TMP_InputField customNameInput;

        [SerializeField, Required]
        private TextMeshProUGUI nameTxt;
        
        [SerializeField, Required]
        private TextMeshProUGUI levelTxt;

        [SerializeField, Required]
        private Image expFillerImg;

        [SerializeField, Required]
        private TextMeshProUGUI expTxt;

        [SerializeField, Required]
        private Button feedBtn;
        
        [SerializeField, Required]
        private TextMeshProUGUI feedBtnTxt;

        public void SetRankIcon(Sprite icon) {
            rankIconImg.sprite = icon;
        }

        public void SetCustomName(string customName) {
            customNameInput.text = customName;
        }

        public void SetName(string name) {
            nameTxt.text = name;
        }

        public void SetLevel(int curLevel, int maxLevel) {
            levelTxt.text = $"Level {curLevel}/{maxLevel}";
        }

        public void SetExp(int curExp, int requiredExp) {
            expTxt.text           = $"XP {curExp}/{requiredExp}";
            expFillerImg.fillAmount = (float)curExp / requiredExp;
        }

        public void SetFoodRequired(int foodRequired) {
            feedBtnTxt.text = $"FEED {utils.ToStrResource(foodRequired)}";
        }
    }
}