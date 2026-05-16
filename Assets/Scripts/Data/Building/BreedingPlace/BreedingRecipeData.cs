using System;
using System.Collections.Generic;
using System.Linq;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BreedingRecipeData {
        [HideLabel]
        [HorizontalGroup("Main", 1 - 1 / (1 + BreedingOutputData.LABEL_WIDTH))]
        public BreedingInputData Input;

        [LabelText("@" + nameof(Input) + "." + nameof(BreedingInputData.First))]
        [FoldoutGroup("Main/Output/Default Output Configs")]
        public BreedingOutputConfigData DefaultOutputConfig_First;

        [LabelText("@" + nameof(Input) + "." + nameof(BreedingInputData.Second))]
        [FoldoutGroup("Main/Output/Default Output Configs")]
        public BreedingOutputConfigData DefaultOutputConfig_Second;

        [LabelText("Custom Outputs")]
        [VerticalGroup("Main/Output")]
        public List<BreedingOutputData> Outputs = new();

        public BreedingOutputData RandomOutput() {
            var allOutputs = new List<BreedingOutputData>(Outputs) {
                new(Input.First, DefaultOutputConfig_First)
              , new(Input.Second, DefaultOutputConfig_Second)
            };

            var totalWeight = allOutputs.Sum(static output => output.Config.Weight);

            if (totalWeight <= 0f) return UtilFuncs.Ins.RandomInside(allOutputs);

            var randomValue = Random.Range(0, totalWeight);
            var curWeightSum = 0f;

            foreach (var output in allOutputs) {
                curWeightSum += output.Config.Weight;
                if (randomValue > curWeightSum) continue;
                return output;
            }

            return allOutputs[^1];
        }
    }
}