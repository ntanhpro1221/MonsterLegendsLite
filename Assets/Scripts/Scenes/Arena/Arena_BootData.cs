using NGDtuanh.Types;

namespace MonsterLegendsLite {
    public class Arena_BootData : Singleton<Arena_BootData> {
        public int? endBattleDeltaElo;

        public void SetData_EndBattleDeltaElo(int deltaElo) {
            endBattleDeltaElo = deltaElo;
        }
    }
}