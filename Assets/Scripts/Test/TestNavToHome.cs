using NGDtuanh.MonsterLegendsLite;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class TestNavToHome : MonoBehaviourExt {
        private void Awake() {
            if (FindAnyObjectByType<TestHomeSingleton>(FindObjectsInactive.Include) == null) {
                SceneManager.LoadScene("HomeScene");
            } 
        }
    }
}