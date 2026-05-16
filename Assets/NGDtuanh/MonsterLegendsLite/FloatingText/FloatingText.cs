using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace NGDtuanh.MonsterLegendsLite {
    [RequireComponent(typeof(RectTransform))
   , RequireComponent(typeof(CanvasGroup))]
    public class FloatingText : MonoBehaviourExt {
        [Serializable]
        private class ShowHidePair<T> {
            public T show;
            public T hide;
        }
        
        [SerializeField, Required]
        private RectTransform floatLayer;

        [SerializeField, Required]
        private TextMeshProUGUI text;

        [SerializeField]
        private ShowHidePair<float> height, scale, duration;

        [SerializeField]
        private ShowHidePair<Ease> heightEase, scaleEase, alphaEase;

        private CanvasGroup canvasGroup;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetText(string content) {
            text.text = content;
        }

        public void SetTextChange(long changeAmount) {
            SetText($"{(changeAmount > 0 ? "+" : "")}{utils.ToStr_Resource(changeAmount)}");
        }

        [Button]
        internal void StartFloat(Action<FloatingText> onCompleted) {
            StartCoroutine(IEFloat(onCompleted));
        }

        private IEnumerator IEFloat(Action<FloatingText> onCompleted) {
            floatLayer.anchoredPosition = Vector2.zero;
            floatLayer.localScale       = Vector3.zero;
            canvasGroup.alpha           = 0;

            floatLayer.DOAnchorPosY(height.show, duration.show).SetEase(heightEase.show);
            floatLayer.DOScale(scale.show, duration.show).SetEase(scaleEase.show);
            canvasGroup.DOFade(1, duration.show).SetEase(alphaEase.show);
            yield return WaitForSecondCache.Get(duration.show);
            
            floatLayer.DOAnchorPosY(height.hide, duration.hide).SetEase(heightEase.hide);
            floatLayer.DOScale(scale.hide, duration.hide).SetEase(scaleEase.hide);
            canvasGroup.DOFade(0, duration.hide).SetEase(alphaEase.hide);
            yield return WaitForSecondCache.Get(duration.hide);

            onCompleted?.Invoke(this);
        }

        internal IEnumerator IEPinToWorldPos(Vector3 worldPos, Canvas canvas, Camera cam) {
            var parent = (RectTransform)RectTF.parent;
            var locPos = default(Vector2);

            while (true) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parent, cam.WorldToScreenPoint(worldPos)
                  , canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera
                  , out locPos);
                RectTF.localPosition = locPos;

                yield return null;
            }
        }

        private void OnDestroy() {
            floatLayer.DOKill();
            canvasGroup.DOKill();
        }
    }
}