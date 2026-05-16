using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BreedingOutputConfigData {
        [Range(0, 100)]
        public float Weight;

        [MinValue(0)]
        public long Duration;
    }
}