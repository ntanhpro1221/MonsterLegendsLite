using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class ReplaceSkillWindow : PopupWindow {
        [SerializeField, Required]
        private MonsterDetail_UI_SkillDetailBtn sampleItem;

        private readonly Stack<MonsterDetail_UI_SkillDetailBtn> freeBtns = new();
        private readonly List<MonsterDetail_UI_SkillDetailBtn> usedBtns = new();

        protected override void Initialize() {
            base.Initialize();

            sampleItem.gameObject.SetActive(false);
        }

        public static ReplaceSkillWindow Show(ReplaceSkillWindow prefab, int slotId, MonsterInsData insData) {
            var defData        = DataManager.Ins.GameDefData.Monster[insData.Id];
            var elementLocData = DataManager.Ins.GameLocDefData.Element;
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: (insData.GetSkill(slotId, defData) == null ? "SET" : "REPLACE") + " SKILL"
              , content: null
              , onDoneClose: null);

            window.ReleaseAllUsedBtns();

            for (int i = 0; i < defData.Skills.Count; ++i) {
                var skill = defData.Skills[i];
                if (skill.UnlockAtLevel > insData.Level) continue;

                var cachedSkillId = i;
                var item          = window.GetBtn();

                item.SetAllData(skill, elementLocData[skill.Element].ElementButton);
                item.SetButton(
                    interactable: !insData.SkillList.Contains(i)
                  , callback: () => {
                        window.Close(null);

                        DataManager.Ins.UpdateData_MonsterSkill(insData, slotId, cachedSkillId);

                        EventDispatcher.PostEvent(EventId.MonsterSkillListChanged);
                    });
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

        private MonsterDetail_UI_SkillDetailBtn GetBtn() {
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