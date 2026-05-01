using System.Collections.Generic;
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
        }

        private void LoadBootDataThenDelete() {
            var bootData = Home_BootData.Ins;
            if (bootData == null) return;

            if (bootData.moveMonster != null) {
                StartMoveMonster(Monsters[bootData.moveMonster.InsId]);
            }
            
            Destroy(bootData.gameObject);
        }

        private void BuildMap() {
            var map        = Home_MapManager.Ins;
            var gameLocDef = DataManager.Ins.GameLocDefData;
            var userIns    = DataManager.Ins.UserInsData;

            // Spawn habitats
            foreach (var insData in userIns.Habitats) {
                var ins = Instantiate(gameLocDef.Habitat[insData.Id].PrefabHomeScene, buildingRoot);
                habitats.Add(insData.InsId, ins);

                ins.Initialize(insData);
                ins.TF.position = map.GetWorldPos(insData.Position);
            }

            // Spawn monsters
            foreach (var insData in userIns.Monsters) {
                var habitat = habitats[insData.Habitat];
                var ins     = Instantiate(gameLocDef.Monster[insData.Id].PrefabHomeScene);
                monsters.Add(insData.InsId, ins);

                ins.Initialize(insData);
                habitat.AddMonster(ins);
            }

            // Spawn farms
            foreach (var insData in userIns.Farms) {
                var ins = Instantiate(gameLocDef.Farm[insData.Id].PrefabHomeScene, buildingRoot);
                farms.Add(insData.InsId, ins);

                ins.Initialize(insData);
                ins.TF.position = map.GetWorldPos(insData.Position);
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
                
                EventDispatcher.PostEvent(EventId.HomeMonsterMoved);
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