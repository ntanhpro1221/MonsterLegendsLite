using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Battle_UI_TurnManager : MonoBehaviourExt {
        [SerializeField, Required]
        private HorizontalOrVerticalLayoutGroup offsetLayer;

        [SerializeField, Required]
        private RectTransform rebuildOutPoint;

        [SerializeField, Required]
        private Battle_UI_Turn sampleLeft, sampleRight;

        [SerializeField, Required]
        private Battle_UI_Turn[] items;

        [SerializeField]
        private float nextDuration;

        [SerializeField]
        private Ease normalNextEase;

        [SerializeField]
        private Ease rebuildNextEase_Out, rebuildNextEase_In;

        private IReadOnlyDictionary<string, Battle_Monster> orgAllies;

        private readonly List<Battle_Monster> turnCycle = new();

        private int turnId;
        private Tween nextTurnTask;

        public void Initialize(IReadOnlyDictionary<string, Battle_Monster> allies, IReadOnlyDictionary<string, Battle_Monster> enemies) {
            orgAllies = new Dictionary<string, Battle_Monster>(allies);
            turnCycle.AddRange(allies.Values.Concat(enemies.Values).OrderBy(static item => -item.GetStat(MonsterStatId.Speed)));
            RebuildUILogic();
        }

        private void OnDestroy() {
            nextTurnTask?.Kill();
        }

        public Battle_Monster GetTurn() {
            return turnCycle[turnId];
        }

        public void NextTurn(List<Battle_Monster> dieMonsters, Action onCompleted) {
            turnId = (turnId + 1) % turnCycle.Count;

            for (int i = turnCycle.Count - 1; i >= 0; --i) {
                if (!dieMonsters.Contains(turnCycle[i])) continue;
                turnCycle.RemoveAt(i);
                if (i < turnId) --turnId;
            }

            turnId %= turnCycle.Count;

            StartCoroutine(dieMonsters.Count == 0 ? NextTurnUI(onCompleted) : RebuildUI(onCompleted));
        }

        private IEnumerator RebuildUI(Action onCompleted) {
            var rootItem = offsetLayer.GetComponent<RectTransform>();

            nextTurnTask = rootItem
                .DOAnchorPosY(rebuildOutPoint.anchoredPosition.y, nextDuration / 2)
                .SetEase(rebuildNextEase_Out);
            yield return nextTurnTask.WaitForCompletion();

            RebuildUILogic();

            nextTurnTask = rootItem
                .DOAnchorPosY(0, nextDuration / 2)
                .SetEase(rebuildNextEase_In);
            yield return nextTurnTask.WaitForCompletion();
            
            onCompleted?.Invoke();
        }

        private IEnumerator NextTurnUI(Action onCompleted) {
            var allTask    = DOTween.Sequence(this);
            var rootItem   = offsetLayer.GetComponent<RectTransform>();
            var firstItem  = items[0];
            var secondItem = items[1];
            var lastItem   = items[^1];
            var staticItem = items[^2];

            nextTurnTask = allTask;

            // change root item offset
            allTask.Join(
                rootItem.DOAnchorPosX(-sampleLeft.RectTF.sizeDelta.x - offsetLayer.spacing
                  , nextDuration).SetEase(normalNextEase));

            // change offset of first item
            allTask.Join(
                firstItem.OffsetLayer.DOAnchorPos(sampleLeft.OffsetLayer.anchoredPosition
                  , nextDuration).SetEase(normalNextEase));

            // change size, offset of second item
            allTask.Join(
                secondItem.OffsetLayer.DOAnchorPos(firstItem.OffsetLayer.anchoredPosition
                  , nextDuration).SetEase(normalNextEase));
            allTask.Join(
                DOTween.To(secondItem.GetMinHeight, secondItem.SetMinHeight, firstItem.GetMinHeight()
                  , nextDuration).SetEase(normalNextEase));

            // change offset last item
            allTask.Join(
                lastItem.OffsetLayer.DOAnchorPos(staticItem.OffsetLayer.anchoredPosition
                  , nextDuration).SetEase(normalNextEase));

            yield return allTask.WaitForCompletion();

            rootItem.anchoredPosition               = Vector2.zero;
            firstItem.OffsetLayer.anchoredPosition  = secondItem.OffsetLayer.anchoredPosition;
            secondItem.OffsetLayer.anchoredPosition = staticItem.OffsetLayer.anchoredPosition;
            secondItem.SetMinHeight(staticItem.GetMinHeight());
            lastItem.OffsetLayer.anchoredPosition = sampleRight.OffsetLayer.anchoredPosition;

            LayoutRebuilder.ForceRebuildLayoutImmediate(rootItem);

            RebuildUILogic();
            
            onCompleted?.Invoke();
        }

        private void RebuildUILogic() {
            int id = turnId;
            foreach (var item in items) {
                var data = turnCycle[id];

                item.SetAvatar(data.Avatar);
                item.SetGlow(false);
                item.SetTeam(orgAllies.ContainsKey(data.insData.InsId));

                id = (id + 1) % turnCycle.Count;
            }

            items[0].SetGlow(true);
        }
    }
}