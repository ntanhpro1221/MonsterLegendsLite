using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.UI {
    public abstract class Home_UI_ResourceInfo : MonoBehaviourExt {
        [SerializeField]
        private EventId refreshEvent;

        [field: SerializeField, Required]
        protected Home_UI_ResourceInfoData SharedData { get; private set; }
        
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