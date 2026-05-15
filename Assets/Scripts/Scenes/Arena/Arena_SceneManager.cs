using System;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class Arena_SceneManager : SceneSingleton<Arena_SceneManager> {
        [SerializeField, Required]
        private Arena_Battle battle;
        
        [SerializeField, Required]
        private Arena_Leaderboard leaderboard;
        
        protected override void Initialize() {
            base.Initialize();
            
            battle.Initialize();
            leaderboard.Initialize();
            
            EventDispatcher.RegisterEvent(EventId.AnyUserEloChanged, leaderboard.TriggerRebuild, this);
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(EventId.AnyUserEloChanged, leaderboard.TriggerRebuild, this);
        }

        private void Start() {
            LoadBootDataThenDelete();
        }

        private void LoadBootDataThenDelete() {
            var bootData = Arena_BootData.Ins;
            if (bootData == null) return;

            if (bootData.endBattleDeltaElo != null) OnEndBattleDeltaElo(bootData.endBattleDeltaElo.Value);

            Destroy(bootData.gameObject);

            void OnEndBattleDeltaElo(int deltaElo) {
                NotificationWindow.Show(
                    title: "ELO CHANGED"
                  , content: $"You {(deltaElo > 0 ? "earned" : "dropped")} {Math.Abs(deltaElo)} elo!");

                FloatingTextPool.Ins.ShowAtCenterScreen(FloatingTextId.EloChanged).SetTextChange(deltaElo);
            }
        }

        public void BackToHomeScene() {
            SceneManager.LoadScene("HomeScene");
        }
    }
}