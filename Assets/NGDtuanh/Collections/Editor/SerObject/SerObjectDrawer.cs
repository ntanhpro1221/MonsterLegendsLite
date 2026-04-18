using System;
using NGDtuanh.Utils.Editor;
using NGDtuanh.Utils.Editor.SearchWindow;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NGDtuanh.Collections.Editor {
    public class SerObjectDrawer<TValue> : OdinValueDrawer<SerObject<TValue>>, IDisposable where TValue : class {
        private const float PICK_BUTTON_WIDTH = 19;

        private GameObject rootPrefab;
        private bool allowSceneObjects;
        private InlineEditorImitator inlineEditorImitator;
        private InlineEditorAttribute inlineEditorAttr;

        protected override void Initialize() {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            rootPrefab           = prefabStage != null ? prefabStage.prefabContentsRoot : null;
            allowSceneObjects    = !EditorUtility.IsPersistent(Property.Tree.WeakTargets[0] as Object);
            inlineEditorImitator = new InlineEditorImitator(typeof(TValue) == typeof(GameObject), wrapper => ((SerObject<TValue>)wrapper).value);
            inlineEditorAttr     = Property.Children[nameof(SerObject<TValue>.value)].GetAttribute<InlineEditorAttribute>();

            if (inlineEditorAttr is { ExpandedHasValue: true }) Property.State.Expanded = true;
        }

        protected override void DrawPropertyLayout(GUIContent label) {
            if (inlineEditorAttr == null) DrawStandardField(EditorGUILayout.GetControlRect(hasLabel: false), label);
            else inlineEditorImitator.DrawLayout(Property, ValueEntry, inlineEditorAttr, label ?? Property.Label ?? GUIContent.none, DrawStandardField);
        }

        private void DrawStandardField(Rect position, GUIContent label) {
            HandlePickButton(position, label);

            EditorGUI.BeginChangeCheck();

            var newValue = SirenixEditorFields.UnityObjectField(
                position
              , label
              , ValueEntry.SmartValue.value
              , typeof(TValue)
              , allowSceneObjects);

            if (EditorGUI.EndChangeCheck()) ApplyNewValueToAllTargets(newValue as TValue);
        }

        private void HandlePickButton(Rect position, GUIContent label) {
            var buttonRect = new Rect(position.xMax - PICK_BUTTON_WIDTH, position.y, PICK_BUTTON_WIDTH, position.height);

            var evt = Event.current;
            if (evt.type == EventType.MouseDown
             && evt.button == 0
             && buttonRect.Contains(evt.mousePosition)) {
                evt.Use();

                ObjectSearchWindow.Open(
                    btnRect: buttonRect
                  , title: typeof(TValue).Name
                  , onSelected: node => ApplyNewValueToAllTargets(node.Data as TValue)
                  , filterType:typeof(TValue)
                  , rootPrefab: rootPrefab
                  , allowSceneObjects
                );
            }
        }

        private void ApplyNewValueToAllTargets(TValue newValue) {
            for (int i = 0; i < ValueEntry.ValueCount; i++) {
                ValueEntry.WeakValues[i] = new SerObject<TValue>(newValue);
            }

            ValueEntry.ApplyChanges();
        }

        public void Dispose() {
            inlineEditorImitator?.Dispose();
        }
    }
}