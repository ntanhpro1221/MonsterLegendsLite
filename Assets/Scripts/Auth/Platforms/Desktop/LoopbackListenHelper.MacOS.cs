#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace NGDtuanh.Auth.Desktop {
    public static partial class LoopbackListenHelper {
        public static partial void TryBringGameToFront() {
            Task.Run(async () => {
                if (!AuthManager.Instance.IsEditor
                 && await RunOsascriptAsync($"-e 'if application id \"{AuthManager.Instance.Identifier}\" is running then tell application id \"{AuthManager.Instance.Identifier}\" to activate'")) {
                    Debug.Log("[Auth] MacOS BringToFront succeeded via bundleId.");
                    return;
                }

                if (await RunOsascriptAsync($"-e 'tell application \"System Events\" to set frontmost of every process whose unix id is {Process.GetCurrentProcess().Id} to true'")) {
                    Debug.LogWarning("[Auth] MacOS BringToFront succeeded via PID.");
                    return;
                }

                Debug.LogWarning("[Auth] MacOS BringToFront failed.");
            }).ContinueWith(task => {
                if (task.IsCanceled) Debug.LogError("[Auth] MacOS BringToFront canceled.");
                
                if (task.IsFaulted) Debug.LogException(task.Exception);
            });
        }

        private static async Task<bool> RunOsascriptAsync(string args) {
            try {
                var psi = new ProcessStartInfo {
                    FileName               = "/usr/bin/osascript"
                  , Arguments              = args
                  , UseShellExecute        = false
                  , CreateNoWindow         = true
                  , RedirectStandardOutput = true
                  , RedirectStandardError  = true
                };

                using var proc = Process.Start(psi);
                if (proc == null) return false;

                var stdoutTask  = proc.StandardOutput.ReadToEndAsync();
                var stderrTask  = proc.StandardError.ReadToEndAsync();
                var bothDone    = Task.WhenAll(stdoutTask, stderrTask);
                var timeoutTask = Task.Delay(1000);

                if (await Task.WhenAny(bothDone, timeoutTask) == timeoutTask) {
                    proc.Kill();
                    Debug.LogWarning("[Auth] osascript timed out.");
                    return false;
                }

                proc.WaitForExit();

                var stderr = stderrTask.Result;
                if (!string.IsNullOrWhiteSpace(stderr)) Debug.LogWarning($"[Auth] osascript stderr: {stderr.Trim()}");

                return proc.ExitCode == 0;
            } catch (Exception e) {
                Debug.LogWarning($"[Auth] osascript failed: {e.Message}");
                return false;
            }
        }
    }
}

#endif