using System;
using NGDtuanh.Types;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class GameLocDefData {
        public EnumMap<MonsterId, MonsterLocDefData> Monster;
        public EnumMap<HabitatId, HabitatLocDefData> Habitat;
        public EnumMap<FarmId, FarmLocDefData> Farm;
    }
}