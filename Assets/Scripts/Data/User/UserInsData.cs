using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class UserInsData {
        public string Name;
        public int Level = 1;
        public int Exp;
        public int Elo;
        public long Gold = 5000;
        public long Food;
        public int CurAdventureLevel;
        public bool Music = true;
        public bool Sound = true;
        public bool Vibrant = true;
        public List<HabitatInsData> Habitats = new();
        public List<FarmInsData> Farms = new();
        public List<BreedingPlaceInsData> BreedingPlaces = new();
        public List<MonsterInsData> Monsters = new();
        public MonsterTeamSlots<string> ArenaTeamAttack = new();
        public MonsterTeamSlots<string> ArenaTeamDefense = new();
        public MonsterTeamSlots<string> AdventureTeam = new();
        
        [OnInspectorInit]
        private void OnInit() {
            if (Level < 1) Level = 1;
        }

        public MonsterTeamSlots<MonsterInsData> GetArenaTeamAttackData() => GetTeamIns(ArenaTeamAttack);
        public MonsterTeamSlots<MonsterInsData> GetArenaTeamDefenseData() => GetTeamIns(ArenaTeamDefense);
        public MonsterTeamSlots<MonsterInsData> GetAdventureTeamData() => GetTeamIns(AdventureTeam);

        public MonsterTeamSlots<MonsterInsData> GetTeamIns(IEnumerable<string> team) {
            return new MonsterTeamSlots<MonsterInsData>().WithAll(team.Select(id => Monsters.FirstOrDefault(monster => monster.InsId == id)));
        }

        public UserInsDataWithId WithId(string id) => new(id, this);
    }
}