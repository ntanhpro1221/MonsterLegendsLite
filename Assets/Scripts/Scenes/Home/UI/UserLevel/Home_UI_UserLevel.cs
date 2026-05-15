using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_UserLevel : MonoBehaviourExt {
        [SerializeField, Required]
        private DetailUserLevelWindow prefabDetailUserLevelWindow;

        [SerializeField, Required]
        private Button button;

        [SerializeField, Required]
        private Image fillImg;

        [SerializeField, Required]
        private Image avatarImg;

        [SerializeField, Required]
        private TextMeshProUGUI levelTxt;

        public void Initialize() {
            RebuildAll();

            EventDispatcher.RegisterEvent(EventId.UserExpOrLevelChanged, RebuildAll, this);
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(EventId.UserExpOrLevelChanged, RebuildAll, this);
        }

        private void RebuildAll() {
            var data        = DataManager.Ins.User;
            var requiredExp = DataManager.Ins.GameDef.User.CalcExpCost(data.Level);

            fillImg.fillAmount = (float)data.Exp / requiredExp;
            avatarImg.sprite   = avatarImg.sprite;
            levelTxt.text      = data.Level.ToString();

            button.onClick.AddListener(() => DetailUserLevelWindow.Show(
                prefab: prefabDetailUserLevelWindow
              , data: data
              , requiredExp: requiredExp
            ));
        }
    }
}