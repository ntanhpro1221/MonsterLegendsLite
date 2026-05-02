using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MonsterLegendsLite {
    public class UI_SpecStat_Revenue : UI_SpecStat {
        [SerializeField, Required]
        private TextMeshProUGUI revenueTxt;

        public void SetRevenue(int revenue) {
            revenueTxt.text = $"{revenue}/MIN";
        }
    }
}