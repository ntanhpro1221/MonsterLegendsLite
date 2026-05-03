using NGDtuanh.MonsterLegendsLite;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class UI_RateBar : MonoBehaviourExt {
        [SerializeField]
        private Image fillImg;

        [SerializeField]
        private Gradient color;

        public float Rate {
            get => fillImg.fillAmount;
            set {
                fillImg.fillAmount = value;
                fillImg.color      = color.Evaluate(value);
            }
        }
    }
}