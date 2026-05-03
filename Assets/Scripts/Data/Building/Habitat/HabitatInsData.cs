using System;
using NGDtuanh.Types;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class HabitatInsData : BuildingInsData {
        public ElementId Id;
        public long CurGold;
        public SerTimestamp LastGoldUpdate;

        public HabitatInsData(ElementId id) {
            OnInit();
            Id = id;
        }

        [OnInspectorInit]
        private void OnInit() {
            if (string.IsNullOrEmpty(InsId)) InsId = "Habitat_" + Guid.NewGuid();
        }
    }
}