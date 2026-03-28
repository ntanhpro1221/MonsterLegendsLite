using Firebase.Auth;

namespace NGDtuanh.Auth {
    public abstract class AuthResultBase {
        public abstract Credential ToCredential();
    }
}