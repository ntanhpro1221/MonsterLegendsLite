using System;
using System.Collections;
using DG.Tweening;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace MonsterLegendsLite {
    public class Home_Monster : MonoBehaviourExt {
        public enum Type {
            Normal
          , BuySample
          , HatchSample
        }

        [ShowInInspector, ReadOnly, PropertyOrder(-100)]
        public MonsterInsData InsData { get; private set; }

        [SerializeField, Required]
        private MonsterModel model;

        [SerializeField, Required]
        private SortingGroup sortingGroup;

        private Type type;

        public void Initialize(MonsterInsData insData, Type type) {
            InsData   = insData;
            this.type = type;

            switch (type) {
                case Type.Normal:
                    EventDispatcher.RegisterEvent(EventId.UserMonsterListChanged, DestroyIfNotExistInDatabase, this);
                    break;

                case Type.BuySample:
                    gameObject.SetActive(false);
                    break;

                case Type.HatchSample:
                    gameObject.SetActive(false);
                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDestroy() {
            TF.DOKill();

            EventDispatcher.UnregisterEvent(EventId.UserMonsterListChanged, DestroyIfNotExistInDatabase, this);
        }

        private void DestroyIfNotExistInDatabase() {
            if (DataManager.Ins.User.Monsters.Contains(InsData)) return;
            Destroy(gameObject);
        }

        public int GetGPM() {
            return DataManager.Ins.GameDef.Monsters[InsData.Id].CalculateStat(InsData, MonsterStatId.GoldPerMin);
        }

        public void StartLocalMove(Vector2Int size) {
            StopCoroutine(nameof(IELocalMove));
            StartCoroutine(IELocalMove(size));
        }

        public void OnMoveDiscarded(Home_BreedingPlace fromBreedingPlace) {
            switch (type) {
                case Type.Normal: 
                    Home_SceneManager.Ins.TryHideMoveMonsterInfo();
                    break;
                
                case Type.BuySample: 
                    Home_SceneManager.Ins.TryHideMoveMonsterInfo();
            
                    // Something bad is going to happen if destroy this :))
                    break;
                
                case Type.HatchSample: 
                    Home_SceneManager.Ins.ForceShowBuildingInfo(fromBreedingPlace);
                    
                    // Something bad is going to happen if destroy this :))
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void OnMoveConfirmed(Home_Habitat toHabitat, Home_BreedingPlace fromBreedingPlace) {
            switch (type) {
                case Type.Normal: 
                    DataManager.Ins.UpdateData_MoveMonster(InsData, toHabitat.InsData);

                    EventDispatcher.PostEvent(EventId.HomeMonsterPlaceChanged);
                    break;
                
                case Type.BuySample: 
                    DataManager.Ins.UpdateData_BuyMonster(InsData.Id, toHabitat.InsData, out var cost, out _);

                    FloatingTextPool.Ins.ShowAtCenterScreen(FloatingTextId.GoldChange).SetTextChange(-cost);

                    EventDispatcher.PostEvent(EventId.UserMonsterListChanged);
                    EventDispatcher.PostEvent(EventId.UserGoldChanged);
                    
                    // Something bad is going to happen if destroy this :))
                    break;
                
                case Type.HatchSample:
                    DataManager.Ins.UpdateData_HatchMonster(fromBreedingPlace.InsData, toHabitat.InsData, out _);
                    
                    fromBreedingPlace.ChangeState(Home_BreedingPlace.State.Normal);
                    
                    EventDispatcher.PostEvent(EventId.UserMonsterListChanged);
                    
                    // Something bad is going to happen if destroy this :))
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }

            Home_SceneManager.Ins.ForceShowBuildingInfo(toHabitat);
        }

        private IEnumerator IELocalMove(Vector2Int size) {
            while (true) {
                var habitatPos    = Home_MapManager.Ins.GetNearestTilePos(Home_SceneManager.Ins.Habitats[InsData.Habitat].TF.position);
                var gloTarget     = Home_MapManager.Ins.RandomPointInHabitat(habitatPos, size);
                var locTarget     = TF.parent.InverseTransformPoint(gloTarget);
                var duration      = Vector2.Distance(TF.localPosition, locTarget) / DataManager.Ins.GameDef.Home_MonsterSpeed;
                var habitatRangeY = Home_MapManager.Ins.GetHabitatRangeY(habitatPos, size);
                var habitatLocRangeY = new Vector2(
                    TF.parent.InverseTransformPoint(new(0, habitatRangeY.x)).y
                  , TF.parent.InverseTransformPoint(new(0, habitatRangeY.y)).y);

                model.LookAt(gloTarget.x);
                model.Play(MonsterAnimId.Walk);

                yield return TF
                    .DOLocalMove(locTarget, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateSortingOrderFromPosY)
                    .WaitForCompletion();

                model.Play(MonsterAnimId.Idle);

                yield return WaitForSecondCache.Get(utils.RandomInside(DataManager.Ins.GameDef.Home_MonsterIdleTime));

                void UpdateSortingOrderFromPosY() {
                    sortingGroup.sortingOrder = (int)Mathf.Lerp(short.MaxValue, short.MinValue
                      , Mathf.InverseLerp(habitatLocRangeY.x, habitatLocRangeY.y, TF.localPosition.y));
                }
            }
        }
    }
}