using System.Linq;
using MonsterLegendsLite.Data;
using UnityEngine;

namespace MonsterLegendsLite {
    public class MonsterDetail_BootDataTest : MonsterDetail_BootData {
        [SerializeField]
        private string testMonsterInsId;
        
        protected override void Initialize() {
            base.Initialize();

            var monster = DataManager.Ins.UserInsData.Monsters.FirstOrDefault(i => i.InsId == testMonsterInsId);
            if (monster == null) monster = DataManager.Ins.UserInsData.Monsters[0];
            SetData(monster);
        }
    }
}