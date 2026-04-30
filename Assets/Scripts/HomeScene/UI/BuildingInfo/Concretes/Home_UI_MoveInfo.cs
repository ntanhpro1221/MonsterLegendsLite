using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_MoveInfo : Home_UI_BuildingInfo {
        [SerializeField, Required]
        private Home_UI_BuildingInfoBtn discardBtn, confirmBtn;
        
        public override void LoadInfoFor(Home_Building building) {
            base.LoadInfoFor(building);
            
            discardBtn.SetCallback(() => {
                CurTarget.ResetPos();
                
                Home_SceneManager.Ins.ForceShowBuildingInfo(CurTarget);
            });
            
            confirmBtn.SetCallback(() => {
                CurTarget.SaveCurPos();
                
                Home_SceneManager.Ins.ForceShowBuildingInfo(CurTarget);
                
                EventDispatcher.PostEvent(EventId.HomeMapChanged);
            });

            CurTarget.onPlaceableChanged += OnTargetIsPlaceableChanged;
            
            Home_MapManager.Ins.SetVisibleGrid(true);
            CurTarget.SetVisibleValidPlace(true);
        }

        public override void UnloadInfo() {
            CurTarget.onPlaceableChanged -= OnTargetIsPlaceableChanged;
            
            Home_MapManager.Ins.SetVisibleGrid(false);
            CurTarget.SetVisibleValidPlace(false);
            
            base.UnloadInfo();
        }

        private void OnTargetIsPlaceableChanged(bool isPlaceable) {
            confirmBtn.SetInteractable(isPlaceable);
        }
    }
}