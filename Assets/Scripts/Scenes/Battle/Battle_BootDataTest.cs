using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class Battle_BootDataTest : Battle_BootData {
        [SerializeField]
        private List<string> teamLeftInsId, teamRightInsId;

        protected override void Initialize() {
            base.Initialize();

            SetData(
                exitWarning: "Are you sure you want to exit this battle?"
              , onExit: NavToHomeScene
              , onBattleEnd: isWin => NotificationWindow.Show(
                    title: isWin ? "WIN" : "LOSE"
                  , content: isWin ? "You are win" : "You are lose"
                  , onDoneClose: NavToHomeScene)
              , teamLeft: DataManager.Ins.User.GetTeamIns(teamLeftInsId)
              , teamRight: DataManager.Ins.User.GetTeamIns(teamRightInsId));
        }

        private void NavToHomeScene() {
            SceneManager.LoadScene("HomeScene");
        }
    }
}