using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NGDtuanh.Utils;
using UnityEngine;

namespace NGDtuanh.Collections {
    [Serializable]
    [ForwardAttributesTo(nameof(_Values))]
    public class EnumMap<TKey, TValue> :
        IReadOnlyDictionary<TKey, TValue>
      , ISerializationCallbackReceiver
        where TKey : struct, Enum {
        [SerializeField] internal TKey[]                _Keys;
        [SerializeField] internal EnumMapItem<TValue>[] _Values;

        private Dictionary<TKey, int> _HashedKeys = new();

        public int                 Count  => _Keys.Length;
        public IEnumerable<TKey>   Keys   => _Keys;
        public IEnumerable<TValue> Values => _Values.Select(item => item.Value);

        public EnumMap() {
            _Keys   = (TKey[])Enum.GetValues(typeof(TKey));
            _Values = new EnumMapItem<TValue>[_Keys.Length];
            ReHashKeys();

            #if UNITY_EDITOR
            _KeyNames = new string[_Keys.Length];
            for (int i = 0; i < _Keys.Length; ++i)
                _KeyNames[i] = _Keys[i].ToString();
            #endif
        }

        public EnumMap(IReadOnlyCollection<KeyValuePair<TKey, TValue>> source) : this() {
            foreach (var (key, value) in source) {
                if (!_HashedKeys.ContainsKey(key)) throw new InvalidKeyException();
                this[key] = value;
            }
        }

        public Dictionary<TKey, TValue> ToDictionary() {
            var dict = new Dictionary<TKey, TValue>();
            for (int i = 0; i < Count; ++i) dict.Add(_Keys[i], _Values[i]);
            return dict;
        }

        public bool ContainsKey(TKey key) => true; // Yes, of course.

        public bool TryGetValue(TKey key, out TValue value) {
            try {
                value = this[key];
                return true;
            } catch {
                value = default;
                return false;
            }
        }

        public TValue this[TKey key] {
            get {
                SafetyHashKey(key);

                return _Values[_HashedKeys[key]];
            }
            set {
                SafetyHashKey(key);

                if (_Values[_HashedKeys[key]] == null)
                    _Values[_HashedKeys[key]] = new(value);
                else
                    _Values[_HashedKeys[key]].Value = value;
            }
        }

        private void ReHashKeys() {
            _HashedKeys.Clear();
            for (int i = 0; i < _Keys.Length; ++i)
                _HashedKeys.Add(_Keys[i], i);
        }

        private void SafetyHashKey(TKey key) {
            if (_HashedKeys == null
             || !_HashedKeys.ContainsKey(key))
                ReHashKeys();
        }

        [SerializeField] internal string[] _KeyNames;
        [SerializeField] internal bool     _IsKeyChanged;
        
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if (_IsKeyChanged || _HashedKeys.Count != _Keys.Length) {
                ReHashKeys();
                _IsKeyChanged = false;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            for (int i = 0; i < Count; ++i)
                yield return new(_Keys[i], _Values[i]);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class InvalidKeyException : Exception {
            public InvalidKeyException() : base("NGDtuanh EnumMap: Given key is not correspond to true enum key") { }
        }
        }
}