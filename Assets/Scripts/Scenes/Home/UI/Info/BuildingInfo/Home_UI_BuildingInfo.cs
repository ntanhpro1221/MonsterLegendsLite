using System;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_BuildingInfo : Home_UI_Info<Home_Building> {
        [SerializeField, Required]
        protected Home_UI_BuildingInfoSharedData shared;

        protected override void LoadInfoFor(Home_Building building) {
            base.LoadInfoFor(building);

            shared.infoBtn.SetCallback(() => {
                switch (building) {
                    case Home_Habitat habitat:             HabitatInfoWindow.Show(shared.prefabInfoWindow_Habitat, habitat.InsData); break;
                    case Home_Farm farm:                   FarmInfoWindow.Show(shared.prefabInfoWindow_Farm, farm.InsData); break;
                    case Home_BreedingPlace breedingPlace: BreedingPlaceInfoWindow.Show(shared.prefabInfoWindow_BreedingPlace, breedingPlace.InsData); break;

                    default: throw new Exception($"Fail to show info window for {building.GetType().Name}");
                }
            });

            EventDispatcher.RegisterEvent(EventId.UserBuildingListChanged, HideIfTargetNotInDatabase, this);
        }

        public override void UnloadInfo() {
            base.UnloadInfo();
            
            EventDispatcher.UnregisterEvent(EventId.UserBuildingListChanged, HideIfTargetNotInDatabase, this);
        }

        private void HideIfTargetNotInDatabase() {
            if (DataManager.Ins.IsHaveBuilding(CurTarget.InsDataWeak)) return;
            Home_SceneManager.Ins.TryHideCurBuildingInfo();
        }
    }
}