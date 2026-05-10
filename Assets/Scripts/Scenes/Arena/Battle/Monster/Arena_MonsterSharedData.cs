using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Arena_MonsterSharedData : MonoBehaviourExt {
        [field: SerializeField, Required]
        public GameObject ContentRoot { get; private set; }
        
        [field: SerializeField, Required]
        public TextMeshProUGUI CustomNameTxt { get; private set; }
        
        [field: SerializeField, Required]
        public TextMeshProUGUI NameTxt { get; private set; }

        [field: SerializeField, Required]
        public TextMeshProUGUI LevelTxt { get; private set; }

        [field: SerializeField]
        public MonsterStats<TextMeshProUGUI> StatsTxt { get; private set; }

        [field: SerializeField, Required]
        public Image AvatarImg { get; private set; }

        [field: SerializeField, Required]
        public Image RankImg { get; private set; }

        [field: SerializeField, Required]
        public UI_SpecStat_Elements Elements { get; private set; }
    }
}