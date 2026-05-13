using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class Adventure_SceneManager : SceneSingleton<Adventure_SceneManager> {
        [SerializeField, Required]
        private Adventure_Team team;
        
        [SerializeField, Required]
        private Adventure_Level[] levels;
        
        protected override void Initialize() {
            base.Initialize();
            
            team.Initialize();
            LoadLevels();
        }

        private void LoadLevels() {
            var levelDatas = DataManager.Ins.GameDefData.AdventureLevels;

            for (int i = 0; i < levelDatas.Count; ++i) levels[i].SetAllData(levelDatas[i], i);
        }

        public void BackToHomeScene() {
            SceneManager.LoadScene("HomeScene");
        }
    }
}