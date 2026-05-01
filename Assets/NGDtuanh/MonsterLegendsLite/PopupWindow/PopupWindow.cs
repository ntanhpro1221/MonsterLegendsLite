using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace NGDtuanh.MonsterLegendsLite {
    public class PopupWindow : MonoBehaviourExt {
        internal const float SHOW_HIDE_DURATION = .2f;

        [field: SerializeField, Required]
        public PopupWindowSharedData SharedData { get; private set; }

        private Action<PopupWindow> onClose;

        protected static PopupWindow Show(string title, string content, Action onClose = null) {
            return PopupWindowPool.Ins.Show(PopupWindowId.Notification, title, content, onClose);
        }

        internal PopupWindow Initialize(string title, string content, Action<PopupWindow> onClose) {
            SharedData.canvasGroup.blocksRaycasts = true;

            SharedData.canvasGroup.alpha = 0;
            SharedData.canvasGroup.DOKill();
            SharedData.canvasGroup.DOFade(1, SHOW_HIDE_DURATION);

            SharedData.title.text   = title;
            SharedData.content.text = content;
            this.onClose            = onClose;

            SetCallbackTo(SharedData.closeBtn, null, appendClose: true);

            return this;
        }

        private void Close() {
            SharedData.canvasGroup.blocksRaycasts = false;

            SharedData.canvasGroup.DOKill();
            SharedData.canvasGroup.DOFade(0, SHOW_HIDE_DURATION).OnComplete(() => onClose?.Invoke(this));
        }

        protected void SetCallbackTo(Button btn, Action callback, bool appendClose) {
            if (appendClose) callback += Close;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(new(callback));
        }

        private void OnDestroy() {
            SharedData.canvasGroup.DOKill();
        }
    }
}