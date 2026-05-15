using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using MonsterLegendsLite.Auth;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using QFSW.QC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class SignIn_SceneManager : SceneSingleton<SignIn_SceneManager> {
        [SerializeField, Required]
        private AuthManager authMan;

        [SerializeField, Required]
        private SignIn_ProgressBar progressBar;

        [SerializeField, Required]
        private EnumMap<AuthProvider, Button> signInBtns;

        protected override async void Initialize() {
            base.Initialize();

            try {
                LoadingIcon.Ins.Show(blockInteract: false);
                
                progressBar.SetVisible(false);
                SetVisibleSignInBtns(false);

                foreach (var (provider, button) in signInBtns) {
                    button.onClick.AddListener(() => _ = SignInAsync(provider));
                }
                
                var dependencyState = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (dependencyState != DependencyStatus.Available) {
                    throw new Exception($"Could not resolve all Firebase dependencies. Status: [{dependencyState}]");
                }

                // Force-initialize here to ensure firebase works properly, even though we reference it again below.
                _ = FirebaseAuth.DefaultInstance;
                
                FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
                
                LoadingIcon.Ins.Hide();

                if (FirebaseAuth.DefaultInstance.CurrentUser != null) {
                    await OnSignedInAsync();
                } else {
                    progressBar.SetVisible(false);
                    SetVisibleSignInBtns(true);
                }
            } catch (Exception e) {
                UtilFuncs.Ins.LogExceptionWithWindow(e);
            }
        }

        private void SetVisibleSignInBtns(bool visible) {
            foreach (var btn in signInBtns.Values) btn.gameObject.SetActive(visible);
        }

        [Command, Button]
        public async Task SignInAsync(AuthProvider provider) {
            try {
                await authMan.SignIn(provider);
                await OnSignedInAsync();
            } catch (OperationCanceledException) {
                Debug.LogWarning("User cancelled sign in");
            } catch (Exception e) {
                UtilFuncs.Ins.LogExceptionWithWindow(e);
            }
        }

        private async Task OnSignedInAsync() {
            progressBar.SetVisible(true);
            SetVisibleSignInBtns(false);

            await DataManager.Ins.LoadDataAsync(progressBar.SetProgress);

            SceneManager.LoadScene("HomeScene");
        }
    }
}