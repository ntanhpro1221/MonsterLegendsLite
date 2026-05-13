using MonsterLegendsLite.Data;

namespace MonsterLegendsLite {
    public class Arena_TeamAttack : Arena_MonsterList {
        protected override MonsterTeamSlots<MonsterInsData> GetListData() {
            return DataManager.Ins.UserInsData.GetArenaTeamAttackData();
        }

        protected override void OnListChanged() {
            DataManager.Ins.UpdateData_ArenaTeamAttack(GetSlotsId());
        }
    }
}