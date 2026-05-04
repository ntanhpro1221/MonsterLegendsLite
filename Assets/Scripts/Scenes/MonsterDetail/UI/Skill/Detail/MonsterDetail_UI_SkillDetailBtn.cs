using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class MonsterDetail_UI_SkillDetailBtn : MonsterDetail_UI_SkillDetail {
        [SerializeField, Required, Header("----BUTTON-------")]
        private Button button;

        [SerializeField, Required]
        private CanvasGroup canvasGroup;

        [SerializeField, Required]
        private float alphaNonInteractable;

        public void SetButton(bool interactable, UnityAction callback) {
            canvasGroup.alpha = interactable ? 1 : alphaNonInteractable;
            button.enabled = interactable;
            
            utils.SetListener(button, callback);
        }
    }
}