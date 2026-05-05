using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NGDtuanh.MonsterLegendsLite {
    public class PopupWindow : MonoBehaviourExt {
        internal const float SHOW_HIDE_DURATION = .2f;

        [field: SerializeField, Required]
        public PopupWindowSharedData SharedData { get; private set; }

        private UnityAction<PopupWindow> onDoneClose;

        protected internal virtual void Initialize() {
            utils.SetListener(SharedData.closeBtn, () => Close(null));
        }

        internal PopupWindow Show(
            string title
          , string content
          , UnityAction<PopupWindow> onDoneClose) {
            gameObject.SetActive(true);
            RectTF.SetAsLastSibling();

            SharedData.canvasGroup.blocksRaycasts = true;

            SharedData.canvasGroup.alpha = 0;
            SharedData.canvasGroup.DOKill();
            SharedData.canvasGroup.DOFade(1, SHOW_HIDE_DURATION);

            SharedData.title.text   = title;
            SharedData.content.text = content;
            this.onDoneClose        = onDoneClose;

            return this;
        }

        protected void Close(UnityAction onDoneClose) {
            SharedData.canvasGroup.blocksRaycasts = false;

            SharedData.canvasGroup.DOKill();
            SharedData.canvasGroup.DOFade(0, SHOW_HIDE_DURATION).OnComplete(() => {
                gameObject.SetActive(false);
                this.onDoneClose?.Invoke(this);
                onDoneClose?.Invoke();
            });
        }

        private void OnDestroy() {
            SharedData.canvasGroup.DOKill();
        }

        protected void SetCallbackTo(Button btn, UnityAction callback, bool afterClose) {
            utils.SetListener(btn, afterClose
                ? () => Close(callback)
                : callback);
        }
    }
}