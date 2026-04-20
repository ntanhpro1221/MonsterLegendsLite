using UnityEngine;

namespace NGDtuanh.MonsterLegends {
    public class Singleton<T> : MonoBehaviourExt where T : MonoBehaviour {
        [SerializeField]
        private bool dontDestroyOnLoad;
        
        private static T ins;

        public static T Ins {
            get {
                if (ins != null) return ins;

                ins = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                if (ins != null) return ins;

                ins = new GameObject(typeof(T).Name + " (Singleton)").AddComponent<T>();

                return ins;
            }
        }

        protected virtual void Awake() {
            if (ins != null && ins != this) {
                Destroy(gameObject);
                return;
            }
            
            ins = this as T;
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
    }
}