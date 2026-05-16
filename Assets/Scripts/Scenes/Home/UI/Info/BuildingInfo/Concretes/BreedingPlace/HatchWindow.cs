using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class HatchWindow : PopupWindow {
        [SerializeField, Required]
        private Arena_Monster monsterDetail;

        [SerializeField, Required]
        private Button placeBtn;

        [SerializeField, Required]
        private Button sellBtn;

        public static HatchWindow Show(HatchWindow prefab, Home_BreedingPlace place) {
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: "HATCH MONSTER"
              , content: null
              , onDoneClose: null);

            window.SetAllData(place);

            return window;
        }

        private void SetAllData(Home_BreedingPlace place) {
            var monsterIns = place.HatchSample.InsData;
            var monsterDef = DataManager.Ins.GameDef.Monsters[monsterIns.Id];
            
            monsterDetail.SetAllData(monsterIns);
            
            utils.SetListener(placeBtn, () => {
                if (!DataManager.Ins.IsAnyHabitatCanAcceptNewMonster(monsterIns)) {
                    NotificationWindow.Show(
                        title: "NO VALID HABITAT"
                      , content: $"You dont have any habitat that can accept {monsterDef.GetCustomNameIfPossible(monsterIns)}");
                    return;
                }

                Home_SceneManager.Ins.StartHatchMonster(place.HatchSample, place);

                Close(null);
            });
            
            utils.SetListener(sellBtn, () => {
                var sellValue = (int)(monsterDef.Cost * DataManager.Ins.GameDef.SellRatio_Monster);

                YesNoWindow.Show(
                    title: "SELL MONSTER"
                  , content: $"Are you sure you want to sell {monsterDef.GetCustomNameIfPossible(monsterIns)} for {sellValue} gold?"
                  , yesCallback: () => {
                        DataManager.Ins.UpdateData_SellMonster(monsterIns);

                        EventDispatcher.PostEvent(EventId.UserGoldChanged);
                        EventDispatcher.PostEvent(EventId.UserMonsterListChanged);
                        
                        place.ChangeState(Home_BreedingPlace.State.Normal);

                        Close(null);
                    });
            });
        }
    }
}