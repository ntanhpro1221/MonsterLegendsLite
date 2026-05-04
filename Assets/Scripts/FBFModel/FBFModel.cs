using System;
using System.Linq;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class FBFModel<TAnim> : FBFModel where TAnim : struct, Enum {
        public void Play(TAnim animId) => Play(animId.ToString());
        public float GetRawDuration(TAnim animId) => GetRawDuration(animId.ToString());

        public void Play(TAnim animId, out float duration) {
            Play(animId);
            duration = GetRawDuration(animId);
        }
    }

    // TODO: Play anim by hash name.
    public class FBFModel : MonoBehaviourExt {
        [SerializeField, Required]
        private FBFModelData data;

        private HorDirection curDir;

        public HorDirection CurDir {
            get => curDir;
            set => data.FlipLayer.localScale = new Vector3((curDir = value) is HorDirection.Left ? -1 : 1, 1, 1);
        }

        protected void Play(string animId) {
            data.Animator.Play(animId);
        }

        protected float GetRawDuration(string animId) {
            return data.Animator.runtimeAnimatorController.animationClips.First(i => i.name == animId).length;
        }

        public void LookAt(float x) {
            CurDir = TF.position.x < x ? HorDirection.Right : HorDirection.Left;
        }
    }
}