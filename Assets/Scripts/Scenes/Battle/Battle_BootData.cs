using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.Types;

namespace MonsterLegendsLite {
    public class Battle_BootData : Singleton<Battle_BootData> {
        private List<MonsterInsData> teamLeft, teamRight;

        public IReadOnlyList<MonsterInsData> TeamLeft => teamLeft;
        public IReadOnlyList<MonsterInsData> TeamRight => teamRight;

        public void SetData(List<MonsterInsData> teamLeft, List<MonsterInsData> teamRight) {
            this.teamLeft  = new(teamLeft);
            this.teamRight = new(teamRight);
        }
    }
}