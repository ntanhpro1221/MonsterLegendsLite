using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class SettingsWindow : PopupWindow {
        [SerializeField, Required]
        private Image avatarImg;
        
        [SerializeField, Required]
        private Button changeAvatarBtn;
        
        [SerializeField, Required]
        private TMP_InputField nameInput;

        [SerializeField, Required]
        private Button changeNameBtn;
        
        [SerializeField, Required]
        private ToggleExt musicToggle;
        
        [SerializeField, Required]
        private ToggleExt soundToggle;
        
        [SerializeField, Required]
        private ToggleExt vibrantToggle;

        [SerializeField, Required]
        private Button signOutBtn;

        public static SettingsWindow Show(SettingsWindow prefab) {
            var window = PopupWindowPool.Ins.Show(
                prefab: prefab
              , title: "SETTINGS"
              , content: ""
              , onDoneClose: null);

            window.SetAllData(DataManager.Ins.User);

            return window;
        }

        private void SetAllData(UserInsData data) {
            avatarImg.sprite = avatarImg.sprite;
            utils.SetListener(changeAvatarBtn, () => { });

            nameInput.text = data.Name;
            utils.SetListener(nameInput.onEndEdit, DataManager.Ins.UpdateData_UserName);
            utils.SetListener(changeNameBtn, () => nameInput.Select());

            SetToggle(musicToggle, data.Music, DataManager.Ins.UpdateData_UserMusic);
            SetToggle(soundToggle, data.Sound, DataManager.Ins.UpdateData_UserSound);
            SetToggle(vibrantToggle, data.Vibrant, DataManager.Ins.UpdateData_UserVibrant);

            utils.SetListener(signOutBtn, () => YesNoWindow.Show(
                title: "SIGN OUT"
              , content: "Are you sure you want to sign out of your current account?"
              , yesCallback: () => {
                    Debug.LogError("Faking sign out");
                    SceneManager.LoadScene("SignInScene");
                }));
        }

        private void SetToggle(ToggleExt toggle, bool isOn, UnityAction<bool> updateFunc) {
            toggle.isOn = isOn;
            toggle.onValueChanged.AddListener(updateFunc);
        }
    }
}