using System;
using NGDtuanh.Types;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class HabitatInsData : BuildingInsData {
        public ElementId Id;
        public long CurGold;
        public SerTimestamp LastGoldUpdate;

        public static HabitatInsData Create(ElementId id) => new() {
            InsId = "Habitat_" + Guid.NewGuid()
          , Id    = id
        };
    }
}