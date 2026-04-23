using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
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

        /// <inheritdoc cref="UnityEditor.EditorGUIUtility.PingObject(Object)"/>
        [Conditional("UNITY_EDITOR")]
        public void PingObject(Object obj) {
            #if UNITY_EDITOR
            UnityEditor.EditorGUIUtility.PingObject(obj);
            #endif
        }
        
        public string GetPath(Object obj) {
            if (obj == null) return null;

            if (obj is Component cpn) return $"{GetPath(cpn.gameObject)}<{cpn.GetType().Name}>";
            
            #if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(obj)) return UnityEditor.AssetDatabase.GetAssetPath(obj);
            #endif

            if (obj is GameObject go) return $"{GetScenePrefixPath(go)}{GetPathTF(go.transform)}";

            return $"[Unknown]/{obj.name}";
        }

        private string GetPathTF(Transform tf) {
            if (tf == null) return string.Empty;
            var result = tf.name;
            while ((tf = tf.parent) != null)
                result = $"{tf.name}/{result}";
            return result;
        }

        private string GetScenePrefixPath(GameObject go) {
            #if UNITY_EDITOR
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(go) != null) return string.Empty;
            #endif
            return (go.scene.IsValid() ? go.scene.name : "[Unknown]") + "/";
        }

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
        
        #region EVENT MODIFY

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddPersistentListener(UnityEventBase)"/>
        [Conditional("UNITY_EDITOR")]
        public void AddEvent(Object holder, UnityEventBase unityEvent) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddPersistentListener(UnityEvent, UnityAction)"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddEvent(Object holder, UnityEvent unityEvent, UnityAction call, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddPersistentListener{T0}(UnityEvent{T0}, UnityAction{T0})"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddEvent<T0>(Object holder, UnityEvent<T0> unityEvent, UnityAction<T0> call, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddPersistentListener{T0,T1}(UnityEvent{T0,T1}, UnityAction{T0,T1})"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddEvent<T0, T1>(Object holder, UnityEvent<T0, T1> unityEvent, UnityAction<T0, T1> call, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddPersistentListener{T0,T1,T2}(UnityEvent{T0,T1,T2}, UnityAction{T0,T1,T2})"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddEvent<T0, T1, T2>(Object holder, UnityEvent<T0, T1, T2> unityEvent, UnityAction<T0, T1, T2> call, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddPersistentListener{T0,T1,T2,T3}(UnityEvent{T0,T1,T2,T3}, UnityAction{T0,T1,T2,T3})"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddEvent<T0, T1, T2, T3>(Object holder, UnityEvent<T0, T1, T2, T3> unityEvent, UnityAction<T0, T1, T2, T3> call, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        // ── Remove ────────────────────────────────────────────────────────────

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RemovePersistentListener(UnityEventBase, int)"/>
        [Conditional("UNITY_EDITOR")]
        public void RemoveEvent(Object holder, UnityEventBase unityEvent, int index) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(unityEvent, index);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RemovePersistentListener(UnityEventBase, UnityAction)"/>
        [Conditional("UNITY_EDITOR")]
        public void RemoveEvent(Object holder, UnityEventBase unityEvent, UnityAction call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RemovePersistentListener{T0}(UnityEventBase, UnityAction{T0})"/>
        [Conditional("UNITY_EDITOR")]
        public void RemoveEvent<T0>(Object holder, UnityEventBase unityEvent, UnityAction<T0> call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RemovePersistentListener{T0,T1}(UnityEventBase, UnityAction{T0,T1})"/>
        [Conditional("UNITY_EDITOR")]
        public void RemoveEvent<T0, T1>(Object holder, UnityEventBase unityEvent, UnityAction<T0, T1> call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RemovePersistentListener{T0,T1,T2}(UnityEventBase, UnityAction{T0,T1,T2})"/>
        [Conditional("UNITY_EDITOR")]
        public void RemoveEvent<T0, T1, T2>(Object holder, UnityEventBase unityEvent, UnityAction<T0, T1, T2> call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RemovePersistentListener{T0,T1,T2,T3}(UnityEventBase, UnityAction{T0,T1,T2,T3})"/>
        [Conditional("UNITY_EDITOR")]
        public void RemoveEvent<T0, T1, T2, T3>(Object holder, UnityEventBase unityEvent, UnityAction<T0, T1, T2, T3> call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        // ── Register ──────────────────────────────────────────────────────────

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterPersistentListener(UnityEvent, int, UnityAction)"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterEvent(Object holder, UnityEvent unityEvent, int index, UnityAction call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterPersistentListener(unityEvent, index, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterPersistentListener{T0}(UnityEvent{T0}, int, UnityAction{T0})"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterEvent<T0>(Object holder, UnityEvent<T0> unityEvent, int index, UnityAction<T0> call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterPersistentListener(unityEvent, index, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterPersistentListener{T0,T1}(UnityEvent{T0,T1}, int, UnityAction{T0,T1})"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterEvent<T0, T1>(Object holder, UnityEvent<T0, T1> unityEvent, int index, UnityAction<T0, T1> call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterPersistentListener(unityEvent, index, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterPersistentListener{T0,T1,T2}(UnityEvent{T0,T1,T2}, int, UnityAction{T0,T1,T2})"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterEvent<T0, T1, T2>(Object holder, UnityEvent<T0, T1, T2> unityEvent, int index, UnityAction<T0, T1, T2> call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterPersistentListener(unityEvent, index, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterPersistentListener{T0,T1,T2,T3}(UnityEvent{T0,T1,T2,T3}, int, UnityAction{T0,T1,T2,T3})"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterEvent<T0, T1, T2, T3>(Object holder, UnityEvent<T0, T1, T2, T3> unityEvent, int index, UnityAction<T0, T1, T2, T3> call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterPersistentListener(unityEvent, index, call);
            MarkDirty(holder);
            #endif
        }

        // ── Unregister ────────────────────────────────────────────────────────

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.UnregisterPersistentListener(UnityEventBase, int)"/>
        [Conditional("UNITY_EDITOR")]
        public void UnregisterEvent(Object holder, UnityEventBase unityEvent, int index) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.UnregisterPersistentListener(unityEvent, index);
            MarkDirty(holder);
            #endif
        }

        // ── Void (param) ───────────────────────────────────────────────

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(UnityEventBase, UnityAction)"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddVoidEvent(Object holder, UnityEventBase unityEvent, UnityAction call, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(unityEvent, call);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterVoidPersistentListener(UnityEventBase, int, UnityAction)"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterVoidEvent(Object holder, UnityEventBase unityEvent, int index, UnityAction call) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterVoidPersistentListener(unityEvent, index, call);
            MarkDirty(holder);
            #endif
        }

        // ── Int (param) ────────────────────────────────────────────────

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddIntPersistentListener(UnityEventBase, UnityAction{int}, int)"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddIntEvent(Object holder, UnityEventBase unityEvent, UnityAction<int> call, int argument, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddIntPersistentListener(unityEvent, call, argument);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterIntPersistentListener(UnityEventBase, int, UnityAction{int}, int)"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterIntEvent(Object holder, UnityEventBase unityEvent, int index, UnityAction<int> call, int argument) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterIntPersistentListener(unityEvent, index, call, argument);
            MarkDirty(holder);
            #endif
        }

        // ── Float (param) ──────────────────────────────────────────────

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddFloatPersistentListener(UnityEventBase, UnityAction{float}, float)"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddFloatEvent(Object holder, UnityEventBase unityEvent, UnityAction<float> call, float argument, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddFloatPersistentListener(unityEvent, call, argument);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterFloatPersistentListener(UnityEventBase, int, UnityAction{float}, float)"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterFloatEvent(Object holder, UnityEventBase unityEvent, int index, UnityAction<float> call, float argument) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterFloatPersistentListener(unityEvent, index, call, argument);
            MarkDirty(holder);
            #endif
        }

        // ── Bool (param) ───────────────────────────────────────────────

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(UnityEventBase, UnityAction{bool}, bool)"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddBoolEvent(Object holder, UnityEventBase unityEvent, UnityAction<bool> call, bool argument, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(unityEvent, call, argument);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterBoolPersistentListener(UnityEventBase, int, UnityAction{bool}, bool)"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterBoolEvent(Object holder, UnityEventBase unityEvent, int index, UnityAction<bool> call, bool argument) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterBoolPersistentListener(unityEvent, index, call, argument);
            MarkDirty(holder);
            #endif
        }

        // ── String (param) ─────────────────────────────────────────────

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddStringPersistentListener(UnityEventBase, UnityAction{string}, string)"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddStringEvent(Object holder, UnityEventBase unityEvent, UnityAction<string> call, string argument, bool unique = true) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(unityEvent, call, argument);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterStringPersistentListener(UnityEventBase, int, UnityAction{string}, string)"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterStringEvent(Object holder, UnityEventBase unityEvent, int index, UnityAction<string> call, string argument) {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterStringPersistentListener(unityEvent, index, call, argument);
            MarkDirty(holder);
            #endif
        }

        // ── Object (param) ─────────────────────────────────────────────

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.AddObjectPersistentListener{T}(UnityEventBase, UnityAction{T}, T)"/>
        /// <param name="unique">Remove event before add</param>
        [Conditional("UNITY_EDITOR")]
        public void AddObjectEvent<T>(Object holder, UnityEventBase unityEvent, UnityAction<T> call, T argument, bool unique = true) where T : Object {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            if (unique) RemoveEvent(holder, unityEvent, call);
            UnityEditor.Events.UnityEventTools.AddObjectPersistentListener(unityEvent, call, argument);
            MarkDirty(holder);
            #endif
        }

        /// <inheritdoc cref="UnityEditor.Events.UnityEventTools.RegisterObjectPersistentListener{T}(UnityEventBase, int, UnityAction{T}, T)"/>
        [Conditional("UNITY_EDITOR")]
        public void RegisterObjectEvent<T>(Object holder, UnityEventBase unityEvent, int index, UnityAction<T> call, T argument) where T : Object {
            #if UNITY_EDITOR
            RecordForUndo(holder);
            UnityEditor.Events.UnityEventTools.RegisterObjectPersistentListener(unityEvent, index, call, argument);
            MarkDirty(holder);
            #endif
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