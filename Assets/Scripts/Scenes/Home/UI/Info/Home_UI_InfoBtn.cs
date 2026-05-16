using System.Collections;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_InfoBtn : MonoBehaviourExt {
        [SerializeField, Required]
        private CanvasGroup canvasGroup;
        
        [SerializeField, Range(0, 1)]
        private float alphaNonInteractable;
        
        [SerializeField, Required]
        private Button button;

        [SerializeField, Required]
        private Image iconImg;

        [SerializeField, Required]
        private TextMeshProUGUI titleTxt;

        [SerializeField, Required]
        private TextMeshProUGUI infoTxt;

        private UnityAction updateCallback;

        private void OnEnable() {
            if (updateCallback != null) SetUpdate(updateCallback);
        }

        public void SetCallback(UnityAction callback) {
            utils.SetListener(button, callback);

            if (callback == null) button.enabled = false;
        }

        public void SetInteractable(bool interactable) {
            button.interactable = interactable;
            canvasGroup.alpha = interactable ? 1 : alphaNonInteractable;
        }

        public void SetIcon(Sprite icon) {
            iconImg.sprite = icon;
        }

        public void SetTitle(string title) {
            titleTxt.text = title;
        }

        public void SetInfo(string info) {
            infoTxt.text = info;
        }

        public void SetUpdate(UnityAction updateCallback) {
            this.updateCallback = updateCallback;

            if (gameObject.activeInHierarchy) {
                StopCoroutine(nameof(IEUpdate));
                StartCoroutine(nameof(IEUpdate));
            }
        }

        private IEnumerator IEUpdate() {
            while (updateCallback != null) {
                updateCallback.Invoke();
                yield return null;
            }
        }
    }
}