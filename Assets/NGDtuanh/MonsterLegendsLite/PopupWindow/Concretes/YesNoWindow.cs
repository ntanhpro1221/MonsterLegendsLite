using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NGDtuanh.MonsterLegendsLite {
    public class YesNoWindow : PopupWindow {
        [SerializeField, Required]
        private Button yesBtn;
        
        [SerializeField, Required]
        private TextMeshProUGUI yesTxt;
        
        [SerializeField, Required]
        private Button noBtn;
        
        [SerializeField, Required]
        private TextMeshProUGUI noTxt;

        public static YesNoWindow Show(string title, string content, string yesText = "Yes", string noText = "No", Action yesCallback = null, Action noCallback = null, Action onClose = null) {
            var window = (YesNoWindow)PopupWindowPool.Ins.Show(PopupWindowId.YesNo, title, content, onClose);
            window.yesTxt.text = yesText;
            window.noTxt.text = noText;
            window.SetCallbackTo(window.yesBtn, yesCallback, appendClose: true);
            window.SetCallbackTo(window.noBtn, noCallback, appendClose: true);
            return window;
        }

        public YesNoWindow SetYesCallback(Action callback, bool appendClose) {
            SetCallbackTo(yesBtn, callback, appendClose);
            return this;
        }
        
        public YesNoWindow SetNoCallback(Action callback, bool appendClose) {
            SetCallbackTo(noBtn, callback, appendClose);
            return this;
        }
    }
}