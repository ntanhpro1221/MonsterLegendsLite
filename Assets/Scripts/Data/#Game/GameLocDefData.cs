using System;
using NGDtuanh.Types;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class GameLocDefData {
        public EnumMap<ElementId, HabitatLocDefData> Habitats;
        public EnumMap<FarmId, FarmLocDefData> Farms;
        public EnumMap<BreedingPlaceId, BreedingPlaceLocDefData> BreedingPlaces;
        public EnumMap<MonsterId, MonsterLocDefData> Monsters;
        public EnumMap<ElementId, ElementLocData> Elements;
        public EnumMap<MonsterRankId, MonsterRankLocData> MonsterRanks;
    }
}