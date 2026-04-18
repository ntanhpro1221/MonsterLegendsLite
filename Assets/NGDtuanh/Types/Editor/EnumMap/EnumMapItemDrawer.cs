using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace NGDtuanh.Types.Editor {
    internal class EnumMapItemDrawer<T> : OdinValueDrawer<EnumMapItem<T>> {
        protected override void DrawPropertyLayout(GUIContent label) {
            Property.Children[nameof(EnumMapItem<T>.value)].Draw(label);
        }
    }
}