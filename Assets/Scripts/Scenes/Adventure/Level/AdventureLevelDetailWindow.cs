using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class AdventureLevelDetailWindow : PopupWindow {
        [SerializeField, Required]
        private TextMeshProUGUI expTxt, goldTxt, foodTxt;

        [SerializeField, Required]
        private MonsterTeamSlots<Arena_Monster> monsters;

        [SerializeField, Required]
        private Button fightBtn;

        public static AdventureLevelDetailWindow Show(AdventureLevelDetailWindow prefab, AdventureLevelData levelData, int levelIndex) {
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: levelData.Name.ToUpper()
              , content: null
              , onDoneClose: null);

            window.SetAllData(levelData, levelIndex);

            return window;
        }

        private void SetAllData(AdventureLevelData levelData, int levelIndex) {
            expTxt.text  = UtilFuncs.Ins.ToStrResource(levelData.RewardExp);
            goldTxt.text = UtilFuncs.Ins.ToStrResource(levelData.RewardGold);
            foodTxt.text = UtilFuncs.Ins.ToStrResource(levelData.RewardFood);

            for (int i = 0; i < monsters.Count; ++i) monsters[i].SetAllData(levelData.Monsters[i]);

            UtilFuncs.Ins.SetListener(fightBtn, () => {
                Battle_BootData.OnBattleEnd onBattleEnd = _ => SceneManager.LoadScene("AdventureScene");

                Battle_BootData.InsAutoSpawn.SetData(
                    exitWarning: "Are you sure you want to exit this battle?"
                  , onExit: () => onBattleEnd.Invoke(isWin: false)
                  , onBattleEnd: isWin => {
                        NotificationWindow.Show(
                            title: isWin ? "WIN" : "LOSE"
                          , content: isWin
                                ? ($"You defeated level {levelData.Name}, you received:"
                                  + $"\n- {UtilFuncs.Ins.ToStrResource(levelData.RewardExp)} exp"
                                  + $"\n- {UtilFuncs.Ins.ToStrResource(levelData.RewardGold)} gold"
                                  + $"\n- {UtilFuncs.Ins.ToStrResource(levelData.RewardFood)} food")
                                : "You are lose"
                          , onDoneClose: () => onBattleEnd.Invoke(isWin: isWin));

                        if (!isWin) return;

                        UtilFuncs.Ins.DelayedCall_Second(CoroutineRunner.InsAutoSpawn, 0, () => FloatingTextPool.Ins
                            .ShowAtCenterScreen(FloatingTextId.ExpChanged)
                            .SetTextChange(levelData.RewardExp));

                        UtilFuncs.Ins.DelayedCall_Second(CoroutineRunner.InsAutoSpawn, .5f, () => FloatingTextPool.Ins
                            .ShowAtCenterScreen(FloatingTextId.GoldChange)
                            .SetTextChange(levelData.RewardGold));

                        UtilFuncs.Ins.DelayedCall_Second(CoroutineRunner.InsAutoSpawn, 1, () => FloatingTextPool.Ins
                            .ShowAtCenterScreen(FloatingTextId.FoodChange)
                            .SetTextChange(levelData.RewardFood));

                        DataManager.Ins.UpdateData_DefeatBattleEnd(levelData, levelIndex, out bool levelUp);

                        if (!levelUp) return;

                        NotificationWindow.Show(
                            title: "LEVEL UP"
                          , content: $"Your current level: {DataManager.Ins.UserInsData.Level}");
                    }
                  , teamLeft: DataManager.Ins.UserInsData.GetAdventureTeamData()
                  , teamRight: levelData.Monsters);

                SceneManager.LoadScene("BattleScene");
            });
        }
    }
}