using NGDtuanh.MonsterLegendsLite;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class BuildingInfoWindowSharedData : MonoBehaviourExt {
        [field: SerializeField] // Not required, maybe there are some buildings that cannot be sell
        public Button SellBtn { get; private set; }
    }
}