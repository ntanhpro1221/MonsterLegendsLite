using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using UnityEngine.Events;

namespace MonsterLegendsLite {
    public class Battle_BootData : Singleton<Battle_BootData> {
        public delegate void OnBattleEnd(bool isWin);

        public string exitWarning;
        public UnityAction onExit;
        public OnBattleEnd onBattleEnd;
        private MonsterTeamSlots<MonsterInsData> teamLeft, teamRight;

        public IReadOnlyList<MonsterInsData> TeamLeft => teamLeft;
        public IReadOnlyList<MonsterInsData> TeamRight => teamRight;

        public void SetData(
            string exitWarning
          , UnityAction onExit
          , OnBattleEnd onBattleEnd
          , MonsterTeamSlots<MonsterInsData> teamLeft
          , MonsterTeamSlots<MonsterInsData> teamRight) {
            this.exitWarning = exitWarning;
            this.onExit      = onExit;
            this.onBattleEnd = onBattleEnd;
            this.teamLeft    = new MonsterTeamSlots<MonsterInsData>().WithAll(teamLeft);
            this.teamRight   = new MonsterTeamSlots<MonsterInsData>().WithAll(teamRight);
        }
    }
}