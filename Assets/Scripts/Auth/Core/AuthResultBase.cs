using Firebase.Auth;

namespace MonsterLegendsLite.Auth {
    public abstract class AuthResultBase {
        public abstract Credential ToCredential();
    }
}