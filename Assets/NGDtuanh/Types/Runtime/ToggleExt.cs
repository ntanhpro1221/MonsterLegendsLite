using NGDtuanh.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace NGDtuanh.Types {
    public class ToggleExt : Toggle {
        [SerializeField]
        internal Graphic targetGraphic_Off, targetGraphic_On;

        protected readonly UtilFuncs utils = new();

        protected override void OnEnable() {
            base.OnEnable();

            onValueChanged.RemoveListener(OnValueChanged);
            onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn) {
            if (utils.IsPlaying(this)) OnValueChanged_Runtime(isOn);
        }

        protected virtual void OnValueChanged_Runtime(bool isOn) {
            UpdateTargetGraphic();
        }
        
        internal void UpdateTargetGraphic() {
            if (targetGraphic_On == null
             || targetGraphic_Off == null)
                return;
            
            var graphicActive   = targetGraphic_On;
            var graphicDeactive = targetGraphic_Off;

            if (!isOn) utils.Swap(ref graphicActive, ref graphicDeactive);

            if (graphicActive.enabled != true) {
                utils.RecordForUndo(graphicActive);
                graphicActive.enabled = true;
                utils.MarkDirty(graphicActive);
            }
                
            if (graphicDeactive.enabled != false) {
                utils.RecordForUndo(graphicDeactive);
                graphicDeactive.enabled = false;
                utils.MarkDirty(graphicDeactive);
            }
                
            if (targetGraphic != graphicActive) {
                utils.RecordForUndo(this);
                targetGraphic = graphicActive;
                utils.MarkDirty(this);
            }
        }

        protected override void Reset() {
            base.Reset();

            utils.RecordForUndo(this);

            group = GetComponentInParent<ToggleGroup>(includeInactive: true);

            utils.MarkDirty(this);
        }
    }
}