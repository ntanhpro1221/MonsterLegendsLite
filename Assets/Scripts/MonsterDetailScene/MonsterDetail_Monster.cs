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
            this.defData = DataManager.Ins.GameDefData.Monster[insData.Id];
        }

        public MonsterStats<int> CalculateStats() {
            return defData.CalculateStats(insData);
        }
    }
}