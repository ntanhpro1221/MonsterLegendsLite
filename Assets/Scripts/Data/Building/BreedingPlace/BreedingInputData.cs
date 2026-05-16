using System;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public struct BreedingInputData : IEquatable<BreedingInputData> {
        [HideLabel]
        public MonsterId First;

        [HideLabel]
        public MonsterId Second;

        public BreedingInputData(
            MonsterId first
          , MonsterId second) {
            First  = first;
            Second = second;
        }

        public void Deconstruct(
            out MonsterId first
          , out MonsterId second) {
            first  = First;
            second = Second;
        }

        public override int GetHashCode() => First < Second
            ? HashCode.Combine(First, Second)
            : HashCode.Combine(Second, First);

        public bool Equals(BreedingInputData other) =>
            (First == other.First && Second == other.Second)
         || (First == other.Second && Second == other.First);

        public override bool Equals(object obj) =>
            obj is BreedingInputData other
         && Equals(other);

        public static bool operator ==(BreedingInputData left, BreedingInputData right) => left.Equals(right);
        public static bool operator !=(BreedingInputData left, BreedingInputData right) => !left.Equals(right);

        public override string ToString() => $"<{First.ToString()} | {Second.ToString()}>";
    }
}