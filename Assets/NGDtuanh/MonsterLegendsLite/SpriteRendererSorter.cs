using UnityEngine;
using Sirenix.OdinInspector;

namespace NGDtuanh.MonsterLegendsLite {
    public class SpriteRendererSorter : MonoBehaviourExt {
        [SerializeField, Required, InlineButton(nameof(FindSprites), "Find")]
        private SpriteRenderer[] sprites;

        [Button]
        private void SortHorizontal() => Sort(isHorizontal: true);

        [Button]
        private void SortVertical() => Sort(isHorizontal: false);

        private void Sort(bool isHorizontal) {
            if (sprites.Length == 0) return;

            Vector2 prevBoundMax = sprites[0].bounds.min;
            foreach (var sprite in sprites) {
                var spriteTF  = sprite.transform;
                var boundsMin = sprite.bounds.min;
                var newPos    = spriteTF.position;

                RecordForUndo(spriteTF);

                if (isHorizontal)
                    newPos.x  += prevBoundMax.x - boundsMin.x;
                else newPos.y += prevBoundMax.y - boundsMin.y;

                spriteTF.position = newPos;
                prevBoundMax      = sprite.bounds.max;

                MarkDirty(spriteTF);
            }
        }

        private void FindSprites() {
            RecordForUndo(this);

            sprites = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

            MarkDirty(this);
        }

        private void Reset() {
            FindSprites();
        }
    }
}