using NGDtuanh.Utils;
using UnityEngine;

namespace NGDtuanh.Types {
    public class Singleton<T> : MonoBehaviourExt where T : Singleton<T> {
        private static T ins;

        public static T Ins {
            get {
                if (ins == null) {
                    ins = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                    if (ins == null) {
                        ins = new GameObject($"{typeof(T).Name} (Singleton)").AddComponent<T>();
                        Debug.LogWarning($"Creating new singleton {typeof(T).Name}. This is unexpected and not a best practice.");
                    }

                    ins.TryInitialize(); // Called even when creating a new GameObject, because a child class may hide the Awake method.
                }

                return ins;
            }
        }

        private bool isInitialized;

        private protected virtual bool IsDontDestroyOnLoad => true;

        /// <summary>
        /// This method is not virtual. Override <see cref="Initialize"/> instead.
        /// </summary>
        private void Awake() {
            TryInitialize();
        }

        private void TryInitialize() {
            if (isInitialized) return;
            isInitialized = true;

            if (ins != null && ins != this) {
                Destroy(gameObject);
                return;
            }

            ins = (T)this;

            if (IsDontDestroyOnLoad) DontDestroyOnLoad(gameObject);

            Initialize();
        }

        protected virtual void Initialize() { }
    }
}