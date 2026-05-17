using System;
using System.Linq;
using System.Threading.Tasks;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
            utils.SetListener(fightBtn, () => _ = OnFightAsync());
        }

        private async Task OnFightAsync() {
            try {
                LoadingIcon.Ins.Show(blockInteract: true);

                var ally        = DataManager.Ins.UserWithId;
                var allUsers    = await DataManager.Ins.GetAllUsersAsync();
                var randomId    = Random.Range(0, allUsers.Count);
                var randomEnemy = allUsers.ElementAt(randomId);
                if (randomEnemy.Key == ally.Id) {
                    randomId    = (randomId + 1) % allUsers.Count;
                    randomEnemy = allUsers.ElementAt(randomId);
                }

                var enemy = randomEnemy.Value.WithId(randomEnemy.Key);

                Debug.Log($"Start arena: {ally.Data.Name} vs {enemy.Data.Name}");

                Battle_BootData.OnBattleEnd onBattleEnd = isWin => {
                    var (winner, loser) = (ally, enemy);
                    if (!isWin) new UtilFuncs().Swap(ref winner, ref loser);

                    CalculateEloAfterBattle(winner.Data.Elo, loser.Data.Elo, out var winnerDeltaElo, out var loserDeltaElo);

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
                  , teamLeft: ally.Data.GetArenaTeamAttackData()
                  , teamRight: enemy.Data.GetArenaTeamDefenseData());

                SceneManager.LoadScene("BattleScene");
            } catch (Exception e) {
                utils.LogExceptionWithWindow(e);
            } finally {
                LoadingIcon.Ins.Hide();
            }
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