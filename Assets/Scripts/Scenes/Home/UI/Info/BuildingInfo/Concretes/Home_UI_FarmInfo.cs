using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_FarmInfo : Home_UI_BuildingInfo {
        [SerializeField]
        private Home_UI_InfoBtn collectBtn;

        protected override void LoadInfoFor(Home_Building building) {
            base.LoadInfoFor(building);
            
            var farm = (Home_Farm)building;
            
            LoadCollectBtn(farm);
        }
        
        private void LoadCollectBtn(Home_Farm farm) {
            UpdateTotalFood();
            
            collectBtn.SetCallback(() => {
                Home_Farm.DoCollectFood(farm);
                
                UpdateTotalFood();
            });
        }
        
        public void Update() {
            UpdateTotalFood();
        }

        private void UpdateTotalFood() {
            collectBtn.SetInfo(utils.ToStr_Resource(CurTarget.To<Home_Farm>().CalculateCurTotalFood()));
        }
    }
}