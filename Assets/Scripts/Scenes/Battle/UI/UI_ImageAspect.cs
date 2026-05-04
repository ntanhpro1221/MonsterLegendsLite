using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class UI_ImageAspect : MonoBehaviourExt {
        [SerializeField, Required]
        private Image image;

        [SerializeField, Required]
        private AspectRatioFitter aspect;

        public Image Image => image;

        public void SetSprite(Sprite sprite) {
            image.sprite = sprite;
            UpdateAspect();
        }

        [Button]
        private void UpdateAspect() {
            RecordForUndo(aspect);

            aspect.aspectRatio = image.sprite.rect.width / image.sprite.rect.height;

            MarkDirty(aspect);
        }

        private void Reset() {
            if (null == image
             && null == (image = GetComponent<Image>()))
                image = utils.AddComponentUndo<Image>(gameObject);

            if (null == aspect
             && null == (aspect = GetComponent<AspectRatioFitter>()))
                aspect = utils.AddComponentUndo<AspectRatioFitter>(gameObject);
        }
    }
}