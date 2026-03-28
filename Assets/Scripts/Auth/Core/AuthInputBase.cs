using Sirenix.OdinInspector;

namespace NGDtuanh.Auth {
    public abstract class AuthInputBase : SerializedScriptableObject {
        public const string AssetPath = "Client Data/";

        public TAuthInput As<TAuthInput>() where TAuthInput : AuthInputBase => (TAuthInput)this;
    }
}