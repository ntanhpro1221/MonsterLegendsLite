using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NGDtuanh.Utils.Editor {
    public static class GameObjectTools {
        private const string PATH = "GameObject/Tools/";
        private static readonly UtilFuncs utils = new();
        
        #region SORT CUT SPRITE

        private const string PATH_SORT_CUT_SPRITE_HOR = PATH + "Sort Cut Sprite - HORIZONTAL";
        private const string PATH_SORT_CUT_SPRITE_VER = PATH + "Sort Cut Sprite - VERTICAL";
        
        [MenuItem(PATH_SORT_CUT_SPRITE_HOR)]
        public static void SnapSpritesHorizontal(MenuCommand cmd) => SortCutSprite(true);

        [MenuItem(PATH_SORT_CUT_SPRITE_VER)]
        public static void SnapSpritesVertical(MenuCommand cmd) => SortCutSprite(false);

        [MenuItem(PATH_SORT_CUT_SPRITE_HOR, true)]
        [MenuItem(PATH_SORT_CUT_SPRITE_VER, true)]
        private static bool SortCutSprite_Validate() {
            var gos = Selection.transforms;
            
            if (gos.Length <= 1) return false;

            foreach (var go in gos) {
                foreach (var child in gos) {
                    if (go == child) continue;
                    if (child.IsChildOf(go)) return false;
                }

                if (!go.GetComponentsInChildren<SpriteRenderer>().Any(SortCutSprite_ValidateRenderer)) return false;
            }

            return true;
        }

        private static void SortCutSprite(bool isHorizontal) {
            var list = Selection.transforms.Select(t => new {
                root = t, rdr = t.GetComponentsInChildren<SpriteRenderer>().First(SortCutSprite_ValidateRenderer)
            }).ToList();

            var roots = Selection.transforms.Cast<Object>().ToArray();
            utils.RecordForUndo(roots);

            for (int i = 0; i < list.Count - 1; i++) {
                var cur  = list[i];
                var next = list[i + 1];

                var curBounds  = cur.rdr.bounds;
                var nextBounds = next.rdr.bounds;

                next.root.position += isHorizontal ? new Vector3(curBounds.max.x - nextBounds.min.x, 0) : new Vector3(0, curBounds.min.y - nextBounds.max.y, 0);
            }

            utils.MarkDirty(roots);
        }

        private static bool SortCutSprite_ValidateRenderer(SpriteRenderer rdr) =>
            rdr.gameObject.activeInHierarchy
         && rdr.enabled
         && rdr.sprite != null;

        #endregion
    }
}