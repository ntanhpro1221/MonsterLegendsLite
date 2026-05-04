using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class MonsterDetail_UI_Info : MonoBehaviourExt {
        [SerializeField]
        private MonsterStats<TextMeshProUGUI> statsTxt; 
        
        [SerializeField, Required]
        private UI_SpecStat_Elements elements;
        
        [SerializeField, Required]
        private UI_SpecStat_Revenue revenue;

        [SerializeField, Required]
        private TextMeshProUGUI descriptionTxt;

        [SerializeField, Required]
        private Button moveBtn, sellBtn;

        public void SetStats(MonsterStats<int> stats) {
            foreach (var (key, text) in statsTxt) {
                if (text == null) continue;
                text.text = stats[key].ToString();
            }
        }

        public void SetElements(params Sprite[] elementSprites) {
            elements.SetElements(elementSprites);
        }
        
        public void SetRevenue(int revenue) {
            this.revenue.SetRevenue(revenue);
        }

        public void SetDescription(string description) {
            descriptionTxt.text = description;
        }

        public void SetMoveBtnCallback(UnityAction callback) {
            utils.SetListener(moveBtn, callback);
        }
        
        public void SetSellBtnCallback(UnityAction callback) {
            utils.SetListener(sellBtn, callback);
        }
    }
}