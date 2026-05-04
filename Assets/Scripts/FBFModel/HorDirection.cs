namespace MonsterLegendsLite {
    public enum HorDirection {
        Left
      , Right
    }

    public static class HorDirectionExtensions {
        public static HorDirection Flip(this HorDirection self) {
            return self == HorDirection.Left ? HorDirection.Right : HorDirection.Left;
        }
    }
}