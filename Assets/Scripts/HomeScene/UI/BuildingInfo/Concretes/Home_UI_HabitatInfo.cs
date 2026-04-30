using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_HabitatInfo : Home_UI_BuildingInfo {
        [SerializeField]
        private Home_UI_BuildingInfoBtn collectBtn;
        
        [SerializeField]
        private Home_UI_BuildingInfoBtn prefabMonsterInfoBtn;

        private readonly List<Home_UI_BuildingInfoBtn> availableMonsterBtns = new(4);

        private readonly List<Home_UI_BuildingInfoBtn> usingMonsterBtns = new(4);

        public override void LoadInfoFor(Home_Building building) {
            base.LoadInfoFor(building);

            var habitat = (Home_Habitat)building;
            
            LoadCollectBtn(habitat);
            LoadMonsterBtns(habitat);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTF);
        }

        private void LoadCollectBtn(Home_Habitat habitat) {
            UpdateTotalGold();
            
            collectBtn.SetCallback(() => {
                var gold = habitat.CalculateCurTotalGold();
                if (gold <= 0) return;
                
                DataManager.Ins.UpdateData_CollectGold(habitat);

                FloatingTextManager.Ins.ShowAtWorld(FloatingTextId.GoldChange, habitat.TF.position).SetTextChange(gold);
                
                UpdateTotalGold();
                
                EventDispatcher.PostEvent(EventId.UserGoldChanged);
            });
        }

        private void LoadMonsterBtns(Home_Habitat habitat) {
            foreach (var usingMonsterBtn in usingMonsterBtns) {
                availableMonsterBtns.Add(usingMonsterBtn);
                usingMonsterBtn.gameObject.SetActive(false);
            }
            usingMonsterBtns.Clear();

            foreach (var monster in habitat.Monsters) {
                Home_UI_BuildingInfoBtn button;
                if (availableMonsterBtns.Count > 0) {
                    button = availableMonsterBtns[0];
                    availableMonsterBtns.RemoveAt(0);
                    button.gameObject.SetActive(true);
                } else button = Instantiate(prefabMonsterInfoBtn, RectTF); 

                usingMonsterBtns.Add(button);
                
                var monsterDefData    = DataManager.Ins.GameDefData.Monster[monster.insData.Id];
                var monsterLocDefData = DataManager.Ins.GameLocDefData.Monster[monster.insData.Id];
                
                button.SetCallback(() => Home_SceneManager.Ins.ViewMonsterDetail(monster.insData.InsId));
                button.SetIcon(monsterLocDefData.Avatar);
                button.SetTitle(monsterDefData.Name);
                button.SetInfo(utils.ToStrResource(monster.insData.Level));
            }
        }
        
        public void Update() {
            UpdateTotalGold();
        }

        private void UpdateTotalGold() {
            collectBtn.SetInfo(utils.ToStrResource(CurTarget.To<Home_Habitat>().CalculateCurTotalGold()));
        }
    }
}