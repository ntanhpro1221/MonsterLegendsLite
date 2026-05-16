using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BreedingPlaceInsData : BuildingInsData {
        public BreedingPlaceId Id;

        [SerializeReference]
        public BreedingProcessData CurBreeding;

        public BreedingPlaceInsData(BreedingPlaceId id) {
            OnInit();
            Id = id;
        }

        [OnInspectorInit]
        private void OnInit() {
            if (string.IsNullOrEmpty(InsId)) InsId = "BreedingPlace_" + Guid.NewGuid();
        }
    }
}