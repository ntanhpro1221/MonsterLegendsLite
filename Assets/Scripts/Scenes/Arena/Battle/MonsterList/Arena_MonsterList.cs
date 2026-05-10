using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public abstract class Arena_MonsterList : MonoBehaviourExt {
        [field: SerializeField, Required]
        protected Arena_MonsterListSharedData SharedData { get; private set; }

        public void Initialize() {
            var data = GetListData();
            var userMonsters = DataManager.Ins.UserInsData.Monsters;
            for (int i = 0; i < SharedData.Slots.Count; ++i)
                SetupSlot(
                    SharedData.Slots[i]
                  , userMonsters.FirstOrDefault(monster => monster.InsId == data[i]));
        }

        private void SetupSlot(Arena_MonsterSlot slot, MonsterInsData monster) {
            slot.SetAllData(monster);
            slot.SetReplaceButton(() => {
                ReplaceMonsterWindow.Show(
                    prefab: SharedData.prefabReplaceMonsterWindow
                  , title: (monster == null ? "SET" : "REPLACE") + " MONSTER"
                  , onSelected: selectedMonster => {
                        SetupSlot(slot, selectedMonster);
                        OnListChanged();
                    });
            });
        }

        protected MonsterTeamSlots<string> GetSlotsId() {
            var result = new MonsterTeamSlots<string>();
            for (int i = 0; i < result.Count; ++i)
                result[i] = SharedData.Slots[i].InsData != null
                    ? SharedData.Slots[i].InsData.InsId
                    : null;
            return result;
        }

        protected abstract MonsterTeamSlots<string> GetListData();
        protected abstract void OnListChanged();
    }
}