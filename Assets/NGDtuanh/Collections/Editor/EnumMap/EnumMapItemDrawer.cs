using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace NGDtuanh.Collections.Editor {
    internal class EnumMapItemDrawer<T> : OdinValueDrawer<EnumMapItem<T>> {
        protected override void DrawPropertyLayout(GUIContent label) {
            Property.Children[nameof(EnumMapItem<T>.value)].Draw(label);
        }
    }
}