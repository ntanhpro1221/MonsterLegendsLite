using NGDtuanh.Types;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterLegendsLite {
    public class Shop_SceneManager : SceneSingleton<Shop_SceneManager> {
        [SerializeField]
        private Shop_ItemList[] uiItemLists;

        protected override void Initialize() {
            base.Initialize();

            foreach (var itemList in uiItemLists) itemList.Initialize();
        }

        public void BackToHomeScene() {
            SceneManager.LoadScene("HomeScene");
        }
    }
}