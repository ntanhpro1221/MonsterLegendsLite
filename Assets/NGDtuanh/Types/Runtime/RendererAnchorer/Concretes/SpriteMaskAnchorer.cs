using NGDtuanh.Utils;
using UnityEngine;

namespace NGDtuanh.Types {
    public class SpriteMaskAnchorer : RendererAnchorer<SpriteMask> {
        protected override Matrix4x4 GetTargetL2WMatrix() {
            if (Target == null) return Matrix4x4.identity;

            return
                Target.transform.localToWorldMatrix
              * Matrix4x4.Translate(utils.With(Target.localBounds.min, UtilFuncs.VecAxis.Z, 0));
        }

        protected override Vector2 GetTargetSize() {
            if (Target == null) return Vector2.zero;

            return Target.localBounds.size;
        }
    }
}