using MonsterLegendsLite.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Arena_MonsterSlot : Arena_Monster {
        [SerializeField, Required, Header("----SLOT-------")]
        private Button replaceBtn;

        public MonsterInsData InsData { get; private set; }
        
        public override void SetAllData(MonsterInsData insData) {
            base.SetAllData(insData);

            InsData = insData;
        }

        public void SetReplaceButton(UnityAction callback) {
            utils.SetListener(replaceBtn, callback);
        }
    }
}