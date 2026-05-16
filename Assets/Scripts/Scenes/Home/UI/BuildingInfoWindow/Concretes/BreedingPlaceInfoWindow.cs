using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class BreedingPlaceInfoWindow : BuildingInfoWindow {
        public static BreedingPlaceInfoWindow Show(BreedingPlaceInfoWindow prefab, BreedingPlaceInsData target) {
            var window = BuildingInfoWindow.Show(prefab,target);

            return window;
        }

        protected override string GetBuildingName(BuildingInsData target) {
            var defData = DataManager.Ins.GameDef.BreedingPlaces[target.To<BreedingPlaceInsData>().Id];
            return $"Breeding {defData.Name}";
        }

        protected override bool IsCanSell(BuildingInsData target, out string blockReason) {
            if (target.To<BreedingPlaceInsData>().CurBreeding != null) {
                blockReason = $"{GetBuildingName(target)} can't be sold right now because it is woring";
                return false;
            }

            return base.IsCanSell(target, out blockReason);
        }
    }
}