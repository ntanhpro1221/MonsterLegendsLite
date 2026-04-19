using System;
using System.Collections.Generic;
using System.Linq;
using NGDtuanh.Utils.Editor.SearchWindow;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace NGDtuanh.Utils.Editor {
    public static class ReplaceComponentTool {
        private static bool isHandlingCommand;
        private static int totalCommandAmount;

        private static readonly List<Component> handlingComponents = new();

        /// <summary>
        /// Caution: References to the old component are only replaced within the currently active scene.
        /// </summary>
        [MenuItem("CONTEXT/Component/Replace Component")]
        public static void ReplaceComponent(MenuCommand command) {
            if (!isHandlingCommand) {
                isHandlingCommand  = true;
                totalCommandAmount = Selection.gameObjects.Length;
            }

            handlingComponents.Add(command.context as Component);

            if (handlingComponents.Count < totalCommandAmount) return;

            var storedHandlingComponents = new List<Component>(handlingComponents);
            ComponentSearchWindow.Open(
                new(EditorWindow.focusedWindow?.position.center ?? new Vector2(Screen.width, Screen.height) / 2, default)
              , newCpnType => ReplaceComponent(storedHandlingComponents, newCpnType)
              , searchText: handlingComponents[0].GetType().Name);

            isHandlingCommand = false;
            handlingComponents.Clear();
        }

        private static void ReplaceComponent(List<Component> components, Type newCpnType) {
            if (components[0].GetType() == newCpnType) {
                Debug.LogWarning($"NGDtuanh {nameof(ReplaceComponentTool)}: You are replacing {newCpnType.Name} with the same type (Action skipped)!");
                return;
            }

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Replace Component");

            var refMap     = new Dictionary<Component, Component>(components.Count);
            var prefabCpns = new List<Component>();

            foreach (var oldCpn in components) {
                var oldSO = new SerializedObject(oldCpn);
                var newSO = new SerializedObject(refMap[oldCpn] = Undo.AddComponent(oldCpn.gameObject, newCpnType));

                // Copy value
                CopySerializedObject(oldSO, newSO);
                newSO.ApplyModifiedProperties();

                // Try to get components in current component's prefab
                if (!UtilFuncs.Ins.TryGetRootPrefab(oldCpn, out var rootPrefab)) continue;
                prefabCpns.AddRange(rootPrefab.GetComponentsInChildren<Component>(includeInactive: true));
            }

            // Try the best to replace reference in [all visible prefab asset] + [active scene]
            // Get components in scene must be call after all new components were created
            foreach (var cpn in prefabCpns.Concat(Object.FindObjectsByType<Component>(FindObjectsInactive.Include))) {
                var so = new SerializedObject(cpn);

                if (!TryReplaceRefs(so, refMap)) continue;

                so.ApplyModifiedProperties();
            }

            foreach (var (oldCpn, newCpn) in refMap) {
                Undo.DestroyObjectImmediate(oldCpn);
                UtilFuncs.Ins.MarkDirty(newCpn.gameObject);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private static void CopySerializedObject(SerializedObject source, SerializedObject target) {
            var srcProp = source.GetIterator();

            if (!srcProp.Next(true)) return;

            do {
                if (srcProp.name == "m_Script") continue;

                if (target.FindProperty(srcProp.name) == null) continue;

                try {
                    target.CopyFromSerializedProperty(srcProp);
                } catch { }
            } while (srcProp.Next(false));
        }

        private static bool TryReplaceRefs(SerializedObject target, Dictionary<Component, Component> refMap) {
            var prop    = target.GetIterator();
            var updated = false;

            while (prop.Next(true)) {
                if (prop.propertyType != SerializedPropertyType.ObjectReference) continue;
                if (prop.objectReferenceValue is not Component propCpnRef) continue;
                if (!refMap.TryGetValue(propCpnRef, out var newRef)) continue;

                prop.objectReferenceValue = newRef;
                updated                   = true;
            }

            return updated;
        }
    }
}