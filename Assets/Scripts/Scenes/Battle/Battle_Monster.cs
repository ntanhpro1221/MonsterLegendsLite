using System;
using System.Collections;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Battle_Monster : MonoBehaviourExt {
        [NonSerialized, ShowInInspector, ReadOnly]
        public MonsterInsData insData;

        [SerializeField, Required]
        private MonsterModel model;

        private MonsterStats<int> orgStats, finalStats;

        public int CurHP { get; private set; }
        public int CurMP { get; private set; }

        public void Initialize(MonsterInsData insData, HorDirection faceDir) {
            this.insData = insData;

            orgStats = DataManager.Ins.GameDefData.Monster[insData.Id].CalculateStats(insData);

            UpdateFinalStats();

            CurHP = finalStats[MonsterStatId.HP];
            CurMP = finalStats[MonsterStatId.MP];
            
            model.SetDirection(faceDir);
        }

        public int GetStat(MonsterStatId statId) {
            return finalStats[statId];
        }

        // TODO: Add effects in the future
        private void UpdateFinalStats() {
            finalStats ??= new();
            foreach (var key in finalStats.Keys) finalStats[key] = orgStats[key];
        }

        public void ApplySkill(MonsterSkillData skill, Battle_Monster target, Action onCompleted) {
            StartCoroutine(IEApplySkill());

            IEnumerator IEApplySkill() {
                onCompleted?.Invoke();
                yield break;
            }
        }
    }
}