#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using NGDtuanh.Utils.Editor.SearchWindow;
using UnityEngine;
using UnityEditor;

namespace NGDtuanh.Utils.Editor {
    public static class ReplaceComponentTool {
        private static          bool            _IsHandlingCommand;
        private static          int             _TotalCommandAmount;
        private static readonly List<Component> _HandlingComponents = new();

        [MenuItem("CONTEXT/Component/Replace Component")]
        public static void ReplaceComponent(MenuCommand command) {
            if (!_IsHandlingCommand) {
                _IsHandlingCommand  = true;
                _TotalCommandAmount = Selection.gameObjects.Length;
            }

            _HandlingComponents.Add(command.context as Component);

            if (_HandlingComponents.Count < _TotalCommandAmount) return;

            var handlingComponents = new List<Component>(_HandlingComponents);
            ComponentSearchWindow.Open(
                new Rect(
                    EditorWindow.focusedWindow?.position.center ?? new Vector2(Screen.width, Screen.height) / 2
                  , default)
              , newMonoType => _ChangeComponent(
                    handlingComponents
                  , newMonoType)
              , searchText: _HandlingComponents[0].GetType().Name);

            _IsHandlingCommand = false;
            _HandlingComponents.Clear();
        }

        private static void _ChangeComponent(List<Component> handlingComponents, Type newMonoType) {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Replace Component");

            foreach (var oldMono in handlingComponents) {
                if (oldMono == null) return;

                var curGO = oldMono.gameObject;

                Undo.RegisterCompleteObjectUndo(curGO, string.Empty);
                
                var oldSO = new SerializedObject(oldMono);
                oldSO.Update();
                Undo.DestroyObjectImmediate(oldMono);
                
                var newMono = Undo.AddComponent(curGO, newMonoType);
                var newSO = new SerializedObject(newMono);
                newSO.Update();

                _CopySerializedObject(oldSO, newSO);

                newSO.ApplyModifiedProperties();

                EditorUtility.SetDirty(curGO);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private static void _CopySerializedObject(SerializedObject source, SerializedObject target) {
            var srcProp = source.GetIterator();

            if (srcProp.NextVisible(true)) {
                do {
                    if (srcProp.name == "m_Script") continue;

                    if (target.FindProperty(srcProp.name) != null) {
                        try {
                            target.CopyFromSerializedProperty(srcProp);
                        } catch { }
                    }
                } while (srcProp.NextVisible(false));
            }
        }
    }
}

#endif