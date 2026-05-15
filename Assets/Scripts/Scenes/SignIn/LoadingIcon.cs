using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class LoadingIcon : Singleton<LoadingIcon> {
        [SerializeField, Required]
        private Image blocker;

        [SerializeField, Required]
        private RectTransform icon;

        [SerializeField, Required]
        private float rotateSpeed;

        private void Update() {
            icon.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }

        [Button]
        public void Show(bool blockInteract) {
            gameObject.SetActive(true);
            blocker.enabled = blockInteract;
        }

        [Button]
        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}