using System;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BuildingInsData {
        public string InsId;
        public Vector2Int Position;

        public T To<T>() where T : BuildingInsData {
            return (T)this;
        }
    }
}