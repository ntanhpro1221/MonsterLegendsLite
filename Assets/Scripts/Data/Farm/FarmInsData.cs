using System;
using NGDtuanh.Types;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class FarmInsData {
        public string InsId;
        public FarmId Id;
        public Vector2Int Position;
        public int CurFood;
        public SerTimestamp LastFoodUpdate;
    }
}