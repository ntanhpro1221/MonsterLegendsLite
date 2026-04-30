using System;
using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Habitat : Home_Building {
        [NonSerialized, ShowInInspector, ReadOnly, PropertyOrder(-99)]
        public HabitatInsData insData;

        [SerializeField, Required]
        public Transform monsterRoot;
        
        private readonly List<Home_Monster> monsters = new();
        
        public IReadOnlyList<Home_Monster> Monsters => monsters;

        public void Initialize(HabitatInsData insData) {
            this.insData = insData;
            
            base.Initialize(insData.InsId);
        }

        public void AddMonster(Home_Monster monster) {
            monsters.Add(monster);
            
            monster.TF.SetParent(monsterRoot);
            monster.TF.localPosition = Vector3.zero;
            
            monster.StartLocalMove(DataManager.Ins.GameDefData.Habitat[insData.Id].Size);
        }

        public long CalculateCurTotalGold() {
            float result = insData.CurGold;
            float minutes = SerTimestamp.DeltaMinutes(SerTimestamp.GetCurTimestamp(), insData.LastGoldUpdate);

            foreach (var monster in Monsters) {
                result += minutes * monster.GetGPM();
            }
            
            return (long)(result);
        }

        public override Vector2Int GetSizeData() {
            return DataManager.Ins.GameDefData.Habitat[insData.Id].Size;
        }

        public override Vector2Int GetPosData() {
            return insData.Position;
        }
        
        protected override void SavePos(Vector2Int tilePos) {
            DataManager.Ins.UpdateData_MoveHabitat(insData, tilePos);
        }
    }
}