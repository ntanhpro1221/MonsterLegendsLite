using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Arena_MonsterListSharedData : MonoBehaviourExt {
        [SerializeField, Required]
        private MonsterTeamSlots<Arena_MonsterSlot> slots;

        public IReadOnlyList<Arena_MonsterSlot> Slots => slots;

        [field: SerializeField, Required]
        public ReplaceMonsterWindow prefabReplaceMonsterWindow { get; private set; }
    }
}