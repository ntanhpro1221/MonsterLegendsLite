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

            if (!CheckOnBattleEndAndExecute()) {
                ExecuteBattleLoop();
            }
        }

        private void LoadBootDataThenDelete() {
            var bootData   = Battle_BootData.Ins;
            var gameLocDef = DataManager.Ins.GameLocDef;

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
                var slotId = 0;
                foreach (var monster in insData) {
                    if (monster == null) continue;

                    var ins = Instantiate(gameLocDef.Monsters[monster.Id].PrefabBattleScene, slots[slotId]);
                    resultHolder.Add(monster.InsId, ins);

                    ins.Initialize(monster, faceDir);

                    ++slotId;
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
                    RemoveAllMonsters(teamLeft, dieMonsters);
                    RemoveAllMonsters(teamRight, dieMonsters);

                    if (CheckOnBattleEndAndExecute()) return;

                    uiTurnManager.NextTurn(dieMonsters, ExecuteBattleLoop);
                });
            });
        }

        private bool CheckOnBattleEndAndExecute() {
            if (teamLeft.Count == 0) {
                onBattleEnd?.Invoke(isWin: false);
                return true;
            }

            if (teamRight.Count == 0) {
                onBattleEnd?.Invoke(isWin: true);
                return true;
            }

            return false;
        }

        private void RemoveAllMonsters(Dictionary<string, Battle_Monster> dict, List<Battle_Monster> targets) {
            foreach (var (key, _) in dict.Where(i => targets.Contains(i.Value)).ToList()) dict.Remove(key);
        }
    }
}