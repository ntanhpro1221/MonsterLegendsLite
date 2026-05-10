using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Arena_Battle : MonoBehaviourExt {
        [SerializeField, Required]
        private Arena_TeamAttack teamAttack;

        [SerializeField, Required]
        private Arena_TeamDefense teamDefense;

        [SerializeField, Required]
        private Button fightBtn;

        public void Initialize() {
            teamAttack.Initialize();
            teamDefense.Initialize();
            utils.SetListener(fightBtn, OnFight);
        }

        private void OnFight() {
            var ally  = DataManager.Ins.UserInsData;
            var enemy = DataManager.Ins.GetUserListTest()[0];

            Battle_BootData.OnBattleEnd onBattleEnd = isWin => {
                var (winner, loser) = (ally, enemy);
                if (!isWin) new UtilFuncs().Swap(ref winner, ref loser);

                CalculateEloAfterBattle(winner.Elo, loser.Elo, out var winnerDeltaElo, out var loserDeltaElo);

                DataManager.Ins.UpdateData_EloAfterBattleTest(winner, loser, winnerDeltaElo, loserDeltaElo);
                
                EventDispatcher.PostEvent(EventId.AnyUserEloChanged);

                Arena_BootData.InsAutoSpawn.SetData_EndBattleDeltaElo(isWin ? winnerDeltaElo : loserDeltaElo);
                
                SceneManager.LoadScene("ArenaScene");
            };

            Battle_BootData.InsAutoSpawn.SetData(
                exitWarning: "Are you sure you want to exit this battle?\nYou will lose the match and your ELO!"
              , onExit: () => onBattleEnd.Invoke(isWin: false)
              , onBattleEnd: isWin => NotificationWindow.Show(
                    title: isWin ? "WIN" : "LOSE"
                  , content: isWin ? "You are win" : "You are lose"
                  , onDoneClose: () => onBattleEnd.Invoke(isWin: isWin))
              , teamLeft: ToInsDataList(ally.ArenaTeamAttack)
              , teamRight: ToInsDataList(enemy.ArenaTeamDefense));

            SceneManager.LoadScene("BattleScene");
        }

        private List<MonsterInsData> ToInsDataList(MonsterTeamSlots<string> insIdList) {
            return insIdList.Select(id => DataManager.Ins.UserInsData.Monsters.First(monster => monster.InsId == id)).ToList();
        }

        private static void CalculateEloAfterBattle(int winnerElo, int loserElo, out int winnerDeltaElo, out int loserDeltaElo) {
            const int maxDelta = 25;

            winnerDeltaElo = Mathf.Max(1, Mathf.RoundToInt(StandardDeltaEloAlgorithm(maxDelta, winnerElo, loserElo)));

            loserDeltaElo = -winnerDeltaElo;

            static float StandardDeltaEloAlgorithm(int maxDelta, int winnerElo, int loserElo) {
                return maxDelta * (1f - 1f / (1f + Mathf.Pow(10f, (loserElo - winnerElo) / 400f)));
            }
        }
    }
}