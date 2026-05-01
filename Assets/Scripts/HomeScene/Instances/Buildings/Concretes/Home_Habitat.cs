using System;
using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Habitat : Home_Building {
        [NonSerialized, ShowInInspector, ReadOnly, PropertyOrder(-99)]
        public HabitatInsData insData;

        [SerializeField, Required]
        public Transform monsterRoot;

        [SerializeField, Required]
        public SpriteRenderer moveMonsterArrow;

        [SerializeField, Required]
        public SpriteRendererAnchorer moveMonsterArrowAnchorer;
        
        private readonly List<Home_Monster> monsters = new();
        
        [ShowInInspector, PropertyOrder(-98)]
        public IReadOnlyList<Home_Monster> Monsters => monsters;

        public void Initialize(HabitatInsData insData) {
            this.insData = insData;
            
            base.Initialize(insData.InsId);
            
            SetVisibleMoveMonsterArrow(false);
            moveMonsterArrowAnchorer.UpdatePosFromAnchor();
            
            EventDispatcher.RegisterEvent(EventId.HomeMonsterMoved, RebuildMonsterList, this);
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(EventId.HomeMonsterMoved, RebuildMonsterList, this);
        }

        private void RebuildMonsterList() {
            var oldList = new List<Home_Monster>(monsters);
            monsters.Clear();

            foreach (var monster in Home_SceneManager.Ins.Monsters.Values) {
                if (monster.insData.Habitat != insData.InsId) continue;
                
                if (oldList.Contains(monster)) monsters.Add(monster);
                else AddMonster(monster);
            }
        }

        public void AddMonster(Home_Monster monster) {
            monsters.Add(monster);
            
            monster.TF.SetParent(monsterRoot);
            monster.TF.localPosition = Vector3.zero;
            
            monster.StartLocalMove(DataManager.Ins.GameDefData.Habitat[insData.Id].Size);
        }

        public bool IsCanAcceptNewMonster(Home_Monster target) {
            var gameDefData = DataManager.Ins.GameDefData;
            return
                gameDefData.Habitat[insData.Id].Capacity > monsters.Count
             && gameDefData.Monster[target.insData.Id].Elements.Contains(gameDefData.Habitat[insData.Id].Element)
             && !monsters.Contains(target);
        }

        public void SetVisibleMoveMonsterArrow(bool visible) {
            moveMonsterArrow.enabled = visible;
        }

        public long CalculateCurTotalGold() {
            float result = insData.CurGold;
            float minutes = SerTimestamp.DeltaMinutes(SerTimestamp.GetCurTimestamp(), insData.LastGoldUpdate);

            foreach (var monster in Monsters) result += minutes * monster.GetGPM();
            
            return Math.Min(DataManager.Ins.GameDefData.Habitat[insData.Id].MaxGold, (long)(result));
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