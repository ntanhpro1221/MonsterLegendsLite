using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;

namespace MonsterLegendsLite {
    public class Battle_TurnManager : MonoBehaviourExt {
        private readonly List<Battle_Monster> turnCycle = new();

        private int turnId;

        public void Initialize(Dictionary<string, Battle_Monster> teamLeft, Dictionary<string, Battle_Monster> teamRight) {
            turnCycle.AddRange(teamLeft.Values.Concat(teamRight.Values).OrderBy(static item => -item.GetStat(MonsterStatId.Speed)));
        }

        public Battle_Monster GetTurn() {
            return turnCycle[turnId];
        }
        
        public void NextTurn() {
            turnId = (turnId + 1) % turnCycle.Count;
        }
    }
}