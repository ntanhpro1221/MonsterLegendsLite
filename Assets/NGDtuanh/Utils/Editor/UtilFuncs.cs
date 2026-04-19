using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NGDtuanh.Utils.Editor {
    public class UtilFuncs : NGDtuanh.Utils.UtilFuncs {
        public static readonly UtilFuncs Ins = new();

        public GameObject GetRootPrefab(Object obj) {
            if (obj is not GameObject go) {
                if (obj is not Component cpn) return null;
                go = cpn.gameObject;
            }

            if (PrefabStageUtility.GetPrefabStage(go) == null
             && !EditorUtility.IsPersistent(go)) {
                return null;
            }

            return go.transform.root.gameObject;
        }

        public bool TryGetRootPrefab(Object obj, out GameObject rootPrefab) {
            rootPrefab = GetRootPrefab(obj);
            return rootPrefab != null;
        }
    }
}