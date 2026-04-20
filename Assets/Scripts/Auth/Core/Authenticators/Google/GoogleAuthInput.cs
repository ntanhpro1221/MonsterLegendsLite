using UnityEngine;

namespace MonsterLegendsLite.Auth {
    [CreateAssetMenu(fileName = nameof(AuthProvider.Google), menuName = AssetPath + nameof(AuthProvider.Google))]
    public class GoogleAuthInput : AuthInputBase {
        [field: SerializeField] public string WebClientId { get; private set; }
        [field: SerializeField] public string WebClientSecret { get; private set; }
        [field: SerializeField] public string DesktopClientId { get; private set; }
        [field: SerializeField] public string DesktopClientSecret { get; private set; }
        [field: SerializeField] public string AndroidClientId { get; private set; }
    }
}