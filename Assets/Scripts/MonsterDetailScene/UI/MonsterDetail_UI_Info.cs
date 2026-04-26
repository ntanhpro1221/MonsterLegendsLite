using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MonsterLegendsLite {
    public class MonsterDetail_UI_Info : MonoBehaviourExt {
        [SerializeField]
        private MonsterStats<TextMeshProUGUI> statsTxt; 

        [SerializeField, Required]
        private TextMeshProUGUI descriptionTxt;

        public void SetStats(MonsterStats<int> stats) {
            foreach (var (key, text) in statsTxt) {
                if (text == null) continue;
                text.text = stats[key].ToString();
            }
        }

        public void SetDescription(string description) {
            descriptionTxt.text = description;
        }
    }
}