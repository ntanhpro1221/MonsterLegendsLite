using System;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class FBFModel<TAnim> : FBFModel where TAnim : struct, Enum {
        public void Play(TAnim animId) => Play(animId.ToString());
    }

    // TODO: Play anim by hash name.
    public class FBFModel : MonoBehaviourExt {
        [SerializeField, Required]
        private FBFModelData data;

        protected void Play(string animId) {
            data.Animator.Play(animId);
        }

        public void SetDirection(HorDirection dir) {
            data.FlipLayer.localScale = new Vector3(dir == HorDirection.Left ? -1 : 1, 1, 1);
        }
    }
}