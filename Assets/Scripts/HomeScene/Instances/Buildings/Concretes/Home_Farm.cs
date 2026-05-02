using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Home_Farm : Home_Building<FarmInsData> {
        public long CalculateCurTotalFood() {
            return DataManager.Ins.GameDefData.Farm[InsData.Id].CalculateFood(InsData);
        }
    }
}