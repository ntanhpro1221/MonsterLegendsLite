using DG.Tweening;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_BuildingInfoManager : MonoBehaviourExt {
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
            DisableInfo(uiFarm);
            DisableInfo(uiHabitat);
        }

        public void ShowInfoFor(Home_Building building) {
            if (curInfo != null) {
                if (curInfo.CurTargetId == building.InsId) return;

                var fakeHideInfo = Instantiate(curInfo, curInfo.RectTF.parent);
                LayoutRebuilder.ForceRebuildLayoutImmediate(fakeHideInfo.RectTF);
                fakeHideInfo.RectTF.SetSiblingIndex(curInfo.RectTF.GetSiblingIndex());
                DOHideInfo(fakeHideInfo, true, () => Destroy(fakeHideInfo.gameObject));

                curInfo.RectTF.DOKill();
                DisableInfo(curInfo);
                curInfo = null;
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

            curInfo.LoadInfoFor(building);
            curInfo.RectTF.SetAsLastSibling();
            DOShowInfo(curInfo);
        }

        public void HideCurInfo() {
            if (curInfo == null) return;
            DOHideInfo(curInfo, false);
            curInfo = null;
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