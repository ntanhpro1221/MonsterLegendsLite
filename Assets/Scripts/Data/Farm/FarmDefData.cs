using System;
using NGDtuanh.Types;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class FarmDefData {
        public string Name;
        public string Description;
        public long MaxFood;
        public int FoodPerMin;
        public Vector2Int Size;

        public long CalculateFood(FarmInsData insData) {
            var deltaFood = FoodPerMin * SerTimestamp.DeltaMinutes(SerTimestamp.GetCurTimestamp(), insData.LastFoodUpdate);
            return Math.Min(MaxFood, insData.CurFood + (long)deltaFood);
        }
    }
}