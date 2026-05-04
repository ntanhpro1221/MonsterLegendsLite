using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class MonsterModel : FBFModel<MonsterAnimId> {
        [SerializeField, Required]
        private Transform attackPos;

        public Vector3 AttackPos => attackPos.position;
    }
}