using System;
using System.Collections.Generic;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterInsData {
        public string InsId;
        public MonsterId Id;
        public string CustomName;
        public int Level;
        public int Exp;
        public string Habitat;
        public List<int> SkillIds;

        public static MonsterInsData Create(MonsterId id) => new() {
            InsId = "Monster_" + Guid.NewGuid()
          , Id    = id
          , Level = 1
        };
    }
}