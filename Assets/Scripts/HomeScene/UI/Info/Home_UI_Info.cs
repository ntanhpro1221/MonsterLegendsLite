using NGDtuanh.MonsterLegendsLite;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_Info : MonoBehaviourExt {
        public Object CurWeakTarget { get; private set; }

        public virtual void LoadInfoFor(Object building) {
            CurWeakTarget = building;
        }

        public virtual void UnloadInfo() {
            CurWeakTarget = null;
        }

        public void TrySelectTarget() {
            if (CurWeakTarget is ISelectableTarget selectableTarget) selectableTarget.OnSelect();
        }
        
        public void TryDeselectTarget() {
            if (CurWeakTarget is ISelectableTarget selectableTarget) selectableTarget.OnDeselect();
        }
    }

    public class Home_UI_Info<TTarget> : Home_UI_Info where TTarget : Object {
        public TTarget CurTarget => CurWeakTarget == null ? null : (TTarget)CurWeakTarget;

        public sealed override void LoadInfoFor(Object building) {
            base.LoadInfoFor(building);
            LoadInfoFor(CurTarget);
        }

        protected virtual void LoadInfoFor(TTarget building) { }
    }
}