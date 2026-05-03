using NGDtuanh.MonsterLegendsLite;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_Info<TTarget> : Home_UI_Info where TTarget : Object {
        public TTarget CurTarget => CurTargetWeak == null ? null : (TTarget)CurTargetWeak;

        public sealed override void LoadInfoFor(Object building) {
            base.LoadInfoFor(building);
            LoadInfoFor(CurTarget);
        }

        protected virtual void LoadInfoFor(TTarget building) { }
    }
    
    public class Home_UI_Info : MonoBehaviourExt {
        public Object CurTargetWeak { get; private set; }

        public virtual void LoadInfoFor(Object building) {
            CurTargetWeak = building;
        }

        public virtual void UnloadInfo() {
            CurTargetWeak = null;
        }

        public void TrySelectTarget() {
            if (CurTargetWeak is ISelectableTarget selectableTarget) selectableTarget.OnSelect();
        }
        
        public void TryDeselectTarget() {
            if (CurTargetWeak is ISelectableTarget selectableTarget) selectableTarget.OnDeselect();
        }
    }
}