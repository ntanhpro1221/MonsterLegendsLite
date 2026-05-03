using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class MonsterDetail_UI_SkillDetailSharedData : MonoBehaviourExt {
        [field: SerializeField, Required]
        public Image ElementImg { get; private set; }

        [field: SerializeField, Required]
        public TextMeshProUGUI NameTxt { get; private set; }

        [field: SerializeField, Required]
        public TextMeshProUGUI DescriptionTxt { get; private set; }

        [field: SerializeField, Required]
        public TextMeshProUGUI TargetTxt { get; private set; }

        [field: SerializeField, Required]
        public Image PowerIconEnemy { get; private set; }

        [field: SerializeField, Required]
        public Image PowerIconAlly { get; private set; }

        [field: SerializeField, Required]
        public UI_RateBar PowerRate { get; private set; }

        [field: SerializeField, Required]
        public UI_RateBar Accuracy { get; private set; }

        [field: SerializeField, Required]
        public TextMeshProUGUI MPCostTxt { get; private set; }

        [field: SerializeField, Required]
        public TextMeshProUGUI CooldownTxt { get; private set; }
    }
}