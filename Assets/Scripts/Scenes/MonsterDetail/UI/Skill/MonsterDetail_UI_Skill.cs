using System;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class MonsterDetail_UI_Skill : MonoBehaviourExt {
        [NonSerialized, ShowInInspector, ReadOnly]
        private int curSlotId;

        [SerializeField, Required]
        private MonsterDetail_UI_SkillDetail skillDetail;

        [SerializeField, Required]
        private GameObject skillDetailNone;

        [SerializeField, Required]
        private UI_ButtonIcon replaceSkillBtn;

        [SerializeField, Required]
        private ReplaceSkillWindow replaceSkillWindowPrefab;

        [SerializeField, Required]
        private MonsterSkillList<MonsterDetail_UI_SkillTab> tabs;

        public void SetAllData(MonsterInsData insData) {
            var defData        = DataManager.Ins.GameDefData.Monsters[insData.Id];
            var elementLocData = DataManager.Ins.GameLocDefData.Elements;

            replaceSkillBtn.SetCallback(() => ReplaceSkillWindow.Show(replaceSkillWindowPrefab, curSlotId, insData));

            for (int slotId = 0; slotId < tabs.Count; ++slotId) {
                var tab          = tabs[slotId];
                var skill        = insData.GetSkill(slotId, defData);
                var cachedSlotId = slotId;

                if (skill == null) {
                    tab.SetElement(null);
                    tab.SetName("None");
                    tab.SetTurnOnCallback(() => {
                        curSlotId = cachedSlotId;
                        ToggleNoneSkillDetail(isNone: true);
                    });
                } else {

                    tab.SetElement(elementLocData[skill.Element].ElementButton);
                    tab.SetName(skill.Name);
                    tab.SetTurnOnCallback(() => {
                        curSlotId = cachedSlotId;
                        ToggleNoneSkillDetail(isNone: false);
                        skillDetail.SetAllData(skill, elementLocData[skill.Element].ElementButton);
                    });
                }
            }

            tabs[curSlotId].TurnOn();
        }

        private void ToggleNoneSkillDetail(bool isNone) {
            skillDetail.gameObject.SetActive(!isNone);
            skillDetailNone.SetActive(isNone);
            replaceSkillBtn.SetText((isNone ? "SET" : "REPLACE") + " SKILL");
        }
    }
}