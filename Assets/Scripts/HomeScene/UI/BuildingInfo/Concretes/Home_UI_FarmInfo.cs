using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_FarmInfo : Home_UI_BuildingInfo {
        [SerializeField]
        private Home_UI_BuildingInfoBtn collectBtn;

        public override void LoadInfoFor(Home_Building building) {
            base.LoadInfoFor(building);
            
            UpdateTotalFood();
        }
        
        public void Update() {
            UpdateTotalFood();
        }

        private void UpdateTotalFood() {
            collectBtn.SetInfo(Home_SceneManager.Ins.Farms[CurTargetId].CalculateCurTotalFood().ToString());
        }
    }
}