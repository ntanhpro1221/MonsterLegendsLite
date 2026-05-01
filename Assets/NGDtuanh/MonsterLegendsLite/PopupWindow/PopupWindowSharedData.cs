using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NGDtuanh.MonsterLegendsLite {
    public class PopupWindowSharedData : MonoBehaviourExt {
        [SerializeField, Required]
        public CanvasGroup canvasGroup;
        
        [SerializeField, Required]
        public Button closeBtn;
        
        [SerializeField, Required]
        public TextMeshProUGUI title, content;
    }
}