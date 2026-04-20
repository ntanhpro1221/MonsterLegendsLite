using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterLegendsLite.Auth.Desktop {
    public class GoogleAuthenticatorDesktop : GoogleAuthenticatorBase {
        [RuntimeInitializeOnLoadMethod(OnAuthenticatorInit)]
        private static void Init() => Init(new GoogleAuthenticatorDesktop(), isFallback: true);

        protected override async Task<GoogleAuthResult> AuthenticateAsync(GoogleAuthInput authInput) {
            if (GoogleSignIn.Configuration == null) {
                GoogleSignIn.Configuration = new GoogleSignInConfiguration {
                    ClientId    = authInput.AndroidClientId
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

        // protected override async Task<GoogleAuthResult> AuthenticateAsync(GoogleAuthInput authInput) {
        //     var codeVerifier  = GeneratePkceVerifier();
        //     var codeChallenge = Base64UrlEncode(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(codeVerifier)));
        //     var state         = Base64UrlEncode(Guid.NewGuid().ToByteArray());
        //     var listener      = LoopbackListenHelper.CreateAndStart(out string redirectUri);
        //
        //     try {
        //         Application.OpenURL("https://accounts.google.com/o/oauth2/v2/auth"
        //           + $"?client_id={Uri.EscapeDataString(authInput.DesktopClientId)}"
        //           + $"&redirect_uri={Uri.EscapeDataString(redirectUri)}"
        //           + $"&response_type=code"
        //           + $"&scope={Uri.EscapeDataString("openid email profile")}"
        //           + $"&code_challenge={codeChallenge}"
        //           + $"&code_challenge_method=S256"
        //           + $"&state={state}");
        //
        //         var context = await listener.GetContextAsync();
        //
        //         LoopbackListenHelper.SendHtmlResponse(context.Response);
        //
        //         LoopbackListenHelper.TryBringGameToFront();
        //
        //         var query = new Dictionary<string, string>();
        //         foreach (var part in context.Request.Url.Query.TrimStart('?').Split('&')) {
        //             var kv                                                   = part.Split('=');
        //             if (kv.Length == 2) query[Uri.UnescapeDataString(kv[0])] = Uri.UnescapeDataString(kv[1]);
        //         }
        //
        //         if (query.GetValueOrDefault("state") != state) throw new Exception("State mismatch!");
        //         if (query.TryGetValue("error", out var err)) throw new Exception($"Google Error: {err}");
        //         if (!query.TryGetValue("code", out var code)) throw new Exception("No code received.");
        //
        //         return await ExchangeCodeAsync(authInput.DesktopClientId, authInput.DesktopClientSecret, code, codeVerifier, redirectUri);
        //     } finally {
        //         listener.Stop();
        //         listener.Close();
        //     }
        // }

        private async Task<GoogleAuthResult> ExchangeCodeAsync(string clientId, string clientSecret, string code, string verifier, string redirectUri) {
            var form = new WWWForm();
            form.AddField("grant_type", "authorization_code");
            form.AddField("code", code);
            form.AddField("redirect_uri", redirectUri);
            form.AddField("client_id", clientId);
            form.AddField("client_secret", clientSecret);
            form.AddField("code_verifier", verifier);

            using var request = UnityWebRequest.Post("https://oauth2.googleapis.com/token", form);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) throw new Exception($"Token Exchange Failed: {request.error}\nDetail: {request.downloadHandler?.text}");

            return JsonConvert.DeserializeObject<GoogleAuthResult>(request.downloadHandler.text);
        }

        private string GeneratePkceVerifier() {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Base64UrlEncode(bytes);
        }

        private string Base64UrlEncode(byte[] bytes) {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}