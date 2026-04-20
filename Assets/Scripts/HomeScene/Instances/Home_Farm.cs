using System;
using NGDtuanh.MonsterLegends;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite {
    public class Home_Farm : MonoBehaviourExt {
        [NonSerialized, ShowInInspector, ReadOnly]
        public string insId;

        public void Initialize(string insId) {
            this.insId = insId;
        }
    }
}