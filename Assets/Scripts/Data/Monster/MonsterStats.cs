using System;
using NGDtuanh.Types;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterStats<T> : EnumMap<MonsterStatId, T> { }
}