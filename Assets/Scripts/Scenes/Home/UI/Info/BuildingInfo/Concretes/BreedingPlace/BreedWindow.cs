using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class BreedWindow : PopupWindow {
        [SerializeField, Required]
        private ReplaceMonsterWindow prefabReplaceWindow;

        [SerializeField, Required]
        private Button startBreedBtn;

        [SerializeField, Required]
        private GameObject selectMonstersNotify;

        [SerializeField, Required]
        private Arena_MonsterSlot monsterFirst, monsterSecond;

        public static BreedWindow Show(BreedWindow prefab, Home_BreedingPlace place) {
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: "BREED"
              , content: null
              , onDoneClose: null);

            window.SetAllData(place);

            return window;
        }

        private void SetAllData(Home_BreedingPlace place) {
            startBreedBtn.gameObject.SetActive(false);
            selectMonstersNotify.SetActive(true);
            
            utils.SetListener(startBreedBtn, () => {
                DataManager.Ins.UpdateData_StartBreed(place.InsData, new(
                    monsterFirst.InsData.Id
                  , monsterSecond.InsData.Id));
                
                place.ChangeState(Home_BreedingPlace.State.Breeding);
                
                Close(null);
            });

            SetupSlot(monsterFirst);
            SetupSlot(monsterSecond);
        }

        private void SetupSlot(Arena_MonsterSlot slot) {
            slot.SetAllData(null);
            slot.SetReplaceButton(() => ReplaceMonsterWindow.Show(
                prefab: prefabReplaceWindow
              , title: "SELECT MONSTER"
              , onSelected: selectedMonster => {
                    slot.SetAllData(selectedMonster);

                    if (monsterFirst.InsData != null
                     && monsterSecond.InsData != null) {
                        startBreedBtn.gameObject.SetActive(true);
                        selectMonstersNotify.SetActive(false);
                    }
                }
              , new[] {
                    monsterFirst.InsData
                  , monsterSecond.InsData
                }
            ));
        }
    }
}