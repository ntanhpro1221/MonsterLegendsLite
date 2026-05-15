using System.Threading.Tasks;
using Google;
using UnityEngine;

namespace MonsterLegendsLite.Auth.Mobile {
    public class GoogleAuthenticatorMobile : GoogleAuthenticatorBase {
        [RuntimeInitializeOnLoadMethod(OnAuthenticatorInit)]
        private static void Init() => Init(new GoogleAuthenticatorMobile());

        protected override async Task<GoogleAuthResult> AuthenticateAsync(GoogleAuthInput authInput) {
            if (GoogleSignIn.Configuration == null) {
                GoogleSignIn.Configuration = new GoogleSignInConfiguration {
                    ClientId       = authInput.WebClientId
                  , RequestIdToken = true
                  , RequestEmail   = true
                  , AdditionalScopes = new[] {
                        "https://www.googleapis.com/auth/drive.readonly"
                      , "https://www.googleapis.com/auth/calendar.events.readonly"
                    }
                };

                Debug.Log("Recreate configuration");
            }

            var user = await GoogleSignIn.DefaultInstance.SignInAsync();

            return new GoogleAuthResult {
                id_token     = user.IdToken
              , access_token = null
            };
        }
    }
}