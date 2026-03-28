using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace NGDtuanh.Auth {
    [CreateAssetMenu(fileName = nameof(AllAuthInput), menuName = AuthInputBase.AssetPath + "All", order = -1)]
    public class AllAuthInput : SerializedScriptableObject {
        private struct AuthInputBaseWrapper {
            [HideLabel]
            [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
            public AuthInputBase value;
        }

        [OdinSerialize, NonSerialized]
        private Dictionary<AuthProvider, AuthInputBaseWrapper> data = new();

        public AuthInputBase this[AuthProvider provider] => data[provider].value;
    }
}