using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_BuildingInfoBtn : MonoBehaviourExt {
        [SerializeField, Required]
        private Button button;

        [SerializeField, Required]
        private Image iconImg;

        [SerializeField, Required]
        private TextMeshProUGUI titleTxt;

        [SerializeField, Required]
        private TextMeshProUGUI infoTxt;
    }
}