using UnityEngine;
using UnityEngine.UI;

namespace NGDtuanh.MonsterLegendsLite {
    [RequireComponent(typeof(RectTransform))]
    public class RebuildLayoutFixer : MonoBehaviour {
        private void Update() {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            enabled = false;
        }
    }
}