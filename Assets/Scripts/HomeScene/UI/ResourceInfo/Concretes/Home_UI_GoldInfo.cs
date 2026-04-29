using MonsterLegendsLite.Data;

namespace MonsterLegendsLite.UI {
    public class Home_UI_GoldInfo : Home_UI_ResourceInfo {
        protected override void Initialize() { }

        protected override void Refresh() {
            Data.SetValue(DataManager.Ins.UserInsData.Gold);
        }
    }
}