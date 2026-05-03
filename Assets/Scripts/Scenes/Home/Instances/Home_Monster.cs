using System.Collections;
using DG.Tweening;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Monster : MonoBehaviourExt {
        [ShowInInspector, ReadOnly, PropertyOrder(-100)]
        public MonsterInsData InsData { get; private set; }

        [SerializeField, Required]
        private MonsterModel model;

        private bool isBuySample;

        public void Initialize(MonsterInsData insData, bool isBuySample) {
            InsData          = insData;
            this.isBuySample = isBuySample;

            if (isBuySample) {
                gameObject.SetActive(false);
                
                
            } else {
                EventDispatcher.RegisterEvent(EventId.UserMonsterListChanged, DestroyIfNotExistInDatabase, this);
            }
        }

        private void OnDestroy() {
            TF.DOKill();
            
            EventDispatcher.UnregisterEvent(EventId.UserMonsterListChanged, DestroyIfNotExistInDatabase, this);
        }

        private void DestroyIfNotExistInDatabase() {
            if (DataManager.Ins.UserInsData.Monsters.Contains(InsData)) return;
            Destroy(gameObject);
        }

        public int GetGPM() {
            return DataManager.Ins.GameDefData.Monster[InsData.Id].CalculateStat(InsData, MonsterStatId.GoldPerMin);
        }

        public void StartLocalMove(Vector2Int size) {
            StopCoroutine(nameof(IELocalMove));
            StartCoroutine(IELocalMove(size));
        }

        public void OnMoveDiscarded() {
            if (isBuySample) {
                gameObject.SetActive(false); // Something bad is going to happen if destroy this
            } else { }
        }

        public void OnMoveConfirmed(Home_Habitat toHabitat) {
            if (isBuySample) {
                DataManager.Ins.UpdateData_BuyMonster(InsData.Id, toHabitat.InsData, out var cost, out _);
                
                FloatingTextPool.Ins.ShowAtCenterScreen(FloatingTextId.GoldChange).SetTextChange(-cost);
                
                EventDispatcher.PostEvent(EventId.UserMonsterListChanged);
                EventDispatcher.PostEvent(EventId.UserGoldChanged);
                
                gameObject.SetActive(false); // Something bad is going to happen if destroy this
                
                Home_SceneManager.Ins.TryHideMoveMonsterInfo();
            } else {
                DataManager.Ins.UpdateData_MoveMonster(InsData, toHabitat.InsData);
                
                EventDispatcher.PostEvent(EventId.HomeMonsterPlaceChanged);
                
                Home_SceneManager.Ins.ForceShowBuildingInfo(toHabitat);
            }
        }

        private IEnumerator IELocalMove(Vector2Int size) {
            while (true) {
                var habitatPos = Home_MapManager.Ins.GetNearestTilePos(Home_SceneManager.Ins.Habitats[InsData.Habitat].TF.position);
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