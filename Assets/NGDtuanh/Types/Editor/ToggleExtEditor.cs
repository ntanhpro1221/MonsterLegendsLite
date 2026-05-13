using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace NGDtuanh.Types.Editor {
    [CustomEditor(typeof(ToggleExt), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class ToggleExtEditor : ToggleEditor {
        private SerializedProperty onValueChangedRevert_Prop;
        private SerializedProperty targetGraphic_OffProp, targetGraphic_OnProp;

        private bool dynamicTargetGraphicFoldout = true;

        protected override void OnEnable() {
            base.OnEnable();
            onValueChangedRevert_Prop = serializedObject.FindProperty(nameof(ToggleExt.onValueChangedRevert));
            targetGraphic_OffProp     = serializedObject.FindProperty(nameof(ToggleExt.targetGraphic_Off));
            targetGraphic_OnProp      = serializedObject.FindProperty(nameof(ToggleExt.targetGraphic_On));
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();
            
            EditorGUILayout.PropertyField(onValueChangedRevert_Prop);

            dynamicTargetGraphicFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(dynamicTargetGraphicFoldout, "Dynamic Target Graphic");

            if (dynamicTargetGraphicFoldout) {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(targetGraphic_OffProp);
                EditorGUILayout.PropertyField(targetGraphic_OnProp);

                var toggleExt = (ToggleExt)target;
                if (toggleExt.targetGraphic_Off != null
                 && toggleExt.targetGraphic_On != null
                 && GUILayout.Button("Update Target Graphic"))
                    toggleExt.UpdateTargetGraphic();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}