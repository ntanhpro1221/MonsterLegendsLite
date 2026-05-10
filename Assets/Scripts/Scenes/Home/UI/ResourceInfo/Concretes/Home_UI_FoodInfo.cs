using MonsterLegendsLite.Data;

namespace MonsterLegendsLite.UI {
    public class Home_UI_FoodInfo : Home_UI_ResourceInfo {
        protected override void Initialize() { }

        protected override void Refresh() {
            SharedData.SetValue(DataManager.Ins.UserInsData.Food);
        }
    }
}