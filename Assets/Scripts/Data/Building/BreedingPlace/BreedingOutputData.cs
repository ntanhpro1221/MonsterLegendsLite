using System;
using Sirenix.OdinInspector;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BreedingOutputData {
        public const float LABEL_WIDTH = .4f;

        [HideLabel]
        [HorizontalGroup(LABEL_WIDTH)]
        public MonsterId Monster;

        [HideLabel]
        [HorizontalGroup]
        public BreedingOutputConfigData Config;

        public BreedingOutputData(MonsterId monster, BreedingOutputConfigData config) {
            Monster = monster;
            Config  = config;
        }
    }
}