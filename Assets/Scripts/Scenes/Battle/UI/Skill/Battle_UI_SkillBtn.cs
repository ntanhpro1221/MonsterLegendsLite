using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Battle_UI_SkillBtn : MonoBehaviourExt {
        [SerializeField, Required]
        private CanvasGroup canvasGroup;

        [SerializeField, Range(0, 1)]
        private float alphaNotInteractable;
        
        [SerializeField, Required]
        private Image backgroundImg;
        
        [SerializeField, Required]
        private Image glowImg;

        [SerializeField, Required]
        private UI_ImageAspect iconImg;

        [SerializeField, Required]
        private TextMeshProUGUI nameTxt;
        
        [SerializeField, Required]
        private TextMeshProUGUI cooldownTxt;

        [SerializeField, Required]
        private TextMeshProUGUI notEnoughMPTxt;

        [SerializeField, Required]
        private Button button;

        public void SetAllData(Battle_Skill skill, int ownerMP, UnityAction callback) {
            gameObject.SetActive(skill != null);
            if (skill == null) return;
            
            SetBackground(skill.elementLocData.SkillButtonBG); 
            SetIcon(skill.elementLocData.Icon);
            SetName(skill.skillData.Name);
            SetState(skill.Cooldown, ownerMP >= skill.skillData.MPCost);
            SetCallback(callback);
        }

        public void SetBackground(Sprite background) {
            backgroundImg.sprite = background;
        }

        public void SetIcon(Sprite icon) {
            iconImg.SetSprite(icon);
        }

        public void SetName(string name) {
            nameTxt.text = name;
        }

        public void SetState(int cooldown, bool enough) {
            var interactable = cooldown == 0 && enough;

            glowImg.enabled   = interactable;
            button.enabled    = interactable;
            canvasGroup.alpha = interactable ? 1 : alphaNotInteractable;

            cooldownTxt.text = cooldown switch {
                0 => string.Empty
              , 1 => "COOLDOWN:\n" + "NEXT TURN"
              , _ => "COOLDOWN:\n" + cooldown + " TURNS"
            };

            notEnoughMPTxt.enabled = !enough && cooldown == 0;
        }

        public void SetCallback(UnityAction callback) {
            utils.SetListener(button, callback);
        }
    }
}