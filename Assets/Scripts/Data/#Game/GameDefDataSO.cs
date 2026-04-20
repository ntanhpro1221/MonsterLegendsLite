using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [CreateAssetMenu(fileName = "GameDefDataSO", menuName = "Data/GameDefDataSO")]
    public class GameDefDataSO : ScriptableObject {
        [HideLabel]
        public GameDefData Data;
    }
}