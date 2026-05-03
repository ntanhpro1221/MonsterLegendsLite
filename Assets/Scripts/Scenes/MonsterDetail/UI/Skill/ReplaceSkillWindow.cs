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

        public static ReplaceSkillWindow Show(ReplaceSkillWindow prefab, int slotId, MonsterInsData insData) {
            var defData        = DataManager.Ins.GameDefData.Monster[insData.Id];
            var elementLocData = DataManager.Ins.GameLocDefData.Element;
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: (insData.GetSkill(slotId, defData) == null ? "SET" : "REPLACE") + " SKILL"
              , content: null
              , onClose: null);

            var itemRoot = (RectTransform)window.sampleItem.RectTF.parent;

            for (int i = 0; i < defData.Skills.Count; ++i) {
                var skill = defData.Skills[i];
                if (skill.UnlockAtLevel > insData.Level) continue;

                var cachedSkillId = i;
                var item          = Instantiate(window.sampleItem, itemRoot);

                item.SetAllData(skill, elementLocData[skill.Element].ElementButton);
                item.SetButton(
                    interactable: !insData.SkillList.Contains(i)
                  , callback: () => {
                        window.Close();

                        DataManager.Ins.UpdateData_MonsterSkill(insData, slotId, cachedSkillId);

                        EventDispatcher.PostEvent(EventId.MonsterSkillListChanged);
                    });
            }

            window.sampleItem.gameObject.SetActive(false);

            LayoutRebuilder.ForceRebuildLayoutImmediate(itemRoot);

            return window;
        }
    }
}