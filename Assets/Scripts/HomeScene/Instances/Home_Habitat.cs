using System;
using System.Collections.Generic;
using MonsterLegendsLite.Data;
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
    }
}