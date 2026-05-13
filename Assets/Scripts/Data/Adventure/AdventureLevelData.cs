using System;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class AdventureLevelData {
        public string Name;
        public int RewardGold;
        public int RewardFood;
        public int RewardExp;
        public MonsterTeamSlots<MonsterInsData> Monsters;
    }
}