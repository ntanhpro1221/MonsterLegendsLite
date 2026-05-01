using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class Home_SceneManager : SceneSingleton<Home_SceneManager> {
        [SerializeField, Required]
        private Home_UI_InfoManager uiInfo;

        [SerializeField, Required]
        private Transform buildingRoot;

        private readonly Dictionary<string, Home_Monster> monsters = new();
        private readonly Dictionary<string, Home_Habitat> habitats = new();
        private readonly Dictionary<string, Home_Farm> farms = new();

        public IReadOnlyDictionary<string, Home_Monster> Monsters => monsters;
        public IReadOnlyDictionary<string, Home_Habitat> Habitats => habitats;
        public IReadOnlyDictionary<string, Home_Farm> Farms => farms;

        private Camera cam;
        public Camera Cam => cam != null ? cam : cam = Camera.main;

        private Home_Monster movingMonster;

        protected override void Initialize() {
            base.Initialize();
            
            uiInfo.Initialize();
            
            BuildMap();
            
            LoadBootDataThenDelete();
            
            EventDispatcher.RegisterEvent(EventId.UserMonsterListChanged, RebuildHabitats, this, 100);
            EventDispatcher.RegisterEvent(EventId.UserHabitatListChanged, RebuildMonsters, this, 100);
            EventDispatcher.RegisterEvent(EventId.UserFarmListChanged, RebuildFarms, this, 100);
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(EventId.UserMonsterListChanged, RebuildHabitats, this);
            EventDispatcher.UnregisterEvent(EventId.UserHabitatListChanged, RebuildMonsters, this);
            EventDispatcher.UnregisterEvent(EventId.UserFarmListChanged, RebuildFarms, this);
        }

        private void LoadBootDataThenDelete() {
            var bootData = Home_BootData.Ins;
            if (bootData == null) return;

            if (bootData.moveMonster != null) StartMoveMonster(Monsters[bootData.moveMonster.InsId]);
            
            if (bootData.floatingGold != null) FloatingTextPool.Ins.ShowAtCenterScreen(FloatingTextId.GoldChange).SetTextChange(bootData.floatingGold.Value);
            
            Destroy(bootData.gameObject);
        }

        private void BuildMap() {
            RebuildHabitats();
            RebuildFarms();
            
            RebuildMonsters();
        }

        private void RebuildHabitats() {
            var map        = Home_MapManager.Ins;
            var gameLocDef = DataManager.Ins.GameLocDefData;
            
            // Remove
            foreach (var (key, _) in habitats.Where(static i =>
                i.Value == null
             || !DataManager.Ins.UserInsData.Habitats.Contains(i.Value.insData)))
                habitats.Remove(key);

            // Add
            foreach (var habitat in DataManager.Ins.UserInsData.Habitats) {
                if (habitats.ContainsKey(habitat.InsId)) continue;
                
                var ins = Instantiate(gameLocDef.Habitat[habitat.Id].PrefabHomeScene, buildingRoot);
                habitats.Add(habitat.InsId, ins);

                ins.Initialize(habitat);
                ins.TF.position = map.GetWorldPos(habitat.Position);
            }
        }
        
        private void RebuildFarms() {
            var map        = Home_MapManager.Ins;
            var gameLocDef = DataManager.Ins.GameLocDefData;
            
            // Remove
            foreach (var (key, _) in farms.Where(static i =>
                i.Value == null
             || !DataManager.Ins.UserInsData.Farms.Contains(i.Value.insData)))
                farms.Remove(key);

            // Add
            foreach (var farm in DataManager.Ins.UserInsData.Farms) {
                if (farms.ContainsKey(farm.InsId)) continue;
                
                var ins = Instantiate(gameLocDef.Farm[farm.Id].PrefabHomeScene, buildingRoot);
                farms.Add(farm.InsId, ins);

                ins.Initialize(farm);
                ins.TF.position = map.GetWorldPos(farm.Position);
            }
        }
        
        private void RebuildMonsters() {
            var gameLocDef = DataManager.Ins.GameLocDefData;
            
            // Remove
            foreach (var (key, _) in monsters.Where(static i =>
                i.Value == null
             || !DataManager.Ins.UserInsData.Monsters.Contains(i.Value.insData)))
                monsters.Remove(key);

            // Add
            foreach (var monster in DataManager.Ins.UserInsData.Monsters) {
                if (monsters.ContainsKey(monster.InsId)) continue;
                
                var habitat = habitats[monster.Habitat];
                var ins     = Instantiate(gameLocDef.Monster[monster.Id].PrefabHomeScene);
                monsters.Add(monster.InsId, ins);

                ins.Initialize(monster);
                habitat.AddMonster(ins);
            }
        }

        public void OnClicked_Building(Home_Building building) {
            if (movingMonster != null) {
                if (building is Home_Habitat habitat
                 && habitat.IsCanAcceptNewMonster(movingMonster))
                    ConfirmMoveMonster(habitat);
            } else uiInfo.ShowBuildingInfoFor(building, hideAllCurrentInfo: false);
        }

        public void OnMove_Building(Home_Building building) {
            uiInfo.ShowMoveBuildingInfoFor(building);
        }

        public void OnClicked_Void() {
            uiInfo.TryHideCurBuildingInfo();
        }

        public void ForceShowBuildingInfo(Home_Building building) {
            uiInfo.ShowBuildingInfoFor(building, hideAllCurrentInfo: true);
        }

        private void StartMoveMonster(Home_Monster target) {
            movingMonster = target;
            
            foreach (var habitat in habitats.Values) habitat.SetVisibleMoveMonsterArrow(habitat.IsCanAcceptNewMonster(target));
            uiInfo.ShowMoveMonsterInfo(target);
        }

        public void ConfirmMoveMonster(Home_Habitat toHabitat) {
            if (toHabitat == null) uiInfo.TryHideMoveMonsterInfo();
            else {
                DataManager.Ins.UpdateData_MoveMonster(movingMonster.insData, toHabitat.insData);
                
                ForceShowBuildingInfo(toHabitat);
                
                EventDispatcher.PostEvent(EventId.HomeMonsterPlaceChanged);
            }
            
            foreach (var habitat in habitats.Values) habitat.SetVisibleMoveMonsterArrow(false);
            movingMonster = null;
        }

        public void ViewMonsterDetail(string insId) {
            MonsterDetail_BootData.InsAutoSpawn.SetData(monsters[insId].insData);
            SceneManager.LoadScene("MonsterDetailScene");
        }

        public IEnumerable<Home_Building> IEBuildings() {
            foreach (var item in habitats.Values) yield return item;
            foreach (var item in farms.Values) yield return item;
        }
    }
}