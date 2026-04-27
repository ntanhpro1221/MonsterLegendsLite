using System;
using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite {
    public class Home_Farm : Home_Building {
        [NonSerialized, ShowInInspector, ReadOnly]
        public FarmInsData insData;
        
        public void Initialize(FarmInsData insData) {
            base.Initialize(insData.InsId);
            
            this.insData = insData;
        }

        public long CalculateCurTotalFood() {
            return DataManager.Ins.GameDefData.Farm[insData.Id].CalculateFood(insData);
        }
    }
}