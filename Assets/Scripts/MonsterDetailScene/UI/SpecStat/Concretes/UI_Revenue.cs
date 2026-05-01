using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MonsterLegendsLite.Concretes {
    public class UI_Revenue : UI_SpecStat {
        [SerializeField, Required]
        private TextMeshProUGUI revenueTxt;

        public void SetRevenue(int revenue) {
            revenueTxt.text = $"{revenue}/MIN";
        }
    }
}