using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MonsterLegendsLite {
    public class Adventure_Level : MonoBehaviourExt, IPointerClickHandler {
        [field: SerializeField, Required]
        protected Adventure_LevelSharedData SharedData { get; private set; }

        private UnityAction onClick;

        public virtual void SetAllData(AdventureLevelData levelData, int levelIndex) {
            var userCurLevelIndex = DataManager.Ins.User.CurAdventureLevel;
            
            var state =
                userCurLevelIndex < levelIndex ? Adventure_LevelState.Locking :
                userCurLevelIndex > levelIndex ? Adventure_LevelState.Cleared :
                                                 Adventure_LevelState.Targeting;
            SetState(state);
            SetIndex(levelIndex);
            SetVisibleIndicator(state == Adventure_LevelState.Targeting);
            SetCallback(() => {
                if (state == Adventure_LevelState.Locking) return;
                AdventureLevelDetailWindow.Show(SharedData.PrefabAdventureLevelDetailWindow, levelData, levelIndex);
            });
        }

        public void SetState(Adventure_LevelState state) {
            SharedData.BackgroundSpr.sprite = SharedData.BackgroundVariants[state];
        }

        public void SetIndex(int index) {
            SharedData.IndexTxt.text = (index + 1).ToString();
        }

        public void SetVisibleIndicator(bool visible) {
            SharedData.IndicatorImg.enabled = visible;
        }

        public void SetCallback(UnityAction callback) {
            onClick = callback;
        }

        public void OnPointerClick(PointerEventData eventData) {
            onClick?.Invoke();
        }
    }
}