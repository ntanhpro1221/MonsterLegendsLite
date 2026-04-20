using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using NGDtuanh.MonsterLegends;
using QFSW.QC;
using Sirenix.Serialization;
using UnityEngine;

namespace MonsterLegendsLite.Auth {
    public class AuthManager : Singleton<AuthManager> {
        [OdinSerialize, NonSerialized]
        private AllAuthInput allAuthInput;

        private readonly Dictionary<AuthProvider, IAuthenticator> authenticators = new();

        public bool IsEditor { get; private set; }
        public string ProductName { get; private set; }
        public string Identifier { get; private set; }

        public static event Action onRegisterAuthenticator;

        protected override async void Awake() {
            base.Awake();

            IsEditor    = Application.isEditor;
            ProductName = Application.productName;
            Identifier  = Application.identifier;

            onRegisterAuthenticator?.Invoke();

            try {
                var dependencyState = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (dependencyState != DependencyStatus.Available) {
                    throw new Exception($"Could not resolve all Firebase dependencies. Status: [{dependencyState}]");
                }

                _ = FirebaseAuth.DefaultInstance;
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public void RegisterAuthenticator(AuthProvider provider, IAuthenticator authenticator, bool isFallback) {
            if (isFallback && authenticators.ContainsKey(provider)) return;
            authenticators.Add(provider, authenticator);
        }

        public async Task SignIn(AuthProvider provider) {
            var credential = await authenticators[provider].AuthenticateAsync(allAuthInput);
            if (credential == null) {
                throw new Exception("Could not authenticate user");
            }

            var user = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);
            if (user == null) {
                throw new Exception("Could not sign in with credential");
            }
        }

        [Command]
        public async void TestSignIn() {
            Debug.Log("Start sign in");
            try {
                await SignIn(AuthProvider.Google);
                await DBTester.Ins.TestRealtimeDatabasePing();
            } catch (OperationCanceledException) {
                Debug.LogError("User cancelled sign in");
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        [Command]
        public async void TestSignInSilently() {
            Debug.Log("Start sign in silently");
            try {
                if (Google.GoogleSignIn.Configuration == null) {
                    Google.GoogleSignIn.Configuration = new Google.GoogleSignInConfiguration {
                        ClientId    = allAuthInput[AuthProvider.Google].As<GoogleAuthInput>().WebClientId
                      , RequestIdToken = true
                      , RequestEmail   = true
                      , AdditionalScopes = new[] {
                            "https://www.googleapis.com/auth/drive.readonly"
                          , "https://www.googleapis.com/auth/calendar.events.readonly"
                        }
                    };
            
                    Debug.Log("Recreate configuration");
                }
            
                var ggUser = await Google.GoogleSignIn.DefaultInstance.SignInSilentlyAsync();
            
                var credential = GoogleAuthProvider.GetCredential(ggUser.IdToken, null);
                if (credential == null) {
                    throw new Exception("Could not authenticate user");
                }
            
                var user = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);
                if (user == null) {
                    throw new Exception("Could not sign in with credential");
                }
            
                await DBTester.Ins.TestRealtimeDatabasePing();
            } catch (OperationCanceledException) {
                Debug.LogError("User cancelled sign in");
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        [Command]
        public void TestSignOut() {
            Debug.Log("Start sign out");
            
            FirebaseAuth.DefaultInstance.SignOut();
            
            Google.GoogleSignIn.DefaultInstance.SignOut();
            
            Debug.Log("Signed out");
        }

        private void OnApplicationQuit() {
            onRegisterAuthenticator = null;
        }
    }
}