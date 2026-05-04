using System;
using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    [Serializable]
    public class Battle_Skill {
        public readonly MonsterSkillData skillData;
        public readonly ElementLocData elementLocData;
        
        [ShowInInspector, ReadOnly]
        public int Cooldown { get; private set; }

        public Battle_Skill(MonsterSkillData skillData, ElementLocData elementLocData) {
            this.skillData      = skillData;
            this.elementLocData = elementLocData;
        }

        public void DecreaseCooldown() {
            Cooldown = Math.Max(0, Cooldown - 1);
        }

        public void StartCooldown() {
            Cooldown = skillData.Cooldown;
        }
    }
}