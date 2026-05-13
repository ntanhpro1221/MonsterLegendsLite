using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterLegendsLite {
    public class Battle_Monster : MonoBehaviourExt, IPointerClickHandler {
        public delegate void ApplyTurnCompleted(List<Battle_Monster> dieMonsters);

        [NonSerialized, ShowInInspector, ReadOnly]
        public MonsterInsData insData;

        [NonSerialized, ShowInInspector, ReadOnly]
        private readonly MonsterSkillList<Battle_Skill> skillList = new();

        [SerializeField, Required]
        private MonsterModel model;

        [SerializeField, Required]
        private GameObject turnIndicator;

        [SerializeField, Required]
        private Battle_StatBar hpBar, mpBar;

        [SerializeField]
        private float atkMoveSpeed;

        private MonsterStats<int> orgStats, finalStats;
        private Tween moveTask;

        public event Action<Battle_Monster> onClick;

        public Sprite Avatar { get; private set; }
        public int CurHP { get; private set; }
        public int CurMP { get; private set; }
        public IReadOnlyList<Battle_Skill> SkillList => skillList;

        public void Initialize(MonsterInsData insData, HorDirection faceDir) {
            var defData        = DataManager.Ins.GameDefData.Monsters[insData.Id];
            var elementLocData = DataManager.Ins.GameLocDefData.Elements;

            this.insData = insData;

            Avatar = DataManager.Ins.GameLocDefData.Monsters[insData.Id].Avatar;

            orgStats = defData.CalculateStats(insData);

            UpdateFinalStats();

            hpBar.Initialize(CurHP = finalStats[MonsterStatId.HP]);
            mpBar.Initialize(CurMP = finalStats[MonsterStatId.MP]);

            for (int i = 0; i < skillList.Count; ++i) {
                var skill = insData.GetSkill(i, defData);
                if (skill == null) continue;
                skillList[i] = new(skill, elementLocData[skill.Element]);
            }

            SetVisibleTurnIndicator(false);
            model.CurDir = faceDir;
        }

        private void OnDestroy() {
            moveTask?.Kill();
        }

        public void OnPointerClick(PointerEventData eventData) {
            onClick?.Invoke(this);
        }

        public int GetStat(MonsterStatId statId) {
            return finalStats[statId];
        }

        // TODO: Add effects in the future
        private void UpdateFinalStats() {
            finalStats ??= new();
            foreach (var key in finalStats.Keys) finalStats[key] = orgStats[key];
        }

        public void ApplyTurn(bool isRecharge, Battle_Skill skill, IEnumerable<Battle_Monster> targets, ApplyTurnCompleted onCompleted) {
            if (isRecharge) Recharge(onCompleted);
            else StartCoroutine(IEApplySkill(skill, targets.ToList(), onCompleted));
        }

        public void Recharge(ApplyTurnCompleted onCompleted) {
            ApplyDeltaMP(50);
            utils.DelayedCall_Second(this, .5f, () => onCompleted.Invoke(new List<Battle_Monster>()));
        }

        private IEnumerator IEApplySkill(Battle_Skill skill, List<Battle_Monster> targets, ApplyTurnCompleted onCompleted) {
            const float atkTouchPoint = .7f;

            var mainTarget   = targets.First();
            var orgParent    = TF.parent;
            var orgPos       = TF.position;
            var orgDir       = model.CurDir;
            var moveDuration = Vector2.Distance(TF.position, mainTarget.TF.position) / atkMoveSpeed;

            ApplyDeltaMP(-skill.skillData.MPCost);
            skill.StartCooldown();

            if (mainTarget != this) {
                model.LookAt(mainTarget.TF.position.x);
                model.Play(MonsterAnimId.Walk);
                moveTask = TF.DOMove(mainTarget.model.AttackPos, moveDuration).SetEase(Ease.Linear);
                yield return WaitForSecondCache.Get(moveDuration / 2);

                TF.SetParent(mainTarget.TF.parent);
                yield return WaitForSecondCache.Get(moveDuration / 2);
            }

            model.Play(MonsterAnimId.Attack, out var atkDuration);
            yield return WaitForSecondCache.Get(atkDuration * atkTouchPoint);

            var dieMonsters = new List<Battle_Monster>();
            foreach (var target in targets) {
                target.ApplyDeltaHP(CalcDeltaHP(skill, target));
                var isAlive = target.CurHP > 0;

                if (skill.skillData.Target.IsTargetEnemy()) StartCoroutine(IEPlayMonsterTakeDamageAnim(target, isAlive));

                if (!isAlive) dieMonsters.Add(target);
            }

            yield return WaitForSecondCache.Get(atkDuration * (1 - atkTouchPoint));

            if (mainTarget != this) {
                model.CurDir = model.CurDir.Flip();
                model.Play(MonsterAnimId.Walk);
                moveTask = TF.DOMove(orgPos, moveDuration).SetEase(Ease.Linear);
                yield return WaitForSecondCache.Get(moveDuration / 2);

                TF.SetParent(orgParent);
                yield return WaitForSecondCache.Get(moveDuration / 2);
            }

            model.CurDir = orgDir;
            model.Play(MonsterAnimId.Idle);

            onCompleted?.Invoke(dieMonsters);

            static IEnumerator IEPlayMonsterTakeDamageAnim(Battle_Monster target, bool isAlive) {
                const float deadPoint = .7f;

                target.model.Play(MonsterAnimId.Hurt, out var hurtDuration);
                yield return WaitForSecondCache.Get(hurtDuration * deadPoint);

                if (isAlive) yield break;

                target.model.Play(MonsterAnimId.Death, out var deadDuration);
                yield return WaitForSecondCache.Get(deadDuration);

                target.gameObject.SetActive(false);
            }
        }

        public void SetVisibleTurnIndicator(bool visible) {
            turnIndicator.SetActive(visible);
        }

        public void SetDeltaHP(int amount) {
            hpBar.SetDelta(amount, CurHP, finalStats[MonsterStatId.HP]);
        }

        public void ApplyDeltaHP(int amount) {
            hpBar.ApplyDelta(amount, CurHP, finalStats[MonsterStatId.HP]);
            CurHP = Mathf.Clamp(CurHP + amount, 0, finalStats[MonsterStatId.HP]);

            FloatingTextPool.Ins
                .ShowAtWorld(amount < 0 ? FloatingTextId.TakeDamageNormal : FloatingTextId.Heal, TF.position)
                .SetTextChange(amount);
        }

        public void SetDeltaMP(int amount) {
            mpBar.SetDelta(amount, CurMP, finalStats[MonsterStatId.MP]);
        }

        public void ApplyDeltaMP(int amount) {
            mpBar.ApplyDelta(amount, CurMP, finalStats[MonsterStatId.MP]);
            CurMP = Mathf.Clamp(CurMP + amount, 0, finalStats[MonsterStatId.MP]);

            if (amount > 0) {
                FloatingTextPool.Ins
                    .ShowAtWorld(FloatingTextId.ManaChange, TF.position)
                    .SetTextChange(amount);
            }
        }

        public int CalcDeltaHP(Battle_Skill skill, Battle_Monster target) {
            return Mathf.RoundToInt(
                (skill.skillData.Target.IsTargetEnemy() ? -finalStats[MonsterStatId.Atk] : target.finalStats[MonsterStatId.HP])
              * skill.skillData.PowerRate / 100f);
        }
    }
}