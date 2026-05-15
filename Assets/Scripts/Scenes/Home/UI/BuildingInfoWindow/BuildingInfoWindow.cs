using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public abstract class BuildingInfoWindow : PopupWindow {
        [field: SerializeField, Required]
        protected BuildingInfoWindowSharedData SharedData_Building { get; private set; }
        
        protected static TBuildingInfoWindow Show<TBuildingInfoWindow>(
            BuildingInsData target
          , TBuildingInfoWindow prefab) 
            where TBuildingInfoWindow : BuildingInfoWindow {
            var defData        = DataManager.Ins.GetBuildingDefData(target);
            var buildingName   = prefab.GetBuildingName(target);
            var buildingNameUC = buildingName.ToUpper();
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: buildingNameUC
              , content: defData.Description
              , onDoneClose: null);

            var sellValue = (int)(defData.Cost * DataManager.Ins.GameDef.SellRatio_Building);
            window.utils.SetListener(window.SharedData_Building.SellBtn, () => {
                if (!window.IsCanSell(target, out var blockReason)) {
                    NotificationWindow.Show(
                        title: $"SELL {buildingNameUC}"
                      , content: blockReason);
                    return;
                }

                YesNoWindow.Show(
                    title: $"SELL {buildingNameUC}"
                  , content: $"Are you sure you want to sell your {buildingName} for {sellValue} gold?"
                  , yesCallback: () => {
                        DataManager.Ins.UpdateData_SellBuilding(target);

                        EventDispatcher.PostEvent(EventId.UserGoldChanged);
                        EventDispatcher.PostEvent(EventId.UserBuildingListChanged);
                        EventDispatcher.PostEvent(EventId.HomeMapChanged);
                        
                        FloatingTextPool.Ins.ShowAtCenterScreen(FloatingTextId.GoldChange).SetTextChange(sellValue);

                        window.Close(null);
                    });
            });

            return window;
        }

        protected abstract string GetBuildingName(BuildingInsData target);

        protected virtual bool IsCanSell(BuildingInsData target, out string blockReason) {
            blockReason = default;
            return true;
        }
    }
}