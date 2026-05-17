using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterLegendsLite {
    public class Home_VoidClickTracker : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            var deltaPos = Vector2.Distance(eventData.pressPosition, eventData.position);
            if (deltaPos > EventSystem.current.pixelDragThreshold) return;
            
            Home_SceneManager.Ins.OnClicked_Void();
        }
    }
}