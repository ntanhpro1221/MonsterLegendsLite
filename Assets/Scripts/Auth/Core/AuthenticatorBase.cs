using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

namespace MonsterLegendsLite.Auth {
    public abstract class AuthenticatorBase<TAuthResult, TAuthInput>
        : IAuthenticator
        where TAuthResult : AuthResultBase
        where TAuthInput : AuthInputBase {
        protected const RuntimeInitializeLoadType OnAuthenticatorInit = RuntimeInitializeLoadType.BeforeSceneLoad;

        protected static void Init(AuthenticatorBase<TAuthResult, TAuthInput> authenticator, bool isFallback = false) {
            AuthManager.RegisterAuthenticator(authenticator.Provider, authenticator, isFallback);
        }

        public async Task<Credential> AuthenticateAsync(AllAuthInput allAuthInput) {
            Debug.Log(allAuthInput[Provider] == null);
            var authResult = await AuthenticateAsync(allAuthInput[Provider].As<TAuthInput>());
            var credential = authResult.ToCredential();
            return credential;
        }

        protected abstract AuthProvider Provider { get; }
        protected abstract Task<TAuthResult> AuthenticateAsync(TAuthInput authInput);
    }
}