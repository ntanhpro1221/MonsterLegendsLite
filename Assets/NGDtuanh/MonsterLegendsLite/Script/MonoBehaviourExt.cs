using System.Diagnostics;
using UnityEngine;

namespace NGDtuanh.MonsterLegends {
    public class MonoBehaviourExt : MonoBehaviour {
        protected readonly UtilFuncs utils = new();

        private Transform tf;
        public Transform TF => tf != null ? tf : tf = transform;

        [Conditional("UNITY_EDITOR")] protected void RecordForUndo(Object target = null) => utils.RecordForUndo(target == null ? this : target);

        [Conditional("UNITY_EDITOR")] protected void RecordForUndo(params Object[] targets) => utils.RecordForUndo(targets);

        [Conditional("UNITY_EDITOR")] protected void MarkDirty(Object target = null) => utils.MarkDirty(target == null ? this : target);

        [Conditional("UNITY_EDITOR")] protected void MarkDirty(params Object[] targets) => utils.MarkDirty(targets);
    }
}