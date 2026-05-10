using System;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Utils;
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

    [ForwardAttributesTo(
        nameof(skill_0)
      , nameof(skill_1)
      , nameof(skill_2)
      , nameof(skill_3))]
    [Serializable]
    public class MonsterSkillList<T> : FixedList<MonsterSkillList<T>, T> {
        [SerializeField]
        private T
            skill_0
          , skill_1
          , skill_2
          , skill_3;

        public override int Count => 4;

        public MonsterSkillList() { }

        public MonsterSkillList(T value) => WithAll(value);

        protected override ref T GetValueRef(int id) {
            switch (id) {
                case 0: return ref skill_0;
                case 1: return ref skill_1;
                case 2: return ref skill_2;
                case 3: return ref skill_3;

                default: throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }
        }
    }
}