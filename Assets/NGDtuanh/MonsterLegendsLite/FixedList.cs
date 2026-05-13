using System.Collections;
using System.Collections.Generic;

namespace NGDtuanh.MonsterLegendsLite {
    public abstract class FixedList<TList, TValue> :
        IReadOnlyList<TValue>
        where TList : FixedList<TList, TValue> {
        public abstract int Count { get; }

        public TList With(int id, TValue value) {
            this[id] = value;
            return (TList)this;
        }

        public TList WithAll(TValue value) {
            for (int i = 0; i < Count; ++i) this[i] = value;
            return (TList)this;
        }

        public TList WithAll(IEnumerable<TValue> values) {
            int id = 0;

            foreach (var value in values) this[id++] = value;

            return (TList)this;
        }

        public bool IsAllEqual(TValue value) {
            foreach (var item in this)
                if (!EqualityComparer<TValue>.Default.Equals(item, value))
                    return false;
            return true;
        }

        public TValue this[int id] {
            get => GetValueRef(id);
            set => GetValueRef(id) = value;
        }

        protected abstract ref TValue GetValueRef(int id);

        public IEnumerator<TValue> GetEnumerator() {
            for (int i = 0; i < Count; ++i) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}