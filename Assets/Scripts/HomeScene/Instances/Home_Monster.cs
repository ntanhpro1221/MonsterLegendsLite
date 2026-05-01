using System;
using System.Collections;
using DG.Tweening;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Monster : MonoBehaviourExt {
        [NonSerialized, ShowInInspector, ReadOnly]
        public MonsterInsData insData;

        [SerializeField, Required]
        private MonsterModel model;

        public void Initialize(MonsterInsData insData) {
            this.insData = insData;
            
            EventDispatcher.RegisterEvent(EventId.UserMonsterListChanged, DestroyIfNotExistInDatabase, this);
        }

        private void OnDestroy() {
            TF.DOKill();
            
            EventDispatcher.UnregisterEvent(EventId.UserMonsterListChanged, DestroyIfNotExistInDatabase, this);
        }

        private void DestroyIfNotExistInDatabase() {
            if (DataManager.Ins.UserInsData.Monsters.Contains(insData)) return;
            Destroy(this);
        }

        public int GetGPM() {
            return DataManager.Ins.GameDefData.Monster[insData.Id].CalculateStat(insData, MonsterStatId.GoldPerMin);
        }

        public void StartLocalMove(Vector2Int size) {
            StopCoroutine(nameof(IELocalMove));
            StartCoroutine(IELocalMove(size));
        }

        private IEnumerator IELocalMove(Vector2Int size) {
            while (true) {
                var habitatPos = Home_MapManager.Ins.GetNearestTilePos(Home_SceneManager.Ins.Habitats[insData.Habitat].TF.position);
                var locTarget  = TF.parent.InverseTransformPoint(Home_MapManager.Ins.RandomPointInHabitat(habitatPos, size));
                var duration   = Vector2.Distance(TF.localPosition, locTarget) / DataManager.Ins.GameDefData.Home_MonsterSpeed;

                model.SetDirection(TF.localPosition.x < locTarget.x ? HorDirection.Right : HorDirection.Left);
                model.Play(MonsterAnimId.Walk);

                yield return TF
                    .DOLocalMove(locTarget, duration)
                    .SetEase(Ease.Linear)
                    .WaitForCompletion();

                model.Play(MonsterAnimId.Idle);

                yield return WaitForSecondCache.Get(utils.RandomInside(DataManager.Ins.GameDefData.Home_MonsterIdleTime));
            }
        }
    }
}