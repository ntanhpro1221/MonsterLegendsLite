using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Battle_UI_SkillDetail : MonoBehaviourExt {
        [SerializeField, Required]
        private MonsterDetail_UI_SkillDetail skillDetail;
            
        [SerializeField, Required]
        private Button backBtn;

        public void SetAllData(Battle_Skill skill, UnityAction onBack) {
            skillDetail.SetAllData(skill.skillData, skill.elementLocData.ElementButton);
            utils.SetListener(backBtn, onBack);
        }
    }
}