using System.Linq;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Battle_BotSkillSelector : Battle_SkillSelector {
        // TODO: It's just random now, use some more intelligent algorithm in the future
        public override void SelectSkill(Battle_Monster monster, OnSelected onSelected) {
            var usableSkills = monster.SkillList.Where(i =>
                i != null
             && i.Cooldown == 0
             && i.skillData.MPCost <= monster.CurMP).ToList();

            var optionCnt = usableSkills.Count + 1;
            var optionId = Mathf.FloorToInt(Random.value / optionCnt);

            if (optionId == usableSkills.Count) {
                onSelected.Invoke(
                    isRecharge: true
                  , skill: null
                  , targets: null);
            } else {
                var skill = usableSkills[optionId];
                onSelected.Invoke(
                    isRecharge: false
                  , skill: skill
                  , targets: GetTargets(skill));
            }
        }
    }
}