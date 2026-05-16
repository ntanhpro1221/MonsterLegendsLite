using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace MonsterLegendsLite {
    public abstract class Shop_Item : MonoBehaviourExt {
        [field: SerializeField, Required]
        protected Shop_ItemSharedData SharedData { get; set; }

        public abstract string GetFailBuyWindowTitle();

        public Shop_Item SetTitle(string title) {
            SharedData.TitleTxt.text = title;
            return this;
        }
        
        public Shop_Item SetAvatar(Sprite avatar) {
            SharedData.AvatarImg.sprite = avatar;
            SharedData.AvatarRatio.aspectRatio = avatar.rect.width / avatar.rect.height;
            return this;
        }

        public Shop_Item SetBuyBtn(int value, UnityAction callback) {
            SharedData.BuyBtn.SetText(utils.ToStr_Resource(value));
            SharedData.BuyBtn.SetCallback(callback);
            return this;
        }
    }
}