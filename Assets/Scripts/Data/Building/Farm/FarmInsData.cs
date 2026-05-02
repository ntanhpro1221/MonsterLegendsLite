using System;
using NGDtuanh.Types;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class FarmInsData : BuildingInsData {
        public FarmId Id;
        public long CurFood;
        public SerTimestamp LastFoodUpdate;
    }
}