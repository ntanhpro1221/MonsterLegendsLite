using UnityEngine;

namespace MonsterLegendsLite.Auth {
    public abstract class AuthInputBase : ScriptableObject {
        public const string AssetPath = "Client Data/";

        public TAuthInput As<TAuthInput>() where TAuthInput : AuthInputBase => (TAuthInput)this;
    }
}