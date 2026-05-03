using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class MonsterSkillList : MonsterSkillList<int> {
        [OnInspectorInit]
        private void OnInit() {
            if (IsAllEqual(0)) ResetAll();
        }
        
        [Button]
        private void ResetAll() {
            WithAll(-1).With(0, 0);
        }
    }
    
    [Serializable]
    public class MonsterSkillList<T> : IReadOnlyList<T> {
        [SerializeField]
        private T skill_0, skill_1, skill_2, skill_3;

        public int Count => 4;

        public MonsterSkillList() { }

        public MonsterSkillList(T value) => WithAll(value);

        public MonsterSkillList<T> With(int id, T value) {
            this[id] = value;
            return this;
        }

        public MonsterSkillList<T> WithAll(T value) {
            for (int i = 0; i < Count; ++i) this[i] = value;
            return this;
        }

        public bool IsAllEqual(T value) {
            foreach (var item in this)
                if (!EqualityComparer<T>.Default.Equals(item, value))
                    return false;
            return true;
        }

        public T this[int id] {
            get => GetValueRef(id);
            set => GetValueRef(id) = value;
        }

        private ref T GetValueRef(int id) {
            switch (id) {
                case 0: return ref skill_0;
                case 1: return ref skill_1;
                case 2: return ref skill_2;
                case 3: return ref skill_3;

                default: throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }
        }

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < Count; ++i) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}