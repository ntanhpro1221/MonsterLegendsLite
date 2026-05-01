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

        public void UpdateData_CollectGold(HabitatInsData habitat) {
            UpdateData_HabitatLastGoldUpdate(habitat);

            userInsDataSO.Data.Gold += habitat.CurGold;
            habitat.CurGold = 0;
        }

        public void UpdateData_CollectFood(FarmInsData farm) {
            UpdateData_FarmLastFoodUpdate(farm);
            
            userInsDataSO.Data.Food += farm.CurFood;
            farm.CurFood = 0;
        }

        public void UpdateData_FeedMonster(MonsterInsData monster, int feedAmount, out bool levelChanged) {
            UpdateData_AddExpMonster(monster, feedAmount, out levelChanged);
            
            userInsDataSO.Data.Food -= feedAmount;
        }

        public void UpdateData_AddExpMonster(MonsterInsData monster, int expAmount, out bool levelChanged) {
            UpdateData_HabitatLastGoldUpdate(UserInsData.Habitats.Find(i => i.InsId == monster.Habitat));
            
            var defData = GameDefData.Monster[monster.Id];
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
        }

        public void UpdateData_MoveHabitat(HabitatInsData habitat, Vector2Int newPos) {
            habitat.Position = newPos;
        }
        
        public void UpdateData_MoveFarm(FarmInsData farm, Vector2Int newPos) {
            farm.Position = newPos;
        }

        public void UpdateData_HabitatLastGoldUpdate(HabitatInsData habitat) {
            habitat.CurGold        = CalculateCurTotalGold(habitat);
            habitat.LastGoldUpdate = SerTimestamp.GetCurTimestamp();

            static long CalculateCurTotalGold(HabitatInsData habitat) {
                float result  = habitat.CurGold;
                float minutes = SerTimestamp.DeltaMinutes(SerTimestamp.GetCurTimestamp(), habitat.LastGoldUpdate);

                foreach (var monster in Ins.UserInsData.Monsters) {
                    if (monster.Habitat != habitat.InsId) continue;
                    result += minutes * Ins.GameDefData.Monster[monster.Id].CalculateStat(monster, MonsterStatId.GoldPerMin);
                }

                return (long)(result);
            }
        }

        public void UpdateData_FarmLastFoodUpdate(FarmInsData farm) {
            farm.CurFood        = CalculateCurTotalFood(farm);
            farm.LastFoodUpdate = SerTimestamp.GetCurTimestamp();

            static long CalculateCurTotalFood(FarmInsData farm) {
                return Ins.GameDefData.Farm[farm.Id].CalculateFood(farm);
            }
        }
    }
}