using System;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class UserDefData {
        public int MaxLevel;
        public AnimationCurve ExpCost;

        public int CalcExpCost(int level) {
            return (int)ExpCost.Evaluate(level);
        }
    }
}