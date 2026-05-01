using System;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class HabitatDefData {
        public string Name;
        public string Description;
        public ElementId Element;
        public int Capacity;
        public int MaxGold;
        public Vector2Int Size;
    }
}