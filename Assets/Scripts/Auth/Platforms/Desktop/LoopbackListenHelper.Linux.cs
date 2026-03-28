#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX

using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace NGDtuanh.Auth.Desktop {
    public static partial class LoopbackListenHelper {
        public static partial void TryBringGameToFront() {
            Task.Run(() => {
                var windowName = AuthManager.Instance.IsEditor ? "Unity" : AuthManager.Instance.ProductName;
                
                if (TryRunLinuxCommand("wmctrl", $"-a \"{windowName}\"")) {
                    Debug.Log("[Auth] Linux BringToFront succeeded via wmctrl.");
                    return;
                }

                if (TryRunLinuxCommand("xdotool", $"search --name \"{windowName}\" windowactivate")) {
                    Debug.Log("[Auth] Linux BringToFront succeeded via xdotool.");
                    return;
                }

                Debug.LogWarning("[Auth] Linux BringToFront failed. Wayland active or missing tools (wmctrl, xdotool).");
            }).ContinueWith(task => {
                if (task.IsCanceled) Debug.LogError("[Auth] Linux BringToFront canceled.");
                
                if (task.IsFaulted) Debug.LogException(task.Exception);
            });
        }

        private static bool TryRunLinuxCommand(string toolName, string arguments) {
            try {
                var psi = new ProcessStartInfo {
                    FileName               = toolName
                  , Arguments              = arguments
                  , UseShellExecute        = false
                  , CreateNoWindow         = true
                  , RedirectStandardError  = false
                  , RedirectStandardOutput = false
                };

                using var proc = Process.Start(psi);
                if (proc == null) return false;

                if (!proc.WaitForExit(1000)) {
                    proc.Kill();
                    return false;
                }

                return proc.ExitCode == 0;
            } catch {
                return false;
            }
        }
    }
}

#endif