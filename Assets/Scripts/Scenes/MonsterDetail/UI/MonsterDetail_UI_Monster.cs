using System;
using System.Collections.Generic;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        private TextMeshProUGUI feedBtnTxt;

        [SerializeField, Required]
        private Button feedBtn;
        
        public RectTransform FeedBtnRect => (RectTransform)feedBtn.transform;

        public void SetRankIcon(Sprite icon) {
            rankIconImg.sprite = icon;
        }

        public void SetCustomName(string customName) {
            customNameInput.text = customName;
        }

        public void SetCustomNameChangedCallback(Action<string> callback) {
            customNameInput.onValueChanged.RemoveAllListeners();
            customNameInput.onValueChanged.AddListener(new(callback));
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

        public void SetFeedAmount(int feedAmount) {
            feedBtnTxt.text = $"FEED {utils.ToStrResource(feedAmount)}";
        }

        public void SetFeedCallback(UnityAction callback) {
            feedBtn.onClick.RemoveAllListeners();
            feedBtn.onClick.AddListener(callback);
        }
    }
}