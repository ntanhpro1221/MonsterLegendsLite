using System;
using NGDtuanh.Types;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class FarmInsData : BuildingInsData {
        public FarmId Id;
        public long CurFood;
        public SerTimestamp LastFoodUpdate;

        public FarmInsData(FarmId id) {
            OnInit();
            Id = id;
        }

        [OnInspectorInit]
        private void OnInit() {
            if (string.IsNullOrEmpty(InsId)) InsId = "Farm_" + Guid.NewGuid();
        }
    }
}