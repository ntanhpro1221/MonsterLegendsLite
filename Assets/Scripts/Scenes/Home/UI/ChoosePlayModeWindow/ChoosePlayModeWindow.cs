using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class ChoosePlayModeWindow : PopupWindow {
        [SerializeField, Required]
        private Button arenaBtn, adventureBtn;

        public static ChoosePlayModeWindow Show(
            ChoosePlayModeWindow prefab
          , UnityAction navArenaScene
          , UnityAction navAdventureScene) {
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: "CHOOSE PLAY MODE"
              , content: ""
              , onDoneClose: null);

            window.utils.SetListener(window.arenaBtn, navArenaScene);
            window.utils.SetListener(window.adventureBtn, navAdventureScene);

            return window;
        }
    }
}