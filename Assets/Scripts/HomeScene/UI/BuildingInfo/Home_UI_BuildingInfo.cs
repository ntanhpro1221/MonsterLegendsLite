using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_BuildingInfo : MonoBehaviourExt {
        [SerializeField, Required]
        private Home_UI_BuildingInfoSharedData shared;

        public Home_Building CurTarget { get; private set; }

        public virtual void LoadInfoFor(Home_Building building) {
            CurTarget = building;
        }

        public virtual void UnloadInfo() {
            CurTarget = null;
        }
    }
}