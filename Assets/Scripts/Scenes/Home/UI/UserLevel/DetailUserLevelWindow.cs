using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class DetailUserLevelWindow : PopupWindow {
        [SerializeField, Required]
        private TextMeshProUGUI pointToNextLevelTxt;

        [SerializeField, Required]
        private Image expFillImg;

        [SerializeField, Required]
        private TextMeshProUGUI levelTxt;

        public static DetailUserLevelWindow Show(
            DetailUserLevelWindow prefab
          , UserInsData data
          , int requiredExp) {
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: null
              , content: null
              , onDoneClose: null);

            window.SetExp(data.Exp, requiredExp);
            window.SetLevel(data.Level);

            return window;
        }

        public void SetExp(int curExp, int requiredExp) {
            pointToNextLevelTxt.text = $"{utils.ToStr_Resource(curExp)}/{utils.ToStr_Resource(requiredExp)}";
            expFillImg.fillAmount    = (float)curExp / requiredExp;
        }

        public void SetLevel(int level) {
            levelTxt.text = level.ToString();
        }
    }
}