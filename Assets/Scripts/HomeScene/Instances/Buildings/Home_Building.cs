using System;
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

        public event Action<bool> onPlaceableChanged;

        public T To<T>() where T : Home_Building {
            return (T)this;
        }

        protected virtual void Initialize(string insId) {
            InsId = insId;

            var size = GetSizeData();
            SharedData.selectOutline.size = size;
            SharedData.validPlaceSpr.size = size;
            SharedData.invalidPlaceSpr.size = size;
            foreach (var arrowAnchor in SharedData.arrowAnchors) arrowAnchor.UpdatePosFromAnchor();

            SharedData.selectWrapper.gameObject.SetActive(false);

            SetVisibleValidPlace(false);
        }

        public void OnSelect() {
            isSelected = true; 
            SharedData.selectWrapper.gameObject.SetActive(true);
            SharedData.sortingGroup.sortingOrder = SharedData.orderOnSelected;
        }
        
        public void OnDeselect() {
            isSelected = false; 
            SharedData.selectWrapper.gameObject.SetActive(false);
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
            if (isPntMoved) return;
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

                    var isPlaceable = Home_MapManager.Ins.IsPlaceable(this);
                    SharedData.validPlaceSpr.enabled   = isPlaceable;
                    SharedData.invalidPlaceSpr.enabled = !isPlaceable;
                    
                    onPlaceableChanged?.Invoke(isPlaceable);
                }

                yield return null;
            }
        }

        public void SetVisibleValidPlace(bool isOn) {
            SharedData.validPlaceSpr.gameObject.SetActive(isOn); 
            SharedData.invalidPlaceSpr.gameObject.SetActive(isOn); 
        }

        public void ResetPos() {
            TF.position = Home_MapManager.Ins.GetWorldPos(GetPosData());
        }

        public void SaveCurPos() {
            SavePos(Home_MapManager.Ins.GetNearestTilePos(TF.position));
        }

        public abstract Vector2Int GetSizeData();
        public abstract Vector2Int GetPosData();
        protected abstract void SavePos(Vector2Int tilePos);
    }
}