using System;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterInsData {
        public string InsId;
        public MonsterId Id;
        public string CustomName;
        public int Level;
        public string Habitat;
    }
}