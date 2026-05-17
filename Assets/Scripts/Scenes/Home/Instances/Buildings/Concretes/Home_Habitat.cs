using System;
using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Habitat : Home_Building<HabitatInsData> {
        [ShowInInspector, PropertyOrder(-99)]
        public IReadOnlyList<Home_Monster> Monsters => monsters;
        
        [SerializeField, Required]
        public Transform monsterRoot;

        [SerializeField, Required]
        public SpriteRenderer moveMonsterArrow;

        [SerializeField, Required]
        public SpriteRendererAnchorer moveMonsterArrowAnchorer;
        
        private readonly List<Home_Monster> monsters = new();

        protected override void Initialize(HabitatInsData insData, bool isBuySample) {
            base.Initialize(insData, isBuySample);
            
            SetVisibleMoveMonsterArrow(false);
            moveMonsterArrowAnchorer.UpdatePosFromAnchor();

            if (isBuySample) { } else {
                EventDispatcher.RegisterEvent(EventId.HomeMonsterPlaceChanged, RebuildMonsterList, this);
                EventDispatcher.RegisterEvent(EventId.UserMonsterListChanged, RebuildMonsterList, this);
            }
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            
            EventDispatcher.UnregisterEvent(EventId.HomeMonsterPlaceChanged, RebuildMonsterList, this);
            EventDispatcher.UnregisterEvent(EventId.UserMonsterListChanged, RebuildMonsterList, this);
        }

        private void RebuildMonsterList() {
            var oldList = new List<Home_Monster>(monsters);
            monsters.Clear();

            foreach (var monster in Home_SceneManager.Ins.Monsters.Values) {
                if (monster.InsData.Habitat != InsData.InsId) continue;
                
                if (oldList.Contains(monster)) monsters.Add(monster);
                else AddMonster(monster);
            }
        }

        public void AddMonster(Home_Monster monster) {
            monsters.Add(monster);
            
            monster.TF.SetParent(monsterRoot);
            monster.TF.localPosition = Vector3.zero;
            
            monster.StartLocalMove(DataManager.Ins.GameDef.Habitats[InsData.Id].Size);
        }

        public bool IsCanAcceptNewMonster(Home_Monster target) {
            var gameDefData = DataManager.Ins.GameDef;
            return
                gameDefData.Habitats[InsData.Id].Capacity > monsters.Count
             && gameDefData.Monsters[target.InsData.Id].Elements.Contains(gameDefData.Habitats[InsData.Id].Element)
             && !monsters.Contains(target);
        }

        public void SetVisibleMoveMonsterArrow(bool visible) {
            moveMonsterArrow.enabled = visible;
        }

        public long CalculateCurTotalGold() {
            float result = InsData.CurGold;
            float minutes = SerTimestamp.DeltaMinutes(SerTimestamp.Now(), InsData.LastGoldUpdate);

            foreach (var monster in Monsters) result += minutes * monster.GetGPM();
            
            return Math.Min(DataManager.Ins.GameDef.Habitats[InsData.Id].MaxGold, (long)(result));
        }

        protected override void UpdateData_BuyBuilding(Vector2Int pos, out int cost, out string insId) {
            DataManager.Ins.UpdateData_BuyHabitat(InsData.Id, pos, out cost, out insId);
        }

        protected override Home_Building GetBuildingFromInsId(string insId) {
            return Home_SceneManager.Ins.Habitats[insId];
        }

        protected override bool IsShouldCollectBtnActive(out Sprite sprite) {
            sprite = null;
            
            float minToShowCollectBtn = 
                DataManager.Ins.GameDef.Home_ShowCollectResourceBtnThreshold
              * DataManager.Ins.GameDef.Habitats[InsData.Id].MaxGold;

            return CalculateCurTotalGold() >= minToShowCollectBtn;
        }
        
        protected override void DoClickCollectBtn() {
            DoCollectGold(this);
        }

        public static void DoCollectGold(Home_Habitat habitat) {
            var gold = habitat.CalculateCurTotalGold();
            if (gold <= 0) return;
                
            DataManager.Ins.UpdateData_CollectGold(habitat.InsData);

            FloatingTextPool.Ins.ShowAtWorld(FloatingTextId.GoldChange, habitat.TF.position).SetTextChange(gold);
                
            EventDispatcher.PostEvent(EventId.UserGoldChanged);
        }
    }
}