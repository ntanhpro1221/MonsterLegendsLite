using System;
using System.Collections.Generic;
using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Battle_UserSkillSelector : Battle_SkillSelector {
        private Action<MonsterSkillData, Battle_Monster> onSelected;

        public override void Initialize(Dictionary<string, Battle_Monster> teamLeft, Dictionary<string, Battle_Monster> teamRight) {
            
        }

        public override void SelectSkill(Battle_Monster monster, Action<MonsterSkillData, Battle_Monster> onSelected) {
            this.onSelected = onSelected;
        }
    }
}