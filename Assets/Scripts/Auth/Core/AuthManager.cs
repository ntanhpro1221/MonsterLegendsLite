using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using NGDtuanh.MonsterLegendsLite;
using UnityEngine;

namespace MonsterLegendsLite.Auth {
    public class AuthManager : MonoBehaviourExt {
        private static readonly Dictionary<AuthProvider, IAuthenticator> authenticators = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize() {
            authenticators.Clear();
        }

        internal static void RegisterAuthenticator(AuthProvider provider, IAuthenticator authenticator, bool isFallback) {
            if (isFallback && authenticators.ContainsKey(provider)) return;
            authenticators[provider] = authenticator;
        }

        [SerializeField]
        private AllAuthInput allAuthInput;

        public async Task<FirebaseUser> SignIn(AuthProvider provider) {
            var credential = await authenticators[provider].AuthenticateAsync(allAuthInput);
            if (credential == null) throw new Exception("Could not authenticate user");

            var user = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);
            if (user == null) throw new Exception("Could not sign in with credential");

            return user;
        }
    }
}