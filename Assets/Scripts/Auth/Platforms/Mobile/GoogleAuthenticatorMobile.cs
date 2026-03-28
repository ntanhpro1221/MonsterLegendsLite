using System.Threading.Tasks;
using Google;
using UnityEngine;

namespace NGDtuanh.Auth.Android {
    public class GoogleAuthenticatorMobile : GoogleAuthenticatorBase {
        [RuntimeInitializeOnLoadMethod(OnAuthenticatorInit)]
        private static void Init() => Init(new GoogleAuthenticatorMobile());

        protected override async Task<GoogleAuthResult> AuthenticateAsync(GoogleAuthInput authInput) {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration {
                WebClientId    = authInput.WebClientId
              , RequestIdToken = true
              , RequestEmail   = true
            };

            var user = await GoogleSignIn.DefaultInstance.SignIn();

            return new GoogleAuthResult {
                id_token     = user.IdToken
              , access_token = null
            };
        }
    }
}