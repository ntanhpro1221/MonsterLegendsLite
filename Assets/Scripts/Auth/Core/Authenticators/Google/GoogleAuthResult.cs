using Firebase.Auth;

namespace MonsterLegendsLite.Auth {
    public class GoogleAuthResult : AuthResultBase {
        public string id_token, access_token;
        
        public override Credential ToCredential() => GoogleAuthProvider.GetCredential(
            idToken: id_token
          , accessToken: access_token);
    }
}