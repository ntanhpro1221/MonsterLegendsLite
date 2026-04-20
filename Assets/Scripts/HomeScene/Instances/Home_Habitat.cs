using System;
using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegends;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Habitat : MonoBehaviourExt {
        [NonSerialized, ShowInInspector, ReadOnly]
        public HabitatInsData data;

        [SerializeField, Required]
        public Transform monsterRoot;
        
        private readonly List<Home_Monster> monsters = new();

        public void Initialize(HabitatInsData data) {
            this.data = data;
        }

        public void AddMonster(Home_Monster monster) {
            monsters.Add(monster);
            
            monster.TF.SetParent(monsterRoot);
            monster.TF.localPosition = Vector3.zero;
            
            monster.StartLocalMove(data.Position, DataManager.Ins.GameDefData.Habitat[data.Id].Size);
        }
    }
}