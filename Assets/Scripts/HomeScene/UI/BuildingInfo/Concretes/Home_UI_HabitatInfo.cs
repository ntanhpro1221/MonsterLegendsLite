using System.Collections.Generic;
using MonsterLegendsLite.Data;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_HabitatInfo : Home_UI_BuildingInfo {
        [SerializeField]
        private Home_UI_BuildingInfoBtn prefabMonsterInfoBtn;

        private readonly List<Home_UI_BuildingInfoBtn> availableMonsterBtns = new(4);

        private readonly List<Home_UI_BuildingInfoBtn> usingMonsterBtns = new(4);

        public override void LoadInfoFor(Home_Building building) {
            base.LoadInfoFor(building);

            var habitat = (Home_Habitat)building;
            
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
                
                var monsterDefData = DataManager.Ins.GameDefData.Monster[monster.insData.Id];
                var monsterLocDefData = DataManager.Ins.GameLocDefData.Monster[monster.insData.Id];
                
                button.SetCallback(() => Home_SceneManager.Ins.ViewMonsterDetail(monster.insData.InsId));
                button.SetIcon(monsterLocDefData.Avatar);
                button.SetTitle(monsterDefData.Name);
                button.SetInfo(monster.insData.Level.ToString());
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTF);
        }
    }
}