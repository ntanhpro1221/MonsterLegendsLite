using System;
using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Habitat : Home_Building {
        [NonSerialized, ShowInInspector, ReadOnly]
        public HabitatInsData insData;

        [SerializeField, Required]
        public Transform monsterRoot;
        
        private readonly List<Home_Monster> monsters = new();
        
        public IReadOnlyList<Home_Monster> Monsters => monsters;

        public void Initialize(HabitatInsData insData) {
            base.Initialize(insData.InsId);
            
            this.insData = insData;
        }

        public void AddMonster(Home_Monster monster) {
            monsters.Add(monster);
            
            monster.TF.SetParent(monsterRoot);
            monster.TF.localPosition = Vector3.zero;
            
            monster.StartLocalMove(insData.Position, DataManager.Ins.GameDefData.Habitat[insData.Id].Size);
        }

        public long CalculateCurTotalGold() {
            float result = insData.CurGold;
            float minutes = SerTimestamp.DeltaMinutes(SerTimestamp.GetCurTimestamp(), insData.LastGoldUpdate);

            foreach (var monster in Monsters) {
                result += minutes * monster.GetGPM();
            }
            
            return (long)(result);
        }
    }
}