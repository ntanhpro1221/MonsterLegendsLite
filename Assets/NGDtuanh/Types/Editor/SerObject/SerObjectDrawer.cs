using System;
using NGDtuanh.Utils.Editor;
using NGDtuanh.Utils.Editor.SearchWindow;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NGDtuanh.Types.Editor {
    public class SerObjectDrawer<TValue> : OdinValueDrawer<SerObject<TValue>>, IDisposable where TValue : class {
        private const float PICK_BUTTON_WIDTH = 19;

        private GameObject rootPrefab;
        private bool allowSceneObjects;
        private SerializedProperty unityValueProp;
        private InlineEditorImitator inlineEditorImitator;
        private InlineEditorAttribute inlineEditorAttr;

        protected override void Initialize() {
            var targetObj = Property.Tree.WeakTargets[0] as Object;
            var valueProp = Property.Children[nameof(SerObject<TValue>.value)];

            rootPrefab           = UtilFuncs.Ins.GetRootPrefab(targetObj);
            allowSceneObjects    = !EditorUtility.IsPersistent(targetObj);
            unityValueProp       = Property.Tree.UnitySerializedObject.FindProperty(valueProp.UnityPropertyPath);
            inlineEditorImitator = new InlineEditorImitator(typeof(TValue) == typeof(GameObject), wrapper => ((SerObject<TValue>)wrapper).value);
            inlineEditorAttr     = valueProp.GetAttribute<InlineEditorAttribute>();

            if (inlineEditorAttr is { ExpandedHasValue: true }) Property.State.Expanded = true;
        }

        protected override void DrawPropertyLayout(GUIContent label) {
            if (inlineEditorAttr == null) DrawStandardField(EditorGUILayout.GetControlRect(hasLabel: false), label);
            else inlineEditorImitator.DrawLayout(Property, ValueEntry, inlineEditorAttr, label ?? Property.Label ?? GUIContent.none, DrawStandardField);
        }

        private void DrawStandardField(Rect position, GUIContent label) {
            HandlePickButton(position);

            var newLabel = EditorGUI.BeginProperty(position, label, unityValueProp);
            EditorGUI.BeginChangeCheck();

            var newValue = SirenixEditorFields.UnityObjectField(
                position
              , label == null ? null : newLabel
              , ValueEntry.SmartValue.value
              , typeof(TValue)
              , allowSceneObjects);

            if (EditorGUI.EndChangeCheck()) ApplyNewValue(newValue as TValue);
            EditorGUI.EndProperty();
        }

        private void HandlePickButton(Rect position) {
            var buttonRect = new Rect(position.xMax - PICK_BUTTON_WIDTH, position.y, PICK_BUTTON_WIDTH, position.height);

            var evt = Event.current;
            if (evt.type == EventType.MouseDown
             && evt.button == 0
             && buttonRect.Contains(evt.mousePosition)) {
                evt.Use();

                ObjectSearchWindow.Open(
                    btnRect: buttonRect
                  , title: typeof(TValue).Name
                  , onSelected: node => ApplyNewValue(node.Data as TValue)
                  , filterType: typeof(TValue)
                  , rootPrefab: rootPrefab
                  , allowSceneObjects
                );
            }
        }

        private void ApplyNewValue(TValue newValue) {
            unityValueProp.objectReferenceValue = newValue as Object;
            unityValueProp.serializedObject.ApplyModifiedProperties();
        }

        public void Dispose() {
            inlineEditorImitator?.Dispose();
        }
    }
}