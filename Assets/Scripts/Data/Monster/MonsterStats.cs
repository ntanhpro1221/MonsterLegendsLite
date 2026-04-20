using System;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterStats<T> {
        public T HP;
        public T MP;
        public T Speed;
        public T Atk;
        public T GoldPerMin;
        public T FoodCost;
    }
}