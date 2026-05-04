using System.Collections.Generic;
using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Battle_UserSkillSelector : Battle_SkillSelector {
        [SerializeField, Required]
        private Battle_UI_Skill uiSkill;

        private Battle_Monster curMonster;
        private OnSelected onSelected;
        private Battle_Skill selectingSkill;

        public override void SelectSkill(Battle_Monster monster, OnSelected onSelected) {
            this.curMonster     = monster;
            this.onSelected     = onSelected;
            this.selectingSkill = null;

            foreach (var item in IEAllMonster()) item.onClick += OnClickMonster;
            monster.SetVisibleTurnIndicator(true);
            uiSkill.Show(
                monster: monster
              , onSelectSkill: skill => {
                    selectingSkill = skill;

                    monster.SetDeltaMP(-skill.skillData.MPCost);

                    foreach (var target in (skill.skillData.Target.IsTargetEnemy() ? Enemies : Allies).Values)
                        target.SetDeltaHP(monster.CalcDeltaHP(skill, target));
                }
              , onDeselectSkill: () => {
                    selectingSkill = null;

                    monster.SetDeltaMP(0);

                    foreach (var target in IEAllMonster())
                        target.SetDeltaHP(0);
                }
              , onSelectRecharge: () => OnDoneSelect(
                    isRecharge: true
                  , skill: null
                  , targets: null));
        }

        private void OnClickMonster(Battle_Monster monster) {
            if (selectingSkill == null) return;
            if (selectingSkill.skillData.Target.IsTargetEnemy() != Enemies.ContainsKey(monster.insData.InsId)) return;

            OnDoneSelect(
                isRecharge: false
              , skill: selectingSkill
              , targets: GetTargets(selectingSkill, monster));
        }

        private void OnDoneSelect(bool isRecharge, Battle_Skill skill, IEnumerable<Battle_Monster> targets) {
            uiSkill.Hide();
            curMonster.SetVisibleTurnIndicator(false);
            foreach (var monster in IEAllMonster()) {
                monster.onClick -= OnClickMonster;
                monster.SetDeltaHP(0);
            }
            
            onSelected.Invoke(
                isRecharge: isRecharge
              , skill: skill
              , targets: targets);
        }
    }
}