using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [CreateAssetMenu(fileName = "UserInsDataSO", menuName = "Data/UserInsDataSO")]
    public class UserInsDataSO :  ScriptableObject {
        [HideLabel]
        public UserInsData Data;
    }
}