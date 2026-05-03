using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterLegendsLite {
    public class Home_VoidClickTracker : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            Home_SceneManager.Ins.OnClicked_Void();
        }
    }
}