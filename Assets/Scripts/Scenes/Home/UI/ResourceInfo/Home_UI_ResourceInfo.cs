using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.UI {
    public abstract class Home_UI_ResourceInfo : MonoBehaviourExt {
        [SerializeField]
        private EventId refreshEvent;

        [SerializeField, Required]
        private Home_UI_ResourceInfoData data;

        protected Home_UI_ResourceInfoData Data => data;
        
        private void Awake() {
            EventDispatcher.RegisterEvent(refreshEvent, Refresh, this);
            Initialize();
            Refresh();
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(refreshEvent, Refresh, this);
        }

        protected abstract void Initialize();
        protected abstract void Refresh();
    }
}