using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Battle_UI_SkillList : MonoBehaviourExt {
        [SerializeField, Required]
        private MonsterSkillList<Battle_UI_SkillBtn> skillBtns;

        [SerializeField, Required]
        private Button rechargeBtn;

        public void SetAllData(Battle_Monster monster, UnityAction<Battle_Skill> onSelectSkill, UnityAction onSelectRecharge) {
            for (int i = 0; i < monster.SkillList.Count; ++i) {
                var skill = monster.SkillList[i];
                skillBtns[i].SetAllData(skill, monster.CurMP, () => onSelectSkill.Invoke(skill));
            }

            utils.SetListener(rechargeBtn, onSelectRecharge);
        }
    }
}