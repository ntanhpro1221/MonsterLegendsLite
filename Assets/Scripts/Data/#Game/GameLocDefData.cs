using System;
using NGDtuanh.Types;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class GameLocDefData {
        public EnumMap<ElementId, ElementLocData> Elements;
        public EnumMap<MonsterRankId, MonsterRankLocData> MonsterRanks;
        public EnumMap<MonsterId, MonsterLocDefData> Monsters;
        public EnumMap<ElementId, HabitatLocDefData> Habitats;
        public EnumMap<FarmId, FarmLocDefData> Farms;
    }
}