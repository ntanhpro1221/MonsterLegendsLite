using System.Linq;
using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class HabitatInfoWindow : BuildingInfoWindow {
        [SerializeField, Required]
        private UI_SpecStat_Elements elements;
        
        [SerializeField, Required]
        private UI_SpecStat_TextIcon capacity;

        [SerializeField, Required]
        private UI_SpecStat_TextIcon maxGold;

        public static HabitatInfoWindow Show(HabitatInfoWindow prefab, HabitatInsData target) {
            var window  = BuildingInfoWindow.Show(prefab,target);
            var defData = DataManager.Ins.GameDef.Habitats[target.To<HabitatInsData>().Id];

            window.elements.SetElements(DataManager.Ins.GameLocDef.Elements[defData.Element].ElementButton);
            window.capacity.SetText(defData.Capacity.ToString());
            window.maxGold.SetText(window.utils.ToStr_Resource(defData.MaxGold));

            return window;
        }

        protected override string GetBuildingName(BuildingInsData target) {
            var defData = DataManager.Ins.GameDef.Habitats[target.To<HabitatInsData>().Id];
            return $"{defData.Name} Habitat";
        }

        protected override bool IsCanSell(BuildingInsData target, out string blockReason) {
            if (DataManager.Ins.User.Monsters.Count(i => i.Habitat == target.InsId) > 0) {
                blockReason = $"{GetBuildingName(target)} can't be sold right now because it has monsters inside";
                return false;
            }

            return base.IsCanSell(target, out blockReason);
        }
    }
}