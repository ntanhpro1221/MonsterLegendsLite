using DG.Tweening;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UtilFuncs = NGDtuanh.Utils.UtilFuncs;

namespace MonsterLegendsLite {
    public class Battle_StatBar : MonoBehaviourExt {
        [SerializeField, Required]
        private TextMeshPro infoTxt;
        
        [SerializeField, Required]
        private SpriteRendererAnchorer main, gain, loss;

        [SerializeField]
        private float applyDuration;
        
        [SerializeField]
        private Ease applyEase;

        public void Initialize(int max) {
            SetRatioGainLoss(0, 0);
            SetRatio(main, 1);
            SetInfo(max, max);
        }

        public void SetDelta(int amount, int current, int max) {
            var gainRatio = (float)Mathf.Abs(amount) / max;
            var lossRatio = 0f;

            if (amount < 0) utils.Swap(ref gainRatio, ref lossRatio);

            gainRatio = Mathf.Min(gainRatio, 1 - (float)current / max);
            lossRatio = Mathf.Min(lossRatio, (float)current / max);

            SetRatioGainLoss(gainRatio, lossRatio);
            SetInfo(Mathf.Clamp(current + amount, 0, max), max); // TODO: Use some text like: "-900" to preview damage
        }

        public void ApplyDelta(int amount, int current, int max) {
            SetRatioGainLoss(0, 0);

            this.DOKill();
            DOVirtual
                .Float(
                    current
                  , Mathf.Clamp(current + amount, 0, max)
                  , applyDuration
                  , value => {
                        SetRatio(main, value / max);
                        SetInfo((int)value, max);
                    })
                .SetEase(applyEase)
                .SetTarget(this);
        }

        private void SetInfo(int cur, int max) {
            infoTxt.text = $"{cur}/{max}";
        }

        private void SetRatioGainLoss(float gainRatio, float lossRatio) {
            SetRatio(gain, gainRatio);
            SetRatio(loss, lossRatio);
        }

        private void SetRatio(Component target, float value) {
            target.transform.localScale = utils.With(target.transform.localScale, UtilFuncs.VecAxis.X, value);
            UpdateAllAnchor();
        }

        private void UpdateAllAnchor() {
            gain.UpdatePosFromAnchor();
            loss.UpdatePosFromAnchor();
            main.UpdatePosFromAnchor();
        }

        private void OnDestroy() {
            this.DOKill();
        }
    }
}