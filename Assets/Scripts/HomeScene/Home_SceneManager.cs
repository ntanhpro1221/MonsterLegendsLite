using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class Home_SceneManager : SceneSingleton<Home_SceneManager> {
        [SerializeField]
        private MonsterDetail_BootData prefabMonsterDetailBootData;

        [SerializeField, Required]
        private Home_UI_BuildingInfoManager uiBuildingInfo;

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

        protected override void Initialize() {
            base.Initialize();
            BuildMap();
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
            uiBuildingInfo.ShowInfoFor(building, hideMoveInfo: false);
        }

        public void OnMove_Building(Home_Building building) {
            uiBuildingInfo.ShowMoveInfoFor(building);
        }

        public void OnClicked_Void() {
            uiBuildingInfo.TryHideCurInfo();
        }

        public void ForceShowBuildingInfo(Home_Building building) {
            uiBuildingInfo.ShowInfoFor(building, hideMoveInfo: true);
        }

        public void ViewMonsterDetail(string insId) {
            Instantiate(prefabMonsterDetailBootData).SetData(monsters[insId].insData);
            SceneManager.LoadScene("MonsterDetailScene");
        }

        public IEnumerable<Home_Building> IEBuildings() {
            foreach (var item in habitats.Values) yield return item;
            foreach (var item in farms.Values) yield return item;
        }
    }
}