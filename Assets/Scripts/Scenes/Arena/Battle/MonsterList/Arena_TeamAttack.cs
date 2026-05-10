using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Arena_TeamAttack : Arena_MonsterList {
        protected override MonsterTeamSlots<string> GetListData() {
            return DataManager.Ins.UserInsData.ArenaTeamAttack;
        }

        protected override void OnListChanged() {
            DataManager.Ins.UpdateData_ArenaTeamAttack(GetSlotsId());
        }
    }
}