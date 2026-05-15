using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace MonsterLegendsLite.Data {
    public class DataManager : Singleton<DataManager> {
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), Required]
        private GameDefDataSO gameDefDataSO;

        public GameDefData GameDef => gameDefDataSO.Data;

        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), Required]
        private GameLocDefDataSO gameLocDefDataSO;

        public GameLocDefData GameLocDef => gameLocDefDataSO.Data;

        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), Required]
        private UserInsDataSO userInsDataSO;

        private DatabaseReference allUsersDbRef;
        private DatabaseReference userDbRef;
        private UserInsData userInsDataRemote;

        public UserInsData User => userInsDataRemote;
        public UserInsDataWithId UserWithId => User.WithId(FirebaseAuth.DefaultInstance.CurrentUser.UserId);

        [Button]
        private void PushUserSOToRemote() {
            userDbRef.SetRawJsonValueAsync(JsonConvert.SerializeObject(userInsDataSO.Data));
        }

        public async Task LoadDataAsync(UnityAction<float> onProgressChanged) {
            onProgressChanged?.Invoke(.2f);

            allUsersDbRef = FirebaseDatabase.DefaultInstance.RootReference.Child($"Users");
            userDbRef     = allUsersDbRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
            var snapshot = await userDbRef.GetValueAsync();
            onProgressChanged?.Invoke(.7f);

            if (snapshot.Exists) {
                var json = snapshot.GetRawJsonValue();
                userInsDataRemote = JsonConvert.DeserializeObject<UserInsData>(json);
            } else {
                userInsDataRemote = new UserInsData();
                var json = JsonConvert.SerializeObject(userInsDataRemote);
                await userDbRef.SetRawJsonValueAsync(json);
                onProgressChanged?.Invoke(.9f);
            }

            onProgressChanged?.Invoke(1);
        }

        private void SaveUserData<T>(T data, string path, DatabaseReference userDbRef = null) {
            userDbRef ??= this.userDbRef;
            userDbRef.Child(path).SetRawJsonValueAsync(JsonConvert.SerializeObject(data));
        }

        private void SaveUserData_List<T>(IReadOnlyList<T> list, T data, string path) {
            int index = 0;
            while (index < list.Count && !EqualityComparer<T>.Default.Equals(list[index], data)) ++index;
            SaveUserData(data, $"{path}/{index}");
        }

        public bool IsAnyHabitatCanAcceptNewMonster(MonsterInsData target) {
            foreach (var habitat in User.Habitats) {
                if (habitat.InsId == target.Habitat) continue;
                if (!GameDef.Monsters[target.Id].Elements.Contains(GameDef.Habitats[habitat.Id].Element)) continue;
                if (GameDef.Habitats[habitat.Id].Capacity <= User.Monsters.Count(i => i.Habitat == habitat.InsId)) continue;
                return true;
            }

            return false;
        }

        public BuildingDefData GetBuildingDefData(BuildingInsData insData) {
            return insData switch {
                FarmInsData farm       => GameDef.Farms[farm.Id]
              , HabitatInsData habitat => GameDef.Habitats[habitat.Id]

              , _ => throw new Exception($"Unknown {nameof(BuildingInsData)} type: {insData.GetType().Name}")
            };
        }

        private ((IList generic, IReadOnlyList<BuildingInsData> concrete) list, string name) GetBuildingList(BuildingInsData item) {
            static (IList, IReadOnlyList<BuildingInsData>) ShortCut<T>(List<T> list) where T : BuildingInsData => (list, list);

            return item switch {
                FarmInsData    => (ShortCut(User.Farms), nameof(User.Farms))
              , HabitatInsData => (ShortCut(User.Habitats), nameof(User.Habitats))

              , _ => throw new Exception($"Unknown {nameof(BuildingInsData)} type: {item.GetType().Name}")
            };
        }

        public bool IsHaveBuilding(BuildingInsData insData) {
            return GetBuildingList(insData).list.concrete.Contains(insData);
        }

        public async Task<Dictionary<string, UserInsData>> GetAllUsersAsync() {
            var result = JsonConvert.DeserializeObject<Dictionary<string, UserInsData>>((await allUsersDbRef.GetValueAsync()).GetRawJsonValue());
            return result;
        }

        public void UpdateData_CollectGold(HabitatInsData habitat) {
            UpdateData_HabitatLastGoldUpdate(habitat);

            SaveUserData(User.Gold += habitat.CurGold, nameof(User.Gold));
            habitat.CurGold = 0;
            SaveUserData_List(User.Habitats, habitat, nameof(User.Habitats));
        }

        public void UpdateData_CollectFood(FarmInsData farm) {
            UpdateData_FarmLastFoodUpdate(farm);

            SaveUserData(User.Food += farm.CurFood, nameof(User.Food));
            farm.CurFood = 0;
            SaveUserData_List(User.Farms, farm, nameof(User.Farms));
        }

        public void UpdateData_FeedMonster(MonsterInsData monster, int feedAmount, out bool levelChanged) {
            UpdateData_AddExpMonster(monster, feedAmount, out levelChanged);

            SaveUserData(User.Food -= feedAmount, nameof(User.Food));
        }

        public void UpdateData_AddExpMonster(MonsterInsData monster, int expAmount, out bool levelChanged) {
            UpdateData_HabitatLastGoldUpdate(User.Habitats.Find(i => i.InsId == monster.Habitat));

            var defData = GameDef.Monsters[monster.Id];
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

            SaveUserData_List(User.Monsters, monster, nameof(User.Monsters));
        }

        public void UpdateData_MoveBuilding(BuildingInsData building, Vector2Int newPos) {
            building.Position = newPos;

            var (list, listName) = GetBuildingList(building);
            SaveUserData_List(list.concrete, building, listName);
        }

        public void UpdateData_HabitatLastGoldUpdate(HabitatInsData habitat) {
            habitat.CurGold        = CalculateCurTotalGold(habitat);
            habitat.LastGoldUpdate = SerTimestamp.Now();

            static long CalculateCurTotalGold(HabitatInsData habitat) {
                float result  = habitat.CurGold;
                float minutes = SerTimestamp.DeltaMinutes(SerTimestamp.Now(), habitat.LastGoldUpdate);

                foreach (var monster in Ins.User.Monsters) {
                    if (monster.Habitat != habitat.InsId) continue;
                    result += minutes * Ins.GameDef.Monsters[monster.Id].CalculateStat(monster, MonsterStatId.GoldPerMin);
                }

                return Math.Min(Ins.GameDef.Habitats[habitat.Id].MaxGold, (long)(result));
            }

            SaveUserData_List(User.Habitats, habitat, nameof(User.Habitats));
        }

        public void UpdateData_FarmLastFoodUpdate(FarmInsData farm) {
            farm.CurFood        = CalculateCurTotalFood(farm);
            farm.LastFoodUpdate = SerTimestamp.Now();

            static long CalculateCurTotalFood(FarmInsData farm) {
                return Ins.GameDef.Farms[farm.Id].CalculateFood(farm);
            }

            SaveUserData_List(User.Farms, farm, nameof(User.Farms));
        }

        public void UpdateData_MonsterCustomName(MonsterInsData monster, string newName) {
            monster.CustomName = newName;

            SaveUserData_List(User.Monsters, monster, nameof(User.Monsters));
        }

        public void UpdateData_MoveMonster(MonsterInsData monster, HabitatInsData newHabitat) {
            var oldHabitat = User.Habitats.Find(i => i.InsId == monster.Habitat);

            UpdateData_HabitatLastGoldUpdate(oldHabitat);
            UpdateData_HabitatLastGoldUpdate(newHabitat);

            monster.Habitat = newHabitat.InsId;
            SaveUserData_List(User.Monsters, monster, nameof(User.Monsters));
        }

        public void UpdateData_SellMonster(MonsterInsData monster) {
            UpdateData_HabitatLastGoldUpdate(User.Habitats.Find(i => i.InsId == monster.Habitat));

            User.Monsters.Remove(monster);
            SaveUserData(User.Monsters, nameof(User.Monsters));

            SaveUserData(
                User.Gold += (int)(GameDef.Monsters[monster.Id].Cost * Ins.GameDef.SellRatio_Monster)
              , nameof(User.Gold));
        }

        public void UpdateData_SellBuilding(BuildingInsData building) {
            var (list, listName) = GetBuildingList(building);
            list.generic.Remove(building);
            SaveUserData(list.generic, listName);

            SaveUserData(
                User.Gold += (int)(GetBuildingDefData(building).Cost * Ins.GameDef.SellRatio_Building)
              , nameof(User.Gold));
        }

        public void UpdateData_BuyMonster(MonsterId id, HabitatInsData habitat, out int cost, out string insId) {
            UpdateData_HabitatLastGoldUpdate(habitat);

            var insData = new MonsterInsData(id);
            insId           = insData.InsId;
            insData.Habitat = habitat.InsId;
            User.Monsters.Add(insData);
            SaveUserData_List(User.Monsters, insData, nameof(User.Monsters));

            cost = GameDef.Monsters[id].Cost;
            SaveUserData(User.Gold -= cost, nameof(User.Gold));
        }

        public void UpdateData_BuyFarm(FarmId id, Vector2Int pos, out int cost, out string insId) {
            var insData = new FarmInsData(id);
            insId                  = insData.InsId;
            insData.Position       = pos;
            insData.LastFoodUpdate = SerTimestamp.Now();
            User.Farms.Add(insData);
            SaveUserData_List(User.Farms, insData, nameof(User.Farms));

            cost = GameDef.Farms[id].Cost;
            SaveUserData(User.Gold -= cost, nameof(User.Gold));
        }

        public void UpdateData_BuyHabitat(ElementId id, Vector2Int pos, out int cost, out string insId) {
            var insData = new HabitatInsData(id);
            insId                  = insData.InsId;
            insData.Position       = pos;
            insData.LastGoldUpdate = SerTimestamp.Now();
            User.Habitats.Add(insData);
            SaveUserData_List(User.Habitats, insData, nameof(User.Habitats));

            cost = GameDef.Habitats[id].Cost;
            SaveUserData(User.Gold -= cost, nameof(User.Gold));
        }

        public void UpdateData_MonsterSkill(MonsterInsData monster, int slotId, int skillId) {
            monster.SkillList[slotId] = skillId;

            SaveUserData_List(User.Monsters, monster, nameof(User.Monsters));
        }

        public void UpdateData_ArenaTeamAttack(MonsterTeamSlots<string> newTeam) {
            for (int i = 0; i < newTeam.Count; ++i) User.ArenaTeamAttack[i] = newTeam[i];

            SaveUserData(newTeam, nameof(User.ArenaTeamAttack));
        }

        public void UpdateData_ArenaTeamDefense(MonsterTeamSlots<string> newTeam) {
            for (int i = 0; i < newTeam.Count; ++i) User.ArenaTeamDefense[i] = newTeam[i];

            SaveUserData(newTeam, nameof(User.ArenaTeamDefense));
        }

        public void UpdateData_AdventureTeam(MonsterTeamSlots<string> newTeam) {
            for (int i = 0; i < newTeam.Count; ++i) User.AdventureTeam[i] = newTeam[i];

            SaveUserData(newTeam, nameof(User.AdventureTeam));
        }

        public void UpdateData_EloAfterBattleTest(UserInsDataWithId winner, UserInsDataWithId loser, int winnerDeltaElo, int loserDeltaElo) {
            SaveUserData(winner.Data.Elo += winnerDeltaElo, nameof(UserInsData.Elo), allUsersDbRef.Child(winner.Id));
            SaveUserData(loser.Data.Elo  += loserDeltaElo, nameof(UserInsData.Elo), allUsersDbRef.Child(loser.Id));
        }

        public void UpdateData_WinAdventureLevel(AdventureLevelData levelData, int levelIndex, out bool levelUp) {
            SaveUserData(User.Exp  += levelData.RewardExp, nameof(User.Exp));
            SaveUserData(User.Gold += levelData.RewardGold, nameof(User.Gold));
            SaveUserData(User.Food += levelData.RewardFood, nameof(User.Food));

            if (levelIndex >= User.CurAdventureLevel) {
                SaveUserData(User.CurAdventureLevel = levelIndex + 1, nameof(User.CurAdventureLevel));
            }

            levelUp = false;
            while (User.Level < GameDef.User.MaxLevel) {
                var expCost = GameDef.User.CalcExpCost(User.Level);
                if (User.Exp < expCost) break;

                levelUp = true;
                SaveUserData(User.Exp   -= expCost, nameof(User.Exp));
                SaveUserData(User.Level += 1, nameof(User.Level));
            }
        }

        public void UpdateData_UserName(string name) {
            SaveUserData(User.Name = name, nameof(User.Name));
        }

        public void UpdateData_UserMusic(bool isOn) {
            SaveUserData(User.Music = isOn, nameof(User.Music));
        }

        public void UpdateData_UserSound(bool isOn) {
            SaveUserData(User.Sound = isOn, nameof(User.Sound));
        }

        public void UpdateData_UserVibrant(bool isOn) {
            SaveUserData(User.Vibrant = isOn, nameof(User.Vibrant));
        }
    }
}