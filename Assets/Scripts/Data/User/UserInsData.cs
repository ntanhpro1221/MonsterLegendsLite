using System;
using System.Collections.Generic;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class UserInsData {
        public string Name;
        public int Level;
        public int Exp;
        public long Gold;
        public long Food;
        public List<MonsterInsData> Monsters;
        public List<HabitatInsData> Habitats;
        public List<FarmInsData> Farms;
    }
}