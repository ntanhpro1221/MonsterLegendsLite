using System.Collections.Generic;
using MonsterLegendsLite.Concretes;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class MonsterDetail_UI_Info : MonoBehaviourExt {
        [SerializeField]
        private MonsterStats<TextMeshProUGUI> statsTxt; 
        
        [SerializeField, Required]
        private UI_Elements elements;
        
        [SerializeField, Required]
        private UI_Revenue revenue;

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

        public void SetElements(List<Sprite> elementSprites) {
            elements.SetElements(elementSprites);
        }
        
        public void SetRevenue(int revenue) {
            this.revenue.SetRevenue(revenue);
        }

        public void SetDescription(string description) {
            descriptionTxt.text = description;
        }
    }
}