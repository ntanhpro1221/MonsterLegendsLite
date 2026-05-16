using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_ProgressBar : MonoBehaviourExt {
        [SerializeField, Required]
        private SpriteRendererAnchorer fillSpr;

        [SerializeField, Required]
        private TextMeshPro infoTxt;

        public void SetProgress_Percent(float percent) {
            SetFill(percent);
            infoTxt.text = (int)(percent * 100) + "%";
        }

        public void SetProgress_Time(long total, long elapsed) {
            SetFill((float)((double)elapsed / total));
            infoTxt.text = utils.ToStr_TimeAmount(total - elapsed);
        }

        private void SetFill(float ratio) {
            fillSpr.TF.localScale = utils.With(fillSpr.TF.localScale, UtilFuncs.VecAxis.X, ratio);
            fillSpr.UpdatePosFromAnchor();
        }

        public void Show() {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
        }

        public void Hide() {
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }
    }
}