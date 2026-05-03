using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public abstract class Shop_Building : Shop_Item {
        [field: SerializeField, Required]
        protected Shop_BuildingSharedData SharedDataBuilding { get; private set; }
    }
}