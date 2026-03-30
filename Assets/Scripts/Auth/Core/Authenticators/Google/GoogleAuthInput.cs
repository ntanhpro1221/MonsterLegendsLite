using System;
using Sirenix.Serialization;
using UnityEngine;

namespace NGDtuanh.Auth {
    [CreateAssetMenu(fileName = nameof(AuthProvider.Google), menuName = AssetPath + nameof(AuthProvider.Google))]
    public class GoogleAuthInput : AuthInputBase {
        [OdinSerialize] [field: NonSerialized] public string WebClientId { get; private set; }
        [OdinSerialize] [field: NonSerialized] public string WebClientSecret { get; private set; }
        [OdinSerialize] [field: NonSerialized] public string DesktopClientId { get; private set; }
        [OdinSerialize] [field: NonSerialized] public string DesktopClientSecret { get; private set; }
        [OdinSerialize] [field: NonSerialized] public string AndroidClientId { get; private set; }
    }
}