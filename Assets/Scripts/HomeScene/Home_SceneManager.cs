using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class Home_SceneManager : Singleton<Home_SceneManager> {
        [SerializeField]
        private MonsterDetail_BootData prefabMonsterDetailBootData;
        
        [SerializeField, Required]
        private Home_UI_BuildingInfoManager uiBuildingInfo;
        
        [SerializeField, Required]
        private Transform habitatRoot, farmRoot;

        private readonly Dictionary<string, Home_Monster> monsters = new();
        private readonly Dictionary<string, Home_Habitat> habitats = new();
        private readonly Dictionary<string, Home_Farm> farms = new();
        
        public IReadOnlyDictionary<string, Home_Monster> Monsters => monsters;
        public IReadOnlyDictionary<string, Home_Habitat> Habitats => habitats;
        public IReadOnlyDictionary<string, Home_Farm> Farms => farms;

        private void Start() {
            BuildMap();
        }

        private void BuildMap() {
            var map        = Home_MapManager.Ins;
            var gameLocDef = DataManager.Ins.GameLocDefData;
            var userIns    = DataManager.Ins.UserInsData;

            // Spawn habitats
            foreach (var insData in userIns.Habitats) {
                var ins = Instantiate(gameLocDef.Habitat[insData.Id].PrefabHomeScene, habitatRoot);
                habitats.Add(insData.InsId, ins);

                ins.Initialize(insData);
                ins.TF.position = map.GetWorldPos(insData.Position);
            }

            // Spawn monsters
            foreach (var insData in userIns.Monsters) {
                var habitat = habitats[insData.Habitat];
                var ins = Instantiate(gameLocDef.Monster[insData.Id].PrefabHomeScene);
                monsters.Add(insData.InsId, ins);

                ins.Initialize(insData);
                habitat.AddMonster(ins);
            }
            
            // Spawn farms
            foreach (var insData in userIns.Farms) {
                var ins = Instantiate(gameLocDef.Farm[insData.Id].PrefabHomeScene, farmRoot);
                farms.Add(insData.InsId, ins);

                ins.Initialize(insData);
                ins.TF.position = map.GetWorldPos(insData.Position);
            }
        }

        public void OnClicked_Building(Home_Building building) {
            uiBuildingInfo.ShowInfoFor(building);
        }

        public void OnClicked_Void() {
            uiBuildingInfo.HideCurInfo();
        }

        public void ViewMonsterDetail(string insId) {
            Instantiate(prefabMonsterDetailBootData).SetData(monsters[insId].insData);
            SceneManager.LoadScene("MonsterDetailScene");
        }
    }
}