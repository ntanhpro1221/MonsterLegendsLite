using System;
using System.Collections;
using DG.Tweening;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegends;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Monster : MonoBehaviourExt {
        [NonSerialized, ShowInInspector, ReadOnly]
        public string insId;

        [SerializeField, Required]
        private MonsterModel model;

        public void Initialize(string insId) {
            this.insId = insId;
        }

        public void StartLocalMove(Vector2Int pos, Vector2Int size) {
            StopCoroutine(nameof(IELocalMove));
            StartCoroutine(IELocalMove(pos, size));
        }

        private IEnumerator IELocalMove(Vector2Int pos, Vector2Int size) {
            while (true) {
                var locTarget = TF.parent.InverseTransformPoint(Home_MapManager.Ins.RandomPointInHabitat(pos, size));
                var duration  = Vector2.Distance(TF.localPosition, locTarget) / DataManager.Ins.GameDefData.Home_MonsterSpeed;

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