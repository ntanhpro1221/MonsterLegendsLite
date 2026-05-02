using DG.Tweening;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_InfoManager : MonoBehaviourExt {
        [SerializeField, Required]
        private Home_UI_MoveBuildingInfo uiMoveBuilding;
        
        [SerializeField, Required]
        private Home_UI_MoveMonsterInfo uiMoveMonster;
        
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

        private Home_UI_BuildingInfo curBuildingInfo;

        public void Initialize() {
            DisableInfo(uiMoveBuilding);
            DisableInfo(uiMoveMonster);
            DisableInfo(uiFarm);
            DisableInfo(uiHabitat);
        }

        public void ShowBuildingInfoFor(Home_Building target, bool hideAllCurrentInfo) {
            if (curBuildingInfo != null && curBuildingInfo.CurTarget == target) return;
            
            TryHideInfo(curBuildingInfo, immediately: true);

            if (uiMoveBuilding.CurTarget != null
             || uiMoveMonster.CurTarget != null) {
                if (hideAllCurrentInfo) {
                    TryHideInfo(uiMoveBuilding, immediately: true);
                    TryHideInfo(uiMoveMonster, immediately: true);
                } else return;
            }

            curBuildingInfo = target switch {
                Home_Farm    => uiFarm
              , Home_Habitat => uiHabitat

              , _ => null
            };

            if (curBuildingInfo == null) {
                Debug.LogError($"Failed to show info, building is unknown type {target.GetType().Name}");
                return;
            }

            ShowInfo(curBuildingInfo, target);
        }

        public void ShowMoveBuildingInfoFor(Home_Building target) {
            if (uiMoveBuilding.CurTarget == target) return;
             
            TryHideInfo(curBuildingInfo, immediately: true);
            
            ShowInfo(uiMoveBuilding, target);
        }

        public void ShowMoveMonsterInfo(Home_Monster target) {
            if (uiMoveMonster.CurTarget == target) return;
            
            // In design, this func is called only when bootData => so there is nothing to hide (it maybe change in the future)
            
            ShowInfo(uiMoveMonster, target);
        }
        
        private void ShowInfo(Home_UI_Info info, Object target) {
            info.LoadInfoFor(target);
            info.RectTF.SetAsLastSibling();
            DOShowInfo(info);
            info.TrySelectTarget();
        }

        public void TryHideCurBuildingInfo() {
            TryHideInfo(curBuildingInfo, immediately: false);
        }
        
        public void TryHideMoveMonsterInfo() {
            TryHideInfo(uiMoveMonster, immediately: false);
        }
        
        private void TryHideInfo(Home_UI_Info info, bool immediately) {
            if (info == null || info.CurTargetWeak == null) return;

            if (immediately) {
                var fakeHideInfo = Instantiate(info, info.RectTF.parent);
                LayoutRebuilder.ForceRebuildLayoutImmediate(fakeHideInfo.RectTF);
                fakeHideInfo.RectTF.SetSiblingIndex(info.RectTF.GetSiblingIndex());
                DOHideInfo(fakeHideInfo, hideForShow: true, () => Destroy(fakeHideInfo.gameObject));
                
                info.RectTF.DOKill();
                DisableInfo(info);
            } else DOHideInfo(info, hideForShow: false);

            info.TryDeselectTarget();
            info.UnloadInfo();
        }

        private void DOShowInfo(Home_UI_Info info, TweenCallback onComplete = null) {
            info.gameObject.SetActive(true);
            info.enabled = true;
            info.RectTF.DOKill();
            var tween = info.RectTF
                .DOAnchorPosY(0, animDuration)
                .SetEase(showEase);
            if (onComplete != null) tween.onComplete += onComplete;
        }

        private void DOHideInfo(Home_UI_Info info, bool hideForShow, TweenCallback onComplete = null) {
            info.enabled = false;
            info.RectTF.DOKill();
            var tween = info.RectTF
                .DOAnchorPosY(posYOnDisable, animDuration)
                .SetEase(hideForShow ? hideForShowEase : hideEase)
                .OnComplete(() => info.gameObject.SetActive(false));
            if (onComplete != null) tween.onComplete += onComplete;
        }
        
        private void DisableInfo(Home_UI_Info info) {
            info.RectTF.anchoredPosition = new Vector2(0, posYOnDisable);
            info.gameObject.SetActive(false);
        }
    }
}