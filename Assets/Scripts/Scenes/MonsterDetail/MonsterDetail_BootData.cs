using MonsterLegendsLite.Data;
using NGDtuanh.Types;

namespace MonsterLegendsLite {
    public class MonsterDetail_BootData : Singleton<MonsterDetail_BootData> {
        public MonsterInsData Monster { get; private set; }

        public void SetData(MonsterInsData monster) {
            this.Monster = monster;
        }
    }
}