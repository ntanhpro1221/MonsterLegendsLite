using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Arena_TeamDefense : Arena_MonsterList {
        protected override MonsterTeamSlots<MonsterInsData> GetListData() {
            return DataManager.Ins.User.GetArenaTeamDefenseData();
        }

        protected override void OnListChanged() {
            DataManager.Ins.UpdateData_ArenaTeamDefense(GetSlotsId());
        }
    }
}