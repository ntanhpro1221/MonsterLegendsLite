using System;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class HabitatDefData :BuildingDefData{
        public ElementId Element;
        public int Capacity;
        public int MaxGold;
    }
}