using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_BuildingInfo : Home_UI_Info<Home_Building> {
        [SerializeField, Required]
        private Home_UI_BuildingInfoSharedData shared;
    }
}