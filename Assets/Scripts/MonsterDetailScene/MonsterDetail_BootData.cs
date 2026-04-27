using MonsterLegendsLite.Data;
using NGDtuanh.Types;

namespace MonsterLegendsLite {
    public class MonsterDetail_BootData : Singleton<MonsterDetail_BootData> {
        public MonsterInsData monster;

        public void SetData(MonsterInsData monster) {
            this.monster = monster;
        }
    }
}