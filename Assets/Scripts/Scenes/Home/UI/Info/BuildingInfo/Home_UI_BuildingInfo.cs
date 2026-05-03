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
                    case Home_Farm farm:       FarmInfoWindow.Show(farm.InsData, shared.prefabInfoWindow_Farm); break;
                    case Home_Habitat habitat: HabitatInfoWindow.Show(habitat.InsData, shared.prefabInfoWindow_Habitat); break;

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