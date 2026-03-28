using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Sirenix.Serialization;
using UnityEngine;

namespace NGDtuanh.Auth {
    public class AuthManager : Singleton<AuthManager> {
        [OdinSerialize, NonSerialized]
        private AllAuthInput allAuthInput;

        private readonly Dictionary<AuthProvider, IAuthenticator> authenticators = new();

        public bool IsEditor { get; private set; }
        public string ProductName { get; private set; }
        public string Identifier { get; private set; }

        public static event Action onRegisterAuthenticator;

        protected override void Awake() {
            base.Awake();

            IsEditor    = Application.isEditor;
            ProductName = Application.productName;
            Identifier  = Application.identifier;

            onRegisterAuthenticator?.Invoke();
        }

        public void RegisterAuthenticator(AuthProvider provider, IAuthenticator authenticator, bool isFallback) {
            Debug.Log(authenticator.GetType().Name);
            if (isFallback && authenticators.ContainsKey(provider)) return;
            authenticators.Add(provider, authenticator);
        }

        public async Task<bool> SignIn(AuthProvider provider) {
            var dependencyState = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyState != DependencyStatus.Available) {
                throw new Exception($"Could not resolve all Firebase dependencies. Status: [{dependencyState}]");
            }
            
            _ = FirebaseAuth.DefaultInstance;

            var credential = await authenticators[provider].AuthenticateAsync(allAuthInput);
            if (credential == null) {
                Debug.LogError("Could not authenticate user");
                return false;
            }

            var user = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);
            if (user == null) {
                Debug.LogError("Could not sign in with credential");
                return false;
            }

            return true;
        }

        private void OnApplicationQuit() {
            onRegisterAuthenticator = null;
        }
    }
}