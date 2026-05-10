using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Arena_TeamDefense : Arena_MonsterList {
        protected override MonsterTeamSlots<string> GetListData() {
            return DataManager.Ins.UserInsData.ArenaTeamDefense;
        }

        protected override void OnListChanged() {
            DataManager.Ins.UpdateData_ArenaTeamDefense(GetSlotsId());
        }
    }
}