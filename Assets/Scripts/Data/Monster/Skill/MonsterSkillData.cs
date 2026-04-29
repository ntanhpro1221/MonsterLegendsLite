using System;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterSkillData {
        public string Name;
        public string Description;
        public ElementId Element;
        public MonsterSkillTargetId Target;
        public int PowerRate;
        public int Accuracy;
        public int MPCost;
        public int Cooldown;
        public int UnlockAtLevel;
    }
}