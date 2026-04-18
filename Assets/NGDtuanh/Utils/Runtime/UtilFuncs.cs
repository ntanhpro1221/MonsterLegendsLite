using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace NGDtuanh.Utils {
    public class UtilFuncs {
        public enum VecAxis {
            X
          , Y
          , Z
        }

        protected virtual WaitForSeconds GetWaitForSeconds(float second) => new WaitForSeconds(second);
        
        #region TWEEN

        public Sequence DOShakePositionDynamic(
            Transform target,
            float     duration,
            Vector3[] strengths,
            int       vibrato    = 10,
            float     randomness = 90f,
            bool      snapping   = false,
            bool      fadeOut    = false) {
            if (strengths.Length == 0) return null;

            float durationUnit = duration / strengths.Length;

            var seq = DOTween.Sequence(target);

            foreach (var strength in strengths)
                seq.Append(target.DOShakePosition(
                    duration: durationUnit
                  , strength: strength
                  , vibrato: vibrato
                  , randomness: randomness
                  , snapping: snapping
                  , fadeOut: fadeOut));

            return seq;
        }

        /// <summary>
        /// Ignores global rotation and scale.<br/>
        /// See <see cref="DOLocalMovePure"/>
        /// </summary>
        public Sequence DOLocalJumpPure(
            Transform     target
          , Vector3       endPnt
          , float         jumpHeight
          , float         duration
          , TweenCallback onReachTop = null
          , Ease          moveEase   = Ease.Linear) {
            var orgY = target.position.y;
            if (target.parent != null)
                orgY -= target.parent.position.y;
            var topY = Mathf.Max(orgY, endPnt.y) + jumpHeight;

            var upDis = topY - orgY;
            var upTime = Mathf.Approximately(upDis, 0)
                ? 0
                : duration / (Mathf.Sqrt((topY - endPnt.y) / upDis) + 1); // Base on real physic formula
            var downTime = duration - upTime;

            var seq = DOTween.Sequence(target);

            seq.Insert(0, DOLocalMovePure(target, endPnt.x, duration, VecAxis.X).SetEase(moveEase));
            seq.Insert(0, DOLocalMovePure(target, endPnt.z, duration, VecAxis.Z).SetEase(moveEase));
            seq.Insert(0, DOLocalMovePure(target, topY,     upTime,   VecAxis.Y).SetEase(Ease.OutQuad));
            if (onReachTop != null) seq.InsertCallback(upTime, onReachTop);
            seq.Insert(upTime, DOLocalMovePure(target, endPnt.y, downTime, VecAxis.Y).SetEase(Ease.InQuad));

            return seq;
        }

        /// <summary>
        /// Similar to <see cref="DG.Tweening.ShortcutExtensions.DOLocalMove"/>, 
        /// but ignores global rotation and scale.
        /// </summary>
        private TweenerCore<float, float, FloatOptions> DOLocalMovePure(
            Transform target
          , float     endValue
          , float     duration
          , VecAxis   axis
          , bool      snapping = false) {
            var t = DOTween.To(
                getter: () => Get(target.position, axis) - Get(target.parent.position, axis)
              , setter: vl => SetPosition(target, axis, Get(target.parent.position, axis) + vl)
              , endValue, duration);
            t.SetOptions(snapping).SetTarget(target);
            return t;
        }

        public Sequence DOJump_BetterHeight(
            Transform     target
          , Vector3       endPnt
          , float         jumpHeight
          , float         duration
          , TweenCallback onReachTop = null
          , Ease          moveEase   = Ease.Linear) {
            var orgY = target.position.y;
            var topY = Mathf.Max(orgY, endPnt.y) + jumpHeight;

            var upDis = topY - orgY;
            var upTime = Mathf.Approximately(upDis, 0)
                ? 0
                : duration / (Mathf.Sqrt((topY - endPnt.y) / upDis) + 1); // Base on real physic formula
            var downTime = duration - upTime;

            var seq = DOTween.Sequence(target);

            seq.Insert(0, target.DOMoveX(endPnt.x, duration).SetEase(moveEase));
            seq.Insert(0, target.DOMoveZ(endPnt.z, duration).SetEase(moveEase));
            seq.Insert(0, target.DOMoveY(topY, upTime).SetEase(Ease.OutQuad));
            if (onReachTop != null) seq.InsertCallback(upTime, onReachTop);
            seq.Insert(upTime, target.DOMoveY(endPnt.y, downTime).SetEase(Ease.InQuad));

            return seq;
        }

        #endregion

        #region MATH

        public float Get(Vector3 value, VecAxis axis) => axis switch {
            VecAxis.X => value.x
          , VecAxis.Y => value.y
          , VecAxis.Z => value.z

          , _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        };

        public void Set(ref Vector3 value, VecAxis axis, float newValue) {
            switch (axis) {
                case VecAxis.X: value.x = newValue; break;
                case VecAxis.Y: value.y = newValue; break;
                case VecAxis.Z: value.z = newValue; break;

                default: throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public void SetPosition(Transform target, VecAxis axis, float newValue) {
            var newPos = target.position;
            Set(ref newPos, axis, newValue);
            target.position = newPos;
        }

        public Vector3 With(Vector3 value, VecAxis axis, float newValue) {
            Set(ref value, axis, newValue);
            return value;
        }

        public Vector3 Div(Vector3 value, Vector3 div) {
            return new(
                value.x / div.x
              , value.y / div.y
              , value.z / div.z);
        }

        public float Pow(float a, uint x) {
            float result = 1f;

            while (x > 0) {
                if ((x & 1) != 0) {
                    result *= a;
                }

                a *=  a;
                x >>= 1;
            }

            return result;
        }
        
        #endregion

        #region OBJECT


        public string GetPath(Transform trans) {
            if (trans == null) return string.Empty;
            string result = trans.name;
            while ((trans = trans.parent) != null)
                result = $"{trans.name}/{result}";
            return result;
        }

        public string GetPath(GameObject obj) => GetPath(obj?.transform);

        public string GetPath(Component comp) => GetPath(comp?.transform);

        #endregion

        #region RANDOM

        public float RandomInside(Vector2 range) {
            if (range.x > range.y) SwapXY(ref range);
            return Random.Range(range.x, range.y);
        }

        public Vector2 RandomInside(BoxCollider2D range) {
            return range.transform.TransformPoint(range.offset + range.size / 2 - new Vector2(
                range.size.x * Random.value
              , range.size.y * Random.value));
        }

        public T RandomInside<T>(IList<T> list) {
            return list[Random.Range(0, list.Count)];
        }

        public float RandomSign(float value) {
            if (Random.value > .5f) value *= -1;
            return value;
        }

        public float RandomSign(int value) {
            if (Random.value > .5f) value *= -1;
            return value;
        }

        public Vector2 RandomInCircle(Vector2 center, float radius) {
            var angle = Random.value * 2 * Mathf.PI;
            return center + Mathf.Sqrt(Random.value) * radius * new Vector2(
                Mathf.Cos(angle)
              , Mathf.Sin(angle));
        }

        #endregion

        #region SWAP

        public void Swap<T>(ref T left, ref T right) => (left, right) = (right, left);

        public void SwapXY(ref Vector2 value) => Swap(ref value.x, ref value.y);

        #endregion

        #region COROUTINE

        public Coroutine DelayedCall_Second(MonoBehaviour runner, float delay, Action callback) {
            return runner.StartCoroutine(IEDelayedCall_Second(delay, callback));
        }

        private IEnumerator IEDelayedCall_Second(float delay, Action callback) {
            yield return GetWaitForSeconds(delay);

            callback?.Invoke();
        }

        public Coroutine DelayedCall_Cond(MonoBehaviour runner, Func<bool> delay, Action callback) {
            return runner.StartCoroutine(IEDelayedCall_Cond(delay, callback));
        }

        private IEnumerator IEDelayedCall_Cond(Func<bool> delay, Action callback) {
            yield return new WaitUntil(delay);
            
            callback?.Invoke();
        }

        public Coroutine DelayedCall_Frame(MonoBehaviour runner, int delay, Action callback) {
            return runner.StartCoroutine(IEDelayedCall_Frame(delay, callback));
        }

        private IEnumerator IEDelayedCall_Frame(int delay, Action callback) {
            while (delay-- > 0) yield return null;

            callback?.Invoke();
        }

        public Coroutine WaitForSeconds(MonoBehaviour runner, float seconds, Action<float> onProgressChanged) {
            return runner.StartCoroutine(IEWaitForSeconds(seconds, onProgressChanged));
        }

        private IEnumerator IEWaitForSeconds(float seconds, Action<float> onProgressChanged) {
            float elapsedTime = 0;

            do {
                onProgressChanged?.Invoke(elapsedTime / seconds);
                yield return null;
            } while ((elapsedTime += Time.deltaTime) < seconds);

            onProgressChanged?.Invoke(1);
        }

        #endregion

        #region PARTICLE


        public void EmitAt(
            ParticleSystem ps
          , Vector3        pos
          , int            amount     = 1
          , bool           applyShape = false) {
            var emitParams = new ParticleSystem.EmitParams {
                position             = pos
              , applyShapeToPosition = applyShape
            };

            ps.Emit(emitParams, amount);
        }

        public void BurstAt(
            ParticleSystem ps
          , MonoBehaviour  runner
          , Vector3        pos
          , int            amount     = 1
          , float          interval   = 0
          , bool           applyShape = false) {
            if (interval > 0) runner.StartCoroutine(IEBurstAt(ps, pos, amount, interval, applyShape));
            else EmitAt(ps, pos, amount, applyShape);

        }

        private IEnumerator IEBurstAt(
            ParticleSystem ps
          , Vector3        pos
          , int            amount
          , float          interval
          , bool           applyShape) {
            while (amount-- > 0) {
                EmitAt(ps, pos, 1, applyShape);

                if (amount > 0)
                    yield return GetWaitForSeconds(interval);
            }
        }

        public void BurstAllAt(
            ParticleSystem ps
          , MonoBehaviour  runner
          , Vector3        pos
          , bool           applyShape = false) {
            var emission = ps.emission;
            var bursts   = new ParticleSystem.Burst[emission.burstCount];
            emission.GetBursts(bursts);

            foreach (var burst in bursts)
                runner.StartCoroutine(IEBurstSingleAt(ps, pos, burst, applyShape, burst.time));
        }

        private IEnumerator IEBurstSingleAt(
            ParticleSystem       ps
          , Vector3              pos
          , ParticleSystem.Burst burst
          , bool                 applyShape
          , float                delay) {
            if (delay > 0) yield return GetWaitForSeconds(delay);

            int cycles = Mathf.Max(1, burst.cycleCount);
            while (cycles-- > 0) {
                if (Random.value <= burst.probability)
                    EmitAt(ps, pos, (int)burst.count.Evaluate(Random.value, Random.value), applyShape);

                if (cycles > 0)
                    yield return GetWaitForSeconds(burst.repeatInterval);
            }
        }

        #endregion

        #region DATA MODIFY

        [Conditional("UNITY_EDITOR")]
        public void RecordForUndo(Object target) {
            #if UNITY_EDITOR
            
            UnityEditor.Undo.RecordObject(target, $"Modify {target.name} {target.GetType().Name}");
            
            #endif
        }
        
        [Conditional("UNITY_EDITOR")]
        public void RecordForUndo(params Object[] targets) {
            foreach (var target in targets) RecordForUndo(target);
        }

        [Conditional("UNITY_EDITOR")]
        public void MarkDirty(Object target) {
            #if UNITY_EDITOR

            UnityEditor.EditorUtility.SetDirty(target);
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(target);

            if (target is not GameObject go) {
                if (target is not Component cpn) return;
                go = cpn.gameObject;
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(go.scene);

            #endif
        }

        [Conditional("UNITY_EDITOR")]
        public void MarkDirty(params Object[] targets) {
            foreach (var target in targets) MarkDirty(target);
        }

        public void SetParentUndo(Transform child, Transform par) {
            #if UNITY_EDITOR
            UnityEditor.Undo.SetTransformParent(child, par, $"{child.name} set parent to {par.name}");
            #else
            child.SetParent(par);
            #endif
        }

        public GameObject CreateGameObjectUndo(string name) {
            var go = new GameObject(name);

            #if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, $"Create game object {name}");
            #endif
            
            return go;
        }

        public TComponent AddComponentUndo<TComponent>(GameObject go) where TComponent : Component {
            TComponent cpn;

            #if UNITY_EDITOR
            cpn = UnityEditor.Undo.AddComponent<TComponent>(go);
            #else
            cpn = go.AddComponent<TComponent>();
            #endif

            return cpn;
        }

        #endregion

        public float Evaluate(Ease ease, float time, float duration = 1) => EaseManager.Evaluate(
            easeType: ease
          , customEase: null
          , time: time
          , duration: duration
          , overshootOrAmplitude: DOTween.defaultEaseOvershootOrAmplitude
          , period: DOTween.defaultEasePeriod);
        
        /// <summary>
        /// Format: 1 + (-1)^(<see cref="power"/> + 1) * (<see cref="time"/> - 1)^<see cref="power"/> <br/>
        /// ______/ <br/>
        /// _____/  <br/>
        /// ____/   <br/>
        /// ___/    <br/>
        /// __/     <br/>
        /// _/      <br/>
        /// 0----->1<br/>
        /// </summary>
        public float Evaluate(float time, uint power) {
            return 1 + Pow(-1, power + 1) * Pow(time - 1, power);
        }
    }
}