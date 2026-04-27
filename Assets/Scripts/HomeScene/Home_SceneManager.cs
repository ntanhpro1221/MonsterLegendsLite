using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_SceneManager : Singleton<Home_SceneManager> {
        [SerializeField, Required]
        private Home_UI_BuildingInfoManager uiBuildingInfo;
        
        [SerializeField, Required]
        private Transform habitatRoot, farmRoot;

        private readonly Dictionary<string, Home_Monster> monster = new();
        private readonly Dictionary<string, Home_Habitat> habitats = new();
        private readonly Dictionary<string, Home_Farm> farms = new();
        
        public IReadOnlyDictionary<string, Home_Habitat> Habitats => habitats;

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
                monster.Add(insData.InsId, ins);

                ins.Initialize(insData.InsId);
                habitat.AddMonster(ins);
            }
            
            // Spawn farms
            foreach (var insData in userIns.Farms) {
                var ins = Instantiate(gameLocDef.Farm[insData.Id].PrefabHomeScene, farmRoot);
                farms.Add(insData.InsId, ins);

                ins.Initialize(insData.InsId);
                ins.TF.position = map.GetWorldPos(insData.Position);
            }
        }

        public void OnClicked_Building(Home_Building building) {
            uiBuildingInfo.ShowInfoFor(building);
        }

        public void OnClicked_Void() {
            uiBuildingInfo.HideCurInfo();
        }
    }
}