using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class MonsterDetail_UI_SkillDetail : MonoBehaviourExt {
        [field: SerializeField, Required]
        protected MonsterDetail_UI_SkillDetailSharedData SharedData { get; private set; }

        public void SetAllData(MonsterSkillData skill, Sprite element) {
            SetElement(element);
            SetName(skill.Name);
            SetDescription(skill.Description);
            SetTarget(skill.Target);
            SetPowerRate(skill.PowerRate);
            SetAccuracy(skill.Accuracy);
            SetMPCost(skill.MPCost);
            SetCooldown(skill.Cooldown);
        }

        public void SetElement(Sprite element) {
            SharedData.ElementImg.sprite = element;
        }

        public void SetName(string name) {
            SharedData.NameTxt.text = name;
        }

        public void SetDescription(string description) {
            SharedData.DescriptionTxt.text = description;
        }

        public void SetTarget(MonsterSkillTargetId target) {
            SharedData.TargetTxt.text = DataManager.Ins.GameDef.MonsterSkillTargets[target].Name;

            SharedData.PowerIconEnemy.gameObject.SetActive(target.IsTargetEnemy());
            SharedData.PowerIconAlly.gameObject.SetActive(!target.IsTargetEnemy());
        }

        public void SetPowerRate(int powerRate) {
            SharedData.PowerRate.Rate = powerRate / 100f;
        }

        public void SetAccuracy(int accuracy) {
            SharedData.Accuracy.Rate = accuracy / 100f;
        }

        public void SetMPCost(int mpCost) {
            SharedData.MPCostTxt.text = mpCost.ToString();
        }

        public void SetCooldown(int cooldown) {
            SharedData.CooldownTxt.text = cooldown.ToString();
        }
    }
}