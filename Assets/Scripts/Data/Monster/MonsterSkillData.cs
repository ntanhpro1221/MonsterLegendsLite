using System;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterSkillData {
        public string Name;
        public string Description;
        public int UnlockAtLevel;
        public float AtkPercent;
        public int MPRequired;
        public int Cooldown;
    }
}