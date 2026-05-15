using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class FarmInfoWindow : BuildingInfoWindow {
        [SerializeField, Required]
        private UI_SpecStat_TextIcon maxFood;

        public static FarmInfoWindow Show(FarmInsData target, FarmInfoWindow prefab) {
            var window  = BuildingInfoWindow.Show(target, prefab);
            var defData = DataManager.Ins.GameDef.Farms[target.To<FarmInsData>().Id];

            window.maxFood.SetText(window.utils.ToStrResource(defData.MaxFood));

            return window;
        }

        protected override string GetBuildingName(BuildingInsData target) {
            var defData = DataManager.Ins.GameDef.Farms[target.To<FarmInsData>().Id];
            return $"{defData.Name} Farm";
        }
    }
}