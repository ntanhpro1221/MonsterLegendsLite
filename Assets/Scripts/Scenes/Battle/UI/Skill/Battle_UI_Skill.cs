using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace MonsterLegendsLite {
    public class Battle_UI_Skill : MonoBehaviourExt {
        [SerializeField, Required]
        private Battle_UI_SkillList uiList;

        [SerializeField, Required]
        private Battle_UI_SkillDetail uiDetail;

        public void Show(
            Battle_Monster monster
          , UnityAction<Battle_Skill> onSelectSkill
          , UnityAction onDeselectSkill
          , UnityAction onSelectRecharge) {
            gameObject.SetActive(true);

            SetActiveListAndDetail(isListActive: true);
            uiList.SetAllData(
                monster: monster
              , onSelectSkill: skill => {
                    onSelectSkill.Invoke(skill);

                    SetActiveListAndDetail(isListActive: false);
                    uiDetail.SetAllData(skill, onBack: () => {
                        onDeselectSkill.Invoke();

                        SetActiveListAndDetail(isListActive: true);
                    });
                }
              , onSelectRecharge: onSelectRecharge);
        }

        private void SetActiveListAndDetail(bool isListActive) {
            uiList.gameObject.SetActive(isListActive);
            uiDetail.gameObject.SetActive(!isListActive);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}