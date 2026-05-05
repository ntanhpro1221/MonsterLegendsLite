using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

        public static YesNoWindow Show(
            string title
          , string content
          , string yesText = "Yes"
          , string noText = "No"
          , UnityAction yesCallback = null
          , UnityAction noCallback = null
          , UnityAction onDoneClose = null) {
            var window = PopupWindowPool.Ins.Show<YesNoWindow>(PopupWindowId.YesNo, title, content, onDoneClose);
            window.yesTxt.text = yesText;
            window.noTxt.text = noText;
            window.SetCallbackTo(window.yesBtn, yesCallback, afterClose: true);
            window.SetCallbackTo(window.noBtn, noCallback, afterClose: true);
            return window;
        }

        public YesNoWindow SetYesCallback(UnityAction callback, bool afterClose) {
            SetCallbackTo(yesBtn, callback, afterClose);
            return this;
        }
        
        public YesNoWindow SetNoCallback(UnityAction callback, bool afterClose) {
            SetCallbackTo(noBtn, callback, afterClose);
            return this;
        }
    }
}