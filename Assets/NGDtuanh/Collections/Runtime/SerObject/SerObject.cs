using System;
using NGDtuanh.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NGDtuanh.Collections {
    [Serializable]
    [ForwardAttributesTo(nameof(value))]
    public struct SerObject<TValue> :
        IEquatable<SerObject<TValue>>
      , IEquatable<TValue>
        where TValue : class {
        [SerializeField]
        internal Object value;

        public TValue Value => value as TValue;
        public bool IsNull => value == null;

        public SerObject(TValue value) {
            this.value = value as Object;
        }

        public override string ToString() => value == null ? "Null" : value.ToString();
        public override int GetHashCode() => value == null ? 0 : value.GetHashCode();

        #region Equal Operators

        public static bool operator ==(SerObject<TValue> left, SerObject<TValue> right) => left.Equals(right);
        public static bool operator !=(SerObject<TValue> left, SerObject<TValue> right) => !left.Equals(right);
        public static bool operator ==(SerObject<TValue> left, TValue right) => left.Equals(right);
        public static bool operator !=(SerObject<TValue> left, TValue right) => !left.Equals(right);
        public static bool operator ==(TValue left, SerObject<TValue> right) => right.Equals(left);
        public static bool operator !=(TValue left, SerObject<TValue> right) => !right.Equals(left);

        public bool Equals(SerObject<TValue> other) => value == other.value;
        public bool Equals(TValue other) => value == other as Object;

        public override bool Equals(object obj) => obj switch {
            SerObject<TValue> otherSerObj => Equals(otherSerObj)
          , TValue otherObj               => Equals(otherObj)
          , _                             => false
        };

        #endregion
    }
}