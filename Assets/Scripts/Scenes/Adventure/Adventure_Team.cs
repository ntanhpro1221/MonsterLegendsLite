using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Adventure_Team : Arena_MonsterList {
        protected override MonsterTeamSlots<MonsterInsData> GetListData() {
            return DataManager.Ins.UserInsData.GetAdventureTeamData();
        }

        protected override void OnListChanged() {
            DataManager.Ins.UpdateData_AdventureTeam(GetSlotsId());
        }
    }
}