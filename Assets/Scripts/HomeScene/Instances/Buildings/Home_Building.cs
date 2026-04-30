using System.Collections;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterLegendsLite {
    public abstract class Home_Building : MonoBehaviourExt, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler {
        [ShowInInspector, ReadOnly, PropertyOrder(-100)]
        public virtual string InsId { get; protected set; }

        [field: SerializeField, Required]
        protected Home_BuildingSharedData SharedData { get; private set; }

        private bool isSelected, isPntDown, isPntMoved;
        private int pntId;
        private Vector2 pntDownOffset;

        public T To<T>() where T : Home_Building {
            return (T)this;
        }
        
        protected virtual void Initialize(string insId) {
            InsId = insId;

            SharedData.selectOutline.size = GetSizeData();
            foreach (var arrowAnchor in SharedData.arrowAnchors) arrowAnchor.UpdatePosFromAnchor();
            
            SharedData.outlineWrapper.gameObject.SetActive(false);
        }

        public void OnSelect() {
            isSelected = true; 
            SharedData.outlineWrapper.gameObject.SetActive(true);
            SharedData.sortingGroup.sortingOrder = SharedData.orderOnSelected;
        }
        
        public void OnDeselect() {
            isSelected = false; 
            SharedData.outlineWrapper.gameObject.SetActive(false);
            SharedData.sortingGroup.sortingOrder = 0;
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            Home_SceneManager.Ins.OnClicked_Building(this);
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (!isSelected) return;
            
            isPntDown = true;
            isPntMoved = false;
            pntId = eventData.pointerId;
            pntDownOffset = Home_SceneManager.Ins.Cam.ScreenToWorldPoint(eventData.position) - TF.position;

            StartCoroutine(IEMoveWithPointer());
        }
        
        public void OnPointerUp(PointerEventData eventData) {
            if (pntId != eventData.pointerId) return;
            
            isPntDown = false;
        }
        
        public void OnPointerMove(PointerEventData eventData) {
            if (!isPntDown) return;
            if (pntId != eventData.pointerId) return;
            
            isPntMoved = true;
            Home_SceneManager.Ins.OnMove_Building(this);
        }

        private IEnumerator IEMoveWithPointer() {
            while (isPntDown && isSelected) {
                if (isPntMoved) {
                    var expectedPos    = (Vector2)Home_SceneManager.Ins.Cam.ScreenToWorldPoint(utils.GetPointerPos(pntId)) - pntDownOffset;
                    var nearestTilePos = Home_MapManager.Ins.GetNearestTilePos(expectedPos);
                    TF.position = Home_MapManager.Ins.GetWorldPos(nearestTilePos);
                }

                yield return null;
            }
        }

        public void ResetPos() {
            TF.position = Home_MapManager.Ins.GetWorldPos(GetPosData());
        }

        public void SaveCurPos() {
            SavePos(Home_MapManager.Ins.GetNearestTilePos(TF.position));
        }

        protected abstract Vector2Int GetSizeData();
        protected abstract Vector2Int GetPosData();
        protected abstract void SavePos(Vector2Int tilePos);
    }
}