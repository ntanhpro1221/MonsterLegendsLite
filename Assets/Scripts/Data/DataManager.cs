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

        public void UpdateData_CollectGold(Home_Habitat habitat) {
            var gold = habitat.CalculateCurTotalGold();

            userInsDataSO.Data.Gold += gold;

            habitat.insData.CurGold        = 0;
            habitat.insData.LastGoldUpdate = SerTimestamp.GetCurTimestamp();
        }

        public void UpdateData_CollectFood(Home_Farm farm) {
            var food = farm.CalculateCurTotalFood();

            userInsDataSO.Data.Food += food;

            farm.insData.CurFood        = 0;
            farm.insData.LastFoodUpdate = SerTimestamp.GetCurTimestamp();
        }

        public void UpdateData_FeedMonster(MonsterInsData monster, int amount, int newLevel, int newExp) {
            userInsDataSO.Data.Food -= amount;

            monster.Level = newLevel;
            monster.Exp   = newExp;
        }

        public void UpdateData_MoveHabitat(HabitatInsData habitat, Vector2Int newPos) {
            habitat.Position = newPos;
        }
        
        public void UpdateData_MoveFarm(FarmInsData farm, Vector2Int newPos) {
            farm.Position = newPos;
        }
    }
}