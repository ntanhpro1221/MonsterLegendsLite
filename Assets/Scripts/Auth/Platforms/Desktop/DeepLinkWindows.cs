#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN) && (NET_4_6 || NET_UNITY_4_8)

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NGDtuanh.Auth.Desktop {
    /// <summary>
    /// Be careful: can only listen once at a time
    /// </summary>
    public class DeepLinkWindows : MonoBehaviour {
        private Action<string> callback;

        private string vbsPath;
        private string resPath;

        private bool isStopped;

        public static DeepLinkWindows StartListen(string appUri, Action<string> callback, bool dontDestroyOnLoad = true) {
            var instanceObj = new GameObject(nameof(DeepLinkWindows));
            if (dontDestroyOnLoad) DontDestroyOnLoad(instanceObj);

            var instance = instanceObj.AddComponent<DeepLinkWindows>();
            instance.Initialize(appUri, callback);

            return instance;
        }

        private void Initialize(string appUri, Action<string> callback) {
            this.callback = callback;

            vbsPath = Path.Combine(Application.temporaryCachePath, $"{Guid.NewGuid()}.vbs");
            resPath = Path.Combine(Application.temporaryCachePath, $"{Guid.NewGuid()}.txt");

            // Register uri
            using var key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\Classes\{appUri}") ?? throw new Exception("Unable to create registry key.");

            key.SetValue("URL Protocol", "");

            using var commandKey = key.CreateSubKey(@"shell\open\command") ?? throw new Exception("Unable to create registry sub key.");

            commandKey.SetValue("", $@"""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "wscript.exe")}"" ""{vbsPath}"" ""%1""");

            // Write vbs script
            File.WriteAllLines(vbsPath, new[] {
                $@"CreateObject(""Scripting.FileSystemObject"").CreateTextFile(""{resPath}"", True).WriteLine WScript.Arguments(0)"
              , $@"CreateObject(""WScript.Shell"").AppActivate {Process.GetCurrentProcess().Id}"
            });

            StartCoroutine(IEListen(.5f));
        }

        private IEnumerator IEListen(float sleepTime) {
            var sleepUnit = new WaitForSecondsRealtime(sleepTime);

            while (!isStopped && !TryResponse()) yield return sleepUnit;
        }

        private void OnApplicationFocus(bool focus) {
            if (!isStopped && focus) TryResponse();
        }

        private void OnDestroy() {
            StopListen();
        }

        private bool TryResponse() {
            string res;

            try {
                res = File.ReadAllText(resPath);
            } catch { return false; }

            try {
                callback?.Invoke(res);
            } catch (Exception e) { Debug.LogException(e); }

            StopListen();
            Destroy(gameObject);

            return true;
        }

        public void StopListen() {
            if (isStopped) return;
            isStopped = true;

            File.Delete(vbsPath);
            File.Delete(resPath);
        }
    }
}

#endif