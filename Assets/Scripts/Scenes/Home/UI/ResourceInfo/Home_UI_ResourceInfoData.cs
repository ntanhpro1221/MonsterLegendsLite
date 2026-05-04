using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite.UI {
    public class Home_UI_ResourceInfoData : MonoBehaviourExt {
        [SerializeField, Required]
        private TextMeshProUGUI text;

        [SerializeField, Required]
        private Button button;

        public void SetText(string content) {
            text.text = content;
        }

        public void SetValue(long value) {
            text.text = utils.ToStrResource(value);
        }

        public void SetCallback(UnityAction callback) {
            utils.SetListener(button, callback);
        }
    }
}