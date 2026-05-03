using System;
using NGDtuanh.Types;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class FarmDefData : BuildingDefData {
        public long MaxFood;
        public int FoodPerMin;

        public long CalculateFood(FarmInsData insData) {
            var deltaFood = FoodPerMin * SerTimestamp.DeltaMinutes(SerTimestamp.Now(), insData.LastFoodUpdate);
            return Math.Min(MaxFood, insData.CurFood + (long)deltaFood);
        }
    }
}