using System;
using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;

namespace MonsterLegendsLite {
    public abstract class Battle_SkillSelector : MonoBehaviourExt {
        public abstract void Initialize(Dictionary<string, Battle_Monster> teamLeft, Dictionary<string, Battle_Monster> teamRight);
        public abstract void SelectSkill(Battle_Monster monster, Action<MonsterSkillData, Battle_Monster> onSelected);
    }
}