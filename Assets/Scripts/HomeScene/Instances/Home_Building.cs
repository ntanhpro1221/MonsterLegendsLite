using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace MonsterLegendsLite {
    public class Home_Building : MonoBehaviourExt, IPointerClickHandler {
        [ShowInInspector, ReadOnly]
        public virtual string InsId { get; protected set; }
        
        protected virtual void Initialize(string insId) {
            InsId = insId;
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            Home_SceneManager.Ins.OnClicked_Building(this);
        }
    }
}