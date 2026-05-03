using System;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class HabitatLocDefData : BuildingLocDefData {
        [Required]
        public Home_Habitat PrefabHomeScene;
    }
}