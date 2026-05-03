using System;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterInsData {
        public string InsId;
        public MonsterId Id;
        public string CustomName;
        public int Level;
        public int Exp;
        public string Habitat;
        public MonsterSkillList SkillList;

        public MonsterInsData(MonsterId id) {
            OnInit();
            Id = id;
        }

        [OnInspectorInit]
        private void OnInit() {
            if (string.IsNullOrEmpty(InsId)) InsId = "Monster_" + Guid.NewGuid();

            if (Level == 0) Level = 1;

            if ((SkillList ??= new()).IsAllEqual(0)) SkillList.WithAll(-1).With(0, 0);
        }

        public MonsterSkillData GetSkill(int slotId, MonsterDefData defData) {
            if (SkillList[slotId] < 0) return null;
            return defData.Skills[SkillList[slotId]];
        }
    }
}