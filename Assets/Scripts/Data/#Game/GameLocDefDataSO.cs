using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    [CreateAssetMenu(fileName = "GameLocDefDataSO", menuName = "Data/GameLocDefDataSO")]
    public class GameLocDefDataSO : ScriptableObject {
        [HideLabel]
        public GameLocDefData Data;
    }
}