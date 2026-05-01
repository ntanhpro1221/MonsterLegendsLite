using MonsterLegendsLite.Data;
using NGDtuanh.Types;

namespace MonsterLegendsLite {
    public class Home_BootData : Singleton<Home_BootData> {
        public MonsterInsData moveMonster;

        public void SetData_MoveMonster(MonsterInsData monster) {
            moveMonster = monster;
        }
    }
}