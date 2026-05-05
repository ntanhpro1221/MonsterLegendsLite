using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class NewSkillAvailableWindow : PopupWindow {
        [SerializeField, Required]
        private MonsterDetail_UI_SkillDetail skillDetail;
        
        public static NewSkillAvailableWindow Show(NewSkillAvailableWindow prefab, MonsterSkillData data, Sprite element) {
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: "NEW SKILL AVAILABLE"
              , content: null
              , onDoneClose: null);

            window.skillDetail.SetAllData(data, element);

            return window;
        }
    }
}