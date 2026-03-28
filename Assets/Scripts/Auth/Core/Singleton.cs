using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class Singleton<T> : SerializedMonoBehaviour where T : MonoBehaviour {
    private static T m_Instance;

    public static T Instance {
        get {
            if (m_Instance == null) {
                m_Instance = FindAnyObjectByType<T>();
                if (m_Instance != null) typeof(T)
                    .GetMethod("OnTouched", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(m_Instance, null);
            }
            return m_Instance;
        }
    }

    protected virtual void OnTouched() { }

    protected virtual void Awake() => DontDestroyOnLoad(gameObject);
}