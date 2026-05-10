using System;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Utils;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [ForwardAttributesTo(
        nameof(item_0)
      , nameof(item_1)
      , nameof(item_2))]
    [Serializable]
    public class MonsterTeamSlots<T> : FixedList<MonsterSkillList<T>, T> {
        [SerializeField]
        private T
            item_0
          , item_1
          , item_2;

        public override int Count => 3;

        public MonsterTeamSlots() { }

        public MonsterTeamSlots(T value) => WithAll(value);

        protected override ref T GetValueRef(int id) {
            switch (id) {
                case 0: return ref item_0;
                case 1: return ref item_1;
                case 2: return ref item_2;

                default: throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }
        }
    }
}