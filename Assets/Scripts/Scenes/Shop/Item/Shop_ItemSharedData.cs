using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Shop_ItemSharedData : MonoBehaviourExt {
        [field: SerializeField, Required]
        public TextMeshProUGUI TitleTxt { get; private set; }

        [field: SerializeField, Required]
        public Image AvatarImg { get; private set; }

        [field: SerializeField, Required]
        public AspectRatioFitter AvatarRatio { get; private set; }

        [field: SerializeField, Required]
        public UI_ButtonIcon BuyBtn { get; private set; }
    }
}