using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Battle_SceneManager : SceneSingleton<Battle_SceneManager> {
        [SerializeField, Required]
        private Button exitBtn;
        
        [SerializeField, Required]
        private Battle_UI_TurnManager uiTurnManager;

        [SerializeField, Required]
        private Battle_UserSkillSelector userSkillSelector;

        [SerializeField, Required]
        private Battle_BotSkillSelector botSkillSelector;

        [SerializeField]
        private List<Transform> slotsLeft, slotsRight;

        private Battle_BootData.OnBattleEnd onBattleEnd;

        private readonly Dictionary<string, Battle_Monster> teamLeft = new(), teamRight = new();

        protected override void Initialize() {
            base.Initialize();
            
            LoadBootDataThenDelete();

            uiTurnManager.Initialize(teamLeft, teamRight);
            userSkillSelector.Initialize(teamLeft, teamRight);
            botSkillSelector.Initialize(teamRight, teamLeft);

            ExecuteBattleLoop();
        }

        private void LoadBootDataThenDelete() {
            var bootData   = Battle_BootData.Ins;
            var gameLocDef = DataManager.Ins.GameLocDefData;

            exitBtn.onClick.AddListener(() => YesNoWindow.Show(
                title: "EXIT BATTLE"
              , content: bootData.exitWarning
              , yesCallback: bootData.onExit));
            
            onBattleEnd = bootData.onBattleEnd;
            
            SpawnMonsters(bootData.TeamLeft, slotsLeft, teamLeft, HorDirection.Right);
            SpawnMonsters(bootData.TeamRight, slotsRight, teamRight, HorDirection.Left);

            Destroy(bootData.gameObject);

            void SpawnMonsters(
                IReadOnlyList<MonsterInsData> insData
              , IReadOnlyList<Transform> slots
              , Dictionary<string, Battle_Monster> resultHolder
              , HorDirection faceDir) {
                for (int i = 0; i < insData.Count && i < slots.Count; i++) {
                    var ins = Instantiate(gameLocDef.Monsters[insData[i].Id].PrefabBattleScene, slots[i]);
                    resultHolder.Add(insData[i].InsId, ins);

                    ins.Initialize(insData[i], faceDir);
                }
            }
        }

        private void ExecuteBattleLoop() {
            var curMonster = uiTurnManager.GetTurn();

            Battle_SkillSelector skillSelector = teamLeft.ContainsKey(curMonster.insData.InsId)
                ? userSkillSelector
                : botSkillSelector;

            skillSelector.SelectSkill(curMonster, (isRecharge, skill, targets) => {
                foreach (var item in curMonster.SkillList) item?.DecreaseCooldown();
                
                curMonster.ApplyTurn(isRecharge, skill, targets, dieMonsters => {
                    if (RemoveAllMonsters(teamLeft, dieMonsters)) {
                        onBattleEnd?.Invoke(isWin: false);
                        return;
                    }

                    if (RemoveAllMonsters(teamRight, dieMonsters)) {
                        onBattleEnd?.Invoke(isWin: true);
                        return;
                    }

                    uiTurnManager.NextTurn(dieMonsters, ExecuteBattleLoop);
                });
            });
        }

        private bool RemoveAllMonsters(Dictionary<string, Battle_Monster> dict, List<Battle_Monster> targets) {
            foreach (var (key, _) in dict.Where(i => targets.Contains(i.Value)).ToList()) dict.Remove(key);
            return dict.Count == 0;
        }
    }
}