using System;
using System.Collections.Generic;
using System.Linq;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    /// TODO: Load, save data remotely
    public class DataManager : Singleton<DataManager> {
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), Required]
        private GameDefDataSO gameDefDataSO;

        public GameDefData GameDefData => gameDefDataSO.Data;

        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), Required]
        private GameLocDefDataSO gameLocDefDataSO;

        public GameLocDefData GameLocDefData => gameLocDefDataSO.Data;

        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), Required]
        private UserInsDataSO userInsDataSO;

        public UserInsData UserInsData => userInsDataSO.Data;

        private void SaveAllSO() {
            UtilFuncs.Ins.SaveSO(
                gameDefDataSO
              , gameLocDefDataSO
              , userInsDataSO);
        }

        public bool IsAnyHabitatCanAcceptNewMonster(MonsterInsData target) {
            foreach (var habitat in UserInsData.Habitats) {
                if (habitat.InsId == target.Habitat) continue;
                if (!GameDefData.Monsters[target.Id].Elements.Contains(GameDefData.Habitats[habitat.Id].Element)) continue;
                if (GameDefData.Habitats[habitat.Id].Capacity <= UserInsData.Monsters.Count(i => i.Habitat == habitat.InsId)) continue;
                return true;
            }

            return false;
        }

        public BuildingDefData GetBuildingDefData(BuildingInsData insData) {
            return insData switch {
                FarmInsData farm       => GameDefData.Farms[farm.Id]
              , HabitatInsData habitat => GameDefData.Habitats[habitat.Id]

              , _ => throw new Exception($"Unknown {nameof(BuildingInsData)} type: {insData.GetType().Name}")
            };
        }

        public bool IsHaveBuilding(BuildingInsData insData) {
            return insData switch {
                FarmInsData farm       => UserInsData.Farms.Contains(farm)
              , HabitatInsData habitat => UserInsData.Habitats.Contains(habitat)

              , _ => throw new Exception($"Unknown {nameof(BuildingInsData)} type: {insData.GetType().Name}")
            };
        }

        public IReadOnlyList<UserInsData> GetUserListTest() {
            return new[] {
                UserInsData
              , new() {
                    Name = "Beast Master"
                  , Elo  = 152
                }
              , new() {
                    Name = "Shadow Hunter"
                  , Elo  = 110
                }
              , new() {
                    Name = "Fire Dragon 99"
                  , Elo  = 75
                }
              , new() {
                    Name = "Slime Rider"
                  , Elo  = 34
                }
            };
        }

        public void UpdateData_CollectGold(HabitatInsData habitat) {
            UpdateData_HabitatLastGoldUpdate(habitat);

            userInsDataSO.Data.Gold += habitat.CurGold;
            habitat.CurGold         =  0;
            
            SaveAllSO();
        }

        public void UpdateData_CollectFood(FarmInsData farm) {
            UpdateData_FarmLastFoodUpdate(farm);

            userInsDataSO.Data.Food += farm.CurFood;
            farm.CurFood            =  0;
            
            SaveAllSO();
        }

        public void UpdateData_FeedMonster(MonsterInsData monster, int feedAmount, out bool levelChanged) {
            UpdateData_AddExpMonster(monster, feedAmount, out levelChanged);

            userInsDataSO.Data.Food -= feedAmount;
            
            SaveAllSO();
        }

        public void UpdateData_AddExpMonster(MonsterInsData monster, int expAmount, out bool levelChanged) {
            UpdateData_HabitatLastGoldUpdate(UserInsData.Habitats.Find(i => i.InsId == monster.Habitat));

            var defData = GameDefData.Monsters[monster.Id];
            levelChanged = false;
            while (expAmount > 0) {
                var requiredExp = defData.CalculateStat(monster, MonsterStatId.FoodCost) - monster.Exp;
                if (expAmount < requiredExp) {
                    monster.Exp += expAmount;
                    break;
                }

                monster.Exp =  0;
                expAmount   -= requiredExp;
                ++monster.Level;
                levelChanged = true;
            }
            
            SaveAllSO();
        }

        public void UpdateData_MoveBuilding(BuildingInsData building, Vector2Int newPos) {
            building.Position = newPos;
            
            SaveAllSO();
        }

        public void UpdateData_HabitatLastGoldUpdate(HabitatInsData habitat) {
            habitat.CurGold        = CalculateCurTotalGold(habitat);
            habitat.LastGoldUpdate = SerTimestamp.Now();

            static long CalculateCurTotalGold(HabitatInsData habitat) {
                float result  = habitat.CurGold;
                float minutes = SerTimestamp.DeltaMinutes(SerTimestamp.Now(), habitat.LastGoldUpdate);

                foreach (var monster in Ins.UserInsData.Monsters) {
                    if (monster.Habitat != habitat.InsId) continue;
                    result += minutes * Ins.GameDefData.Monsters[monster.Id].CalculateStat(monster, MonsterStatId.GoldPerMin);
                }

                return Math.Min(Ins.GameDefData.Habitats[habitat.Id].MaxGold, (long)(result));
            }
            
            SaveAllSO();
        }

        public void UpdateData_FarmLastFoodUpdate(FarmInsData farm) {
            farm.CurFood        = CalculateCurTotalFood(farm);
            farm.LastFoodUpdate = SerTimestamp.Now();

            static long CalculateCurTotalFood(FarmInsData farm) {
                return Ins.GameDefData.Farms[farm.Id].CalculateFood(farm);
            }
            
            SaveAllSO();
        }

        public void UpdateData_MonsterCustomName(MonsterInsData monster, string newName) {
            monster.CustomName = newName;
            
            SaveAllSO();
        }

        public void UpdateData_MoveMonster(MonsterInsData monster, HabitatInsData newHabitat) {
            var oldHabitat = UserInsData.Habitats.Find(i => i.InsId == monster.Habitat);

            UpdateData_HabitatLastGoldUpdate(oldHabitat);
            UpdateData_HabitatLastGoldUpdate(newHabitat);

            monster.Habitat = newHabitat.InsId;
            
            SaveAllSO();
        }

        public void UpdateData_SellMonster(MonsterInsData monster) {
            UpdateData_HabitatLastGoldUpdate(UserInsData.Habitats.Find(i => i.InsId == monster.Habitat));

            UserInsData.Gold += (int)(GameDefData.Monsters[monster.Id].Cost * Ins.GameDefData.SellRatio_Monster);
            UserInsData.Monsters.Remove(monster);
            
            SaveAllSO();
        }

        public void UpdateData_SellBuilding(BuildingInsData building) {
            switch (building) {
                case FarmInsData farm:       UserInsData.Farms.Remove(farm); break;
                case HabitatInsData habitat: UserInsData.Habitats.Remove(habitat); break;

                default: throw new Exception($"Unknown building type {building.GetType().Name}");
            }

            UserInsData.Gold += (int)(GetBuildingDefData(building).Cost * Ins.GameDefData.SellRatio_Building);
            
            SaveAllSO();
        }

        public void UpdateData_BuyMonster(MonsterId id, HabitatInsData habitat, out int cost, out string insId) {
            UpdateData_HabitatLastGoldUpdate(habitat);

            var insData = new MonsterInsData(id);
            insId           = insData.InsId;
            insData.Habitat = habitat.InsId;
            UserInsData.Monsters.Add(insData);

            cost             =  GameDefData.Monsters[id].Cost;
            UserInsData.Gold -= cost;
            
            SaveAllSO();
        }

        public void UpdateData_BuyFarm(FarmId id, Vector2Int pos, out int cost, out string insId) {
            var insData = new FarmInsData(id);
            insId                  = insData.InsId;
            insData.Position       = pos;
            insData.LastFoodUpdate = SerTimestamp.Now();
            UserInsData.Farms.Add(insData);

            cost             =  GameDefData.Farms[id].Cost;
            UserInsData.Gold -= cost;
            
            SaveAllSO();
        }

        public void UpdateData_BuyHabitat(ElementId id, Vector2Int pos, out int cost, out string insId) {
            var insData = new HabitatInsData(id);
            insId                  = insData.InsId;
            insData.Position       = pos;
            insData.LastGoldUpdate = SerTimestamp.Now();
            UserInsData.Habitats.Add(insData);

            cost             =  GameDefData.Habitats[id].Cost;
            UserInsData.Gold -= cost;
            
            SaveAllSO();
        }

        public void UpdateData_MonsterSkill(MonsterInsData monster, int slotId, int skillId) {
            monster.SkillList[slotId] = skillId;
            
            SaveAllSO();
        }

        public void UpdateData_ArenaTeamAttack(MonsterTeamSlots<string> newTeam) {
            for (int i = 0; i < newTeam.Count; ++i) UserInsData.ArenaTeamAttack[i] = newTeam[i];
            
            SaveAllSO();
        }

        public void UpdateData_ArenaTeamDefense(MonsterTeamSlots<string> newTeam) {
            for (int i = 0; i < newTeam.Count; ++i) UserInsData.ArenaTeamDefense[i] = newTeam[i];
            
            SaveAllSO();
        }

        public void UpdateData_AdventureTeam(MonsterTeamSlots<string> newTeam) {
            for (int i = 0; i < newTeam.Count; ++i) UserInsData.AdventureTeam[i] = newTeam[i];
            
            SaveAllSO();
        }

        public void UpdateData_EloAfterBattleTest(UserInsData winner, UserInsData loser, int winnerDeltaElo, int loserDeltaElo) {
            winner.Elo += winnerDeltaElo;
            loser.Elo  += loserDeltaElo;
            
            SaveAllSO();
        }

        public void UpdateData_DefeatBattleEnd(AdventureLevelData levelData, int levelIndex, out bool levelUp) {
            UserInsData.Exp  += levelData.RewardExp;
            UserInsData.Gold += levelData.RewardGold;
            UserInsData.Food += levelData.RewardFood;

            if (levelIndex >= UserInsData.CurAdventureLevel) UserInsData.CurAdventureLevel = levelIndex + 1;

            levelUp = false;
            while (UserInsData.Level < GameDefData.User.MaxLevel) {
                var expCost = GameDefData.User.CalcExpCost(UserInsData.Level);
                if (UserInsData.Exp < expCost) break;

                levelUp           =  true;
                UserInsData.Exp   -= expCost;
                UserInsData.Level += 1;
            }
            
            SaveAllSO();
        }

        public void UpdateData_UserName(string name) {
            UserInsData.Name = name;
            
            SaveAllSO();
        }

        public void UpdateData_UserMusic(bool isOn) {
            UserInsData.Music = isOn;
            
            SaveAllSO();
        }

        public void UpdateData_UserSound(bool isOn) {
            UserInsData.Sound = isOn;
            
            SaveAllSO();
        }

        public void UpdateData_UserVibrant(bool isOn) {
            UserInsData.Vibrant = isOn;
            
            SaveAllSO();
        }
    }
}