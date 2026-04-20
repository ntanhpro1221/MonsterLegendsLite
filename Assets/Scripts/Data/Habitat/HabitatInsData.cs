using System;
using NGDtuanh.Types;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class HabitatInsData {
        public string InsId;
        public HabitatId Id;
        public Vector2Int Position;
        public int CurGold;
        public SerTimestamp LastGoldUpdate;
    }
}