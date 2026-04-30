using System;
using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_Farm : Home_Building {
        [NonSerialized, ShowInInspector, ReadOnly, PropertyOrder(-99)]
        public FarmInsData insData;
        
        public void Initialize(FarmInsData insData) {
            this.insData = insData;
            
            base.Initialize(insData.InsId);
        }

        public long CalculateCurTotalFood() {
            return DataManager.Ins.GameDefData.Farm[insData.Id].CalculateFood(insData);
        }

        public override Vector2Int GetSizeData() {
            return DataManager.Ins.GameDefData.Farm[insData.Id].Size;
        }

        public override Vector2Int GetPosData() {
            return insData.Position;
        }
        
        protected override void SavePos(Vector2Int tilePos) {
            DataManager.Ins.UpdateData_MoveFarm(insData, tilePos);
        }
    }
}