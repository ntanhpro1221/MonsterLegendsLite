using DG.Tweening;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_BuildingInfoManager : MonoBehaviourExt {
        [SerializeField, Required]
        private Home_UI_MoveInfo uiMove;
        
        [SerializeField, Required]
        private Home_UI_FarmInfo uiFarm;

        [SerializeField, Required]
        private Home_UI_HabitatInfo uiHabitat;

        [SerializeField, FoldoutGroup("Show Anim")]
        private float posYOnDisable;

        [SerializeField, FoldoutGroup("Show Anim")]
        private float animDuration;

        [SerializeField, FoldoutGroup("Show Anim")]
        private Ease showEase;

        [SerializeField, FoldoutGroup("Show Anim")]
        private Ease hideEase;
        
        [SerializeField, FoldoutGroup("Show Anim")]
        private Ease hideForShowEase;

        private Home_UI_BuildingInfo curInfo;

        private void Awake() {
            DisableInfo(uiMove);
            DisableInfo(uiFarm);
            DisableInfo(uiHabitat);
        }

        public void ShowInfoFor(Home_Building building, bool hideMoveInfo) {
            if (curInfo != null && curInfo.CurTarget == building) return;
            
            TryHideInfo(curInfo, immediately: true);

            if (uiMove.CurTarget != null) {
                if (hideMoveInfo) TryHideInfo(uiMove, immediately: true);
                else return;
            }

            curInfo = building switch {
                Home_Farm    => uiFarm
              , Home_Habitat => uiHabitat

              , _ => null
            };

            if (curInfo == null) {
                Debug.LogError($"Failed to show info, building is unknown type {building.GetType().Name}");
                return;
            }

            ShowInfo(curInfo, building);
        }

        public void ShowMoveInfoFor(Home_Building building) {
            if (uiMove.CurTarget == building) return;
             
            TryHideInfo(curInfo, immediately: true);
            
            ShowInfo(uiMove, building);
        }

        private void ShowInfo(Home_UI_BuildingInfo info, Home_Building target) {
            info.LoadInfoFor(target);
            info.RectTF.SetAsLastSibling();
            DOShowInfo(info);
            target.OnSelect();
        }

        public void TryHideCurInfo() {
            TryHideInfo(curInfo, immediately: false);
        }
        
        private void TryHideInfo(Home_UI_BuildingInfo info, bool immediately) {
            if (info == null || info.CurTarget == null) return;

            if (immediately) {
                var fakeHideInfo = Instantiate(info, info.RectTF.parent);
                LayoutRebuilder.ForceRebuildLayoutImmediate(fakeHideInfo.RectTF);
                fakeHideInfo.RectTF.SetSiblingIndex(info.RectTF.GetSiblingIndex());
                DOHideInfo(fakeHideInfo, hideForShow: true, () => Destroy(fakeHideInfo.gameObject));
                
                info.RectTF.DOKill();
                DisableInfo(info);
            } else DOHideInfo(info, hideForShow: false);

            info.CurTarget.OnDeselect();
            info.UnloadInfo();
        }

        private void DOShowInfo(Home_UI_BuildingInfo target, TweenCallback onComplete = null) {
            target.gameObject.SetActive(true);
            target.enabled = true;
            target.RectTF.DOKill();
            var tween = target.RectTF
                .DOAnchorPosY(0, animDuration)
                .SetEase(showEase);
            if (onComplete != null) tween.onComplete += onComplete;
        }

        private void DOHideInfo(Home_UI_BuildingInfo target, bool hideForShow, TweenCallback onComplete = null) {
            target.enabled = false;
            target.RectTF.DOKill();
            var tween = target.RectTF
                .DOAnchorPosY(posYOnDisable, animDuration)
                .SetEase(hideForShow ? hideForShowEase : hideEase)
                .OnComplete(() => target.gameObject.SetActive(false));
            if (onComplete != null) tween.onComplete += onComplete;
        }
        
        private void DisableInfo(Home_UI_BuildingInfo target) {
            target.RectTF.anchoredPosition = new Vector2(0, posYOnDisable);
            target.gameObject.SetActive(false);
        }
    }
}