using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Battle_SceneManager : SceneSingleton<Battle_SceneManager> {
        [SerializeField, Required]
        private Battle_UserSkillSelector userSkillSelector;

        [SerializeField, Required]
        private Battle_BotSkillSelector botSkillSelector;

        [SerializeField, Required]
        private Battle_TurnManager turnManager;

        [SerializeField]
        private List<Transform> slotsLeft, slotsRight;

        private readonly Dictionary<string, Battle_Monster> teamLeft = new(), teamRight = new();

        private void Start() {
            LoadBootDataThenDelete();

            userSkillSelector.Initialize(teamLeft, teamRight);
            botSkillSelector.Initialize(teamLeft, teamRight);
            turnManager.Initialize(teamLeft, teamRight);
            
            ExecuteBattleLoop();
        }

        private void LoadBootDataThenDelete() {
            var bootData   = Battle_BootData.Ins;
            var gameLocDef = DataManager.Ins.GameLocDefData;

            SpawnMonsters(bootData.teamLeft, slotsLeft, teamLeft, HorDirection.Right);
            SpawnMonsters(bootData.teamRight, slotsRight, teamRight, HorDirection.Left);

            Destroy(bootData.gameObject);

            void SpawnMonsters(
                List<MonsterInsData> insData
              , List<Transform> slots
              , Dictionary<string, Battle_Monster> resultHolder
              , HorDirection faceDir) {
                for (int i = 0; i < insData.Count && i < slots.Count; i++) {
                    var ins = Instantiate(gameLocDef.Monster[insData[i].Id].PrefabBattleScene, slots[i]);
                    resultHolder.Add(insData[i].InsId, ins);

                    ins.Initialize(insData[i], faceDir);
                }
            }
        }

        private void ExecuteBattleLoop() {
            var curMonster = turnManager.GetTurn();

            Battle_SkillSelector skillSelector = teamLeft.ContainsKey(curMonster.insData.InsId)
                ? userSkillSelector
                : botSkillSelector;

            skillSelector.SelectSkill(curMonster, (skill, target) => {
                curMonster.ApplySkill(skill, target, () => {
                    turnManager.NextTurn();
                    ExecuteBattleLoop();
                });
            });
        }
    }
}