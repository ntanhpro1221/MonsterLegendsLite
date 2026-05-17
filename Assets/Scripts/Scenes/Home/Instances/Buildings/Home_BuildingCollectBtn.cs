using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MonsterLegendsLite {
    public class Home_BuildingCollectBtn : MonoBehaviourExt, IPointerClickHandler {
        [SerializeField, Required]
        private SpriteRendererAnchorer anchorer;

        [SerializeField, Required]
        private SpriteRenderer iconSpr;

        public event UnityAction<PointerEventData> onPointerClickRaw;

        public void OnPointerClick(PointerEventData eventData) {
            onPointerClickRaw?.Invoke(eventData);
        }

        public void SetActive(bool active) {
            gameObject.SetActive(active);

            if (active) anchorer.UpdatePosFromAnchor();
        }

        public void SetIcon(Sprite icon) {
            iconSpr.sprite = icon;
        }
    }
}