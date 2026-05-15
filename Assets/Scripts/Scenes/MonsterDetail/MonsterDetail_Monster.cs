using System;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class MonsterDetail_Monster : MonoBehaviourExt {
        [NonSerialized, ShowInInspector, ReadOnly]
        public MonsterInsData insData;

        [NonSerialized, ShowInInspector, ReadOnly]
        public MonsterDefData defData;

        [SerializeField, Required]
        private MonsterModel model;

        public void Initialize(MonsterInsData insData) {
            this.insData = insData;
            this.defData = DataManager.Ins.GameDef.Monsters[insData.Id];
            
            EventDispatcher.RegisterEvent(EventId.MonsterLevelChangedInMonsterDetail, PlayLevelUpEffect, this);
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(EventId.MonsterLevelChangedInMonsterDetail, PlayLevelUpEffect, this);
        }

        private void PlayLevelUpEffect() {
            model.Play(MonsterAnimId.Attack); // we dont have a celebrate animation, so sad :(
        }

        public MonsterStats<int> CalculateStats() {
            return defData.CalculateStats(insData);
        }

        public int CalculateStat(MonsterStatId id) {
            return defData.CalculateStat(insData, id);
        }
    }
}