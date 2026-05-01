using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite.Concretes {
    public class UI_Elements : UI_SpecStat {
        [SerializeField, Required]
        public Image prefabElement;
        
        [SerializeField, Required]
        public RectTransform elementRoot;

        private readonly List<Image> availableElements = new(1);
        private readonly List<Image> usingElements = new(1);

        public void SetElements(List<Sprite> elementSprites) {
            if (prefabElement.transform.parent == elementRoot) prefabElement.gameObject.SetActive(false);
            foreach (var element in usingElements) {
                element.gameObject.SetActive(false);
                availableElements.Add(element);
            }
            usingElements.Clear();

            foreach (var sprite in elementSprites) {
                Image element;
                if (availableElements.Count > 0) {
                    element = availableElements[0];
                    availableElements.RemoveAt(0);
                } else element = Instantiate(prefabElement, elementRoot);

                usingElements.Add(element);
                element.gameObject.SetActive(true);
                element.sprite = sprite;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(elementRoot);
        }
    }
}