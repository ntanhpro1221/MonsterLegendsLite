using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
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
                var food = farm.CalculateCurTotalFood();
                if (food <= 0) return;
                
                DataManager.Ins.UpdateData_CollectFood(farm.InsData);

                FloatingTextPool.Ins.ShowAtWorld(FloatingTextId.FoodChange, farm.TF.position).SetTextChange(food);
                
                UpdateTotalFood();
                
                EventDispatcher.PostEvent(EventId.UserFoodChanged);
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