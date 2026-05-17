using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class FarmInfoWindow : BuildingInfoWindow {
        [SerializeField, Required]
        private UI_SpecStat_TextIcon foodPerMin;
        
        [SerializeField, Required]
        private UI_SpecStat_TextIcon maxFood;

        public static FarmInfoWindow Show(FarmInfoWindow prefab, FarmInsData target) {
            var window  = BuildingInfoWindow.Show(prefab,target);
            var defData = DataManager.Ins.GameDef.Farms[target.To<FarmInsData>().Id];

            window.foodPerMin.SetText(window.utils.ToStr_Resource(defData.FoodPerMin));
            window.maxFood.SetText(window.utils.ToStr_Resource(defData.MaxFood));

            return window;
        }

        protected override string GetBuildingName(BuildingInsData target) {
            var defData = DataManager.Ins.GameDef.Farms[target.To<FarmInsData>().Id];
            return $"{defData.Name} Farm";
        }
    }
}