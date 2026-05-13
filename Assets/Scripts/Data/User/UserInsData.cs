using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class UserInsData {
        public string Name;
        public int Level;
        public int Exp;
        public int Elo;
        public long Gold;
        public long Food;
        public int CurAdventureLevel;
        public List<MonsterInsData> Monsters;
        public List<HabitatInsData> Habitats;
        public List<FarmInsData> Farms;
        public MonsterTeamSlots<string> ArenaTeamAttack;
        public MonsterTeamSlots<string> ArenaTeamDefense;
        public MonsterTeamSlots<string> AdventureTeam;

        public MonsterTeamSlots<MonsterInsData> GetArenaTeamAttackData() => GetTeamIns(ArenaTeamAttack);
        public MonsterTeamSlots<MonsterInsData> GetArenaTeamDefenseData() => GetTeamIns(ArenaTeamDefense);
        public MonsterTeamSlots<MonsterInsData> GetAdventureTeamData() => GetTeamIns(AdventureTeam);

        public MonsterTeamSlots<MonsterInsData> GetTeamIns(IEnumerable<string> team) {
            return new MonsterTeamSlots<MonsterInsData>().WithAll(team.Select(id => Monsters.First(monster => monster.InsId == id)));
        }
    }
}