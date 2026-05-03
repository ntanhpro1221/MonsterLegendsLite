using System;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class FarmLocDefData : BuildingLocDefData {
        [Required]
        public Home_Farm PrefabHomeScene;
    }
}