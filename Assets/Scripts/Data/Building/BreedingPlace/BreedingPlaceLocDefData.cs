using System;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BreedingPlaceLocDefData : BuildingLocDefData {
        [Required]
        public Home_BreedingPlace PrefabHomeScene;
    }
}