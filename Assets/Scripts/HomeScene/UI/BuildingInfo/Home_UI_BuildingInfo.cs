using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_BuildingInfo : MonoBehaviourExt {
        [SerializeField, Required]
        private Home_UI_BuildingInfoSharedData shared;

        public string CurTargetId { get; private set; }

        public virtual void LoadInfoFor(Home_Building building) {
            CurTargetId = building.InsId;
        }
    }
}