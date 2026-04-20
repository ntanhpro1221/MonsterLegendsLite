using NGDtuanh.Types;
using UnityEngine;

namespace MonsterLegendsLite.Auth {
    [CreateAssetMenu(fileName = nameof(AllAuthInput), menuName = AuthInputBase.AssetPath + "All", order = -1)]
    public class AllAuthInput : ScriptableObject {
        [SerializeField]
        private EnumMap<AuthProvider, AuthInputBase> data;

        public AuthInputBase this[AuthProvider provider] => data[provider];
    }
}