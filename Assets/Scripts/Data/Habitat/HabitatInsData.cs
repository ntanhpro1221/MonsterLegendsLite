using System;
using NGDtuanh.Types;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class HabitatInsData {
        public string InsId;
        public ElementId Id;
        public Vector2Int Position;
        public long CurGold;
        public SerTimestamp LastGoldUpdate;
    }
}