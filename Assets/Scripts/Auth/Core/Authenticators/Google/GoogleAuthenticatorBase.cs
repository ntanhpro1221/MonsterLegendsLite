namespace MonsterLegendsLite.Auth {
    public abstract class GoogleAuthenticatorBase : AuthenticatorBase<GoogleAuthResult, GoogleAuthInput> {
        protected override AuthProvider Provider => AuthProvider.Google;
    }
}