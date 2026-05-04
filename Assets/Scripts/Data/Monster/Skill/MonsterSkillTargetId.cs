namespace MonsterLegendsLite.Data {
    public enum MonsterSkillTargetId {
        SingleEnemy
      , MultipleEnemies
      , SingleAlly
      , MultipleAllies
    }

    public static class MonsterSkillTargetExtensions {
        public static bool IsTargetEnemy(this MonsterSkillTargetId self) {
            return self is
                MonsterSkillTargetId.SingleEnemy
             or MonsterSkillTargetId.MultipleEnemies;
        }

        public static bool IsSingle(this MonsterSkillTargetId self) {
            return self is
                MonsterSkillTargetId.SingleEnemy
             or MonsterSkillTargetId.SingleAlly;
        }
    }
}