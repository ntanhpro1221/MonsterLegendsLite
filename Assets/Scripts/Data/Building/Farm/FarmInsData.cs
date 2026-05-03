using System;
using NGDtuanh.Types;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class FarmInsData : BuildingInsData {
        public FarmId Id;
        public long CurFood;
        public SerTimestamp LastFoodUpdate;

        public static FarmInsData Create(FarmId id) => new() {
            InsId = "Farm_" + Guid.NewGuid()
          , Id    = id
        };
    }
}