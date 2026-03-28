using Firebase.Auth;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using UnityEngine;
using NGDtuanh.Auth;
using QFSW.QC;

public class AuthTester : Singleton<AuthTester> {
    private static FirebaseAuth _FirebaseService
        => FirebaseAuth.DefaultInstance;

    public static bool IsSignedIn =>
        _FirebaseService.CurrentUser != null &&
        _FirebaseService.CurrentUser.IsValid();

    public static FirebaseUser User => _FirebaseService.CurrentUser;

    public static async Task SignInWithGoogle(Action<bool> callback) {
        try {
            var success = await AuthManager.Instance.SignIn(AuthProvider.Google);

            callback?.Invoke(success);
        } catch (Exception e) {
            Debug.LogException(e);
            callback?.Invoke(false);
        }
    }

    [Command]
    public void TestSignIn() {
        Debug.Log("Bắt đầu tiến trình đăng nhập...");
        _ = SignInWithGoogle(success => {
            Debug.Log("Done");
            if (success) {
                Debug.Log($"[AUTH] Đăng nhập thành công: {User.DisplayName} ({User.Email})");

                // Kích hoạt bài test Database ngay sau khi có User ID
                _ = TestRealtimeDatabasePing();
            } else {
                Debug.LogError("[AUTH] Đăng nhập thất bại.");
            }
        });
    }

    private async Task TestRealtimeDatabasePing() {
        if (User == null) return;

        // 1. Trỏ vào một nhánh dành riêng cho User này để test
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.GetReference($"desktop_test_users/{User.UserId}");

        // 2. Tạo dữ liệu để ghi
        string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log($"[DB] Đang thử GHI dữ liệu: {currentTime}...");

        // 3. Thực hiện GHI (Set Value)
        await dbRef.Child("last_login_ping").SetValueAsync(currentTime);

        Debug.Log("[DB] GHI thành công! Đang thử ĐỌC lại dữ liệu vừa ghi...");

        var snapshot = await dbRef.Child("last_login_ping").GetValueAsync();

        if (snapshot.Exists) {
            Debug.Log($"[DB] ĐỌC thành công! Dữ liệu từ Server trả về: {snapshot.Value}");
            Debug.Log("==== BÀI TEST DESKTOP HOÀN HẢO TỪ A-Z ====");
        } else {
            Debug.LogWarning("[DB] Đọc được nhưng không thấy dữ liệu (Snapshot rỗng).");
        }
    }
}