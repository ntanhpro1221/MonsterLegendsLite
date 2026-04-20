using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegends;

namespace MonsterLegendsLite {
    public class Battle_BootData : Singleton<Battle_BootData> {
        public List<MonsterInsData> teamLeft, teamRight;
    }
}