using System;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NGDtuanh.Types.Editor {
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class SerObjectListDrawer<TList, TItem> : OdinValueDrawer<TList> where TList : IList<TItem> {
        private Type valueType;
        private bool needDrawDragDropHighlight;

        protected override void Initialize() {
            valueType = typeof(TItem).GenericTypeArguments[0];
        }

        public override bool CanDrawTypeFilter(Type type) {
            if (!typeof(TItem).IsGenericType) return false;
            if (typeof(TItem).GetGenericTypeDefinition() != typeof(SerObject<>)) return false;

            return true;
        }

        protected override void DrawPropertyLayout(GUIContent label) {
            var trueEvtType = Event.current.type;
            
            CallNextDrawer(label);
            
            HandleDragDrop(trueEvtType);

            DrawDragDropHighlight(trueEvtType);
        }

        private void HandleDragDrop(EventType trueEvtType) {
            if (DragAndDrop.visualMode != DragAndDropVisualMode.None) return;

            if (trueEvtType is not (EventType.DragUpdated or EventType.DragPerform)) return;

            var rect = Property.LastDrawnValueRect;
            var evt  = Event.current;
            if (!rect.Contains(evt.mousePosition)) return;

            if (!DragAndDrop.objectReferences.Any(IsValidObject)) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (trueEvtType is EventType.DragPerform) {
                DragAndDrop.AcceptDrag();

                var newList = ValueEntry.SmartValue?.ToList() ?? new List<TItem>();

                foreach (var objRef in DragAndDrop.objectReferences) {
                    if (!IsValidObject(objRef, out var item)) continue;

                    newList.Add(item);
                }

                var newValue = typeof(TList).IsArray
                    ? (TList)(object)newList.ToArray()
                    : (TList)(object)newList;
                
                for (int i = 0; i < ValueEntry.ValueCount; i++) {
                    ValueEntry.WeakValues[i] = newValue;
                }

                ValueEntry.ApplyChanges();
            } else needDrawDragDropHighlight = true;

            evt.Use();
        }

        private void DrawDragDropHighlight(EventType trueEvtType) {
            if (trueEvtType is not EventType.Repaint) return;

            if (!needDrawDragDropHighlight) return;
            needDrawDragDropHighlight = false;

            Handles.DrawSolidRectangleWithOutline(
                Property.LastDrawnValueRect
              , faceColor: new Color(0.3f, 0.6f, 1f, 0.1f)
              , outlineColor: new Color(0.3f, 0.6f, 1f, 1f));
        }

        private bool IsValidObject(Object obj) {
            return IsValidObject(obj, out _);
        }

        private bool IsValidObject(Object obj, out TItem item) {
            if (valueType.IsAssignableFrom(obj.GetType())) {
                item = (TItem)Activator.CreateInstance(typeof(TItem), new object[] { obj });
                return true;
            }

            if (obj is GameObject go
             && go.TryGetComponent(valueType, out var cpn)) {
                item = (TItem)Activator.CreateInstance(typeof(TItem), new object[] { cpn });
                return true;
            }

            item = default;
            return false;
        }
    }
}