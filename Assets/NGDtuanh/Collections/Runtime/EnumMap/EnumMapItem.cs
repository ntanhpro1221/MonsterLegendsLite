using System;
using NGDtuanh.Utils;
using UnityEngine;

namespace NGDtuanh.Collections {
    [Serializable]
    [ForwardAttributesTo(nameof(value))]
    internal class EnumMapItem<TValue> : IEquatable<TValue> {
        [SerializeField]
        public TValue value;

        public virtual TValue Value {
            get => value;
            set => this.value = value;
        }

        public EnumMapItem() { }
        public EnumMapItem(in TValue value) => this.value = value;

        public bool Equals(TValue other) => value == null ? other == null : value.Equals(other);
        public override string ToString() => value.ToString();
        public static implicit operator TValue(EnumMapItem<TValue> value) => value == null ? default : value.Value;
    }
}