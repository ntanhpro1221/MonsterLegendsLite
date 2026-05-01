using MonsterLegendsLite.Data;
using NGDtuanh.Types;

namespace MonsterLegendsLite {
    public class Home_BootData : Singleton<Home_BootData> {
        public MonsterInsData moveMonster;
        public int? floatingGold;

        public void SetData_MoveMonster(MonsterInsData monster) {
            moveMonster = monster;
        }

        public void SetData_FloatingGold(int gold) {
            floatingGold = gold;
        }
    }
}