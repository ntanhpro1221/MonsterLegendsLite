using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class ReplaceMonsterWindow : PopupWindow {
        [SerializeField, Required]
        private Arena_MonsterBtn sampleItem;

        private readonly Stack<Arena_MonsterBtn> freeBtns = new();
        private readonly List<Arena_MonsterBtn> usedBtns = new();

        protected override void Initialize() {
            base.Initialize();

            sampleItem.gameObject.SetActive(false);
        }

        public static ReplaceMonsterWindow Show(ReplaceMonsterWindow prefab, string title, UnityAction<MonsterInsData> onSelected, params IReadOnlyList<MonsterInsData>[] unavailableMonsters) {
            var userMonsters   = DataManager.Ins.User.Monsters;
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: title
              , content: null
              , onDoneClose: null);

            window.ReleaseAllUsedBtns();

            for (int i = 0; i < userMonsters.Count; ++i) {
                var monster = userMonsters[i];

                var item = window.GetBtn();

                item.SetAllData(monster);
                item.SetButton(
                    interactable: !unavailableMonsters.Any(blockList => blockList.Any(blockItem => blockItem == monster))
                  , callback: () => window.Close(() => onSelected?.Invoke(monster)));
            }

            window.RebuildItemsLayout();

            return window;
        }

        private void ReleaseAllUsedBtns() {
            foreach (var usedBtn in usedBtns) {
                usedBtn.gameObject.SetActive(false);
                freeBtns.Push(usedBtn);
            }

            usedBtns.Clear();
        }

        private Arena_MonsterBtn GetBtn() {
            if (!freeBtns.TryPop(out var result)) result = Instantiate(sampleItem, sampleItem.RectTF.parent);

            result.gameObject.SetActive(true);
            result.RectTF.SetAsLastSibling();
            usedBtns.Add(result);
            return result;
        }

        private void RebuildItemsLayout() {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)sampleItem.RectTF.parent);
        }
    }
}