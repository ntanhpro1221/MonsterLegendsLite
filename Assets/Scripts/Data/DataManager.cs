using NGDtuanh.MonsterLegends;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite.Data {
    /// TODO: Load, save data remotely
    public class DataManager : Singleton<DataManager> {
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), Required]
        private GameDefDataSO gameDefDataSO;

        public GameDefData GameDefData => gameDefDataSO.Data;

        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), Required]
        private GameLocDefDataSO gameLocDefDataSO;

        public GameLocDefData GameLocDefData => gameLocDefDataSO.Data;

        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout), Required]
        private UserInsDataSO userInsDataSO;

        public UserInsData UserInsData => userInsDataSO.Data;
    }
}