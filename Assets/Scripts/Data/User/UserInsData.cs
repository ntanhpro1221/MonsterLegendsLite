using System;
using System.Collections.Generic;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class UserInsData {
        public string Name;
        public int Level;
        public int Exp;
        public int Elo;
        public long Gold;
        public long Food;
        public List<MonsterInsData> Monsters;
        public List<HabitatInsData> Habitats;
        public List<FarmInsData> Farms;
        public MonsterTeamSlots<string> ArenaTeamAttack;
        public MonsterTeamSlots<string> ArenaTeamDefense;
        public MonsterTeamSlots<string> AdventureTeam;
    }
}