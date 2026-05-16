using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_BuildingInfoSharedData : MonoBehaviourExt {
        [SerializeField, Required]
        public Home_UI_InfoBtn infoBtn;

        [SerializeField, Required]
        public HabitatInfoWindow prefabInfoWindow_Habitat;

        [SerializeField, Required]
        public FarmInfoWindow prefabInfoWindow_Farm;

        [SerializeField, Required]
        public BreedingPlaceInfoWindow prefabInfoWindow_BreedingPlace;
    }
}