using System;
using NGDtuanh.Types;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BreedingProcessData {
        public BreedingInputData Input;
        public BreedingOutputData Output;
        public SerTimestamp StartedAt;

        public BreedingProcessData(
            BreedingInputData input
          , BreedingOutputData output) {
            Input     = input;
            Output    = output;
            StartedAt = SerTimestamp.Now();
        }
    }
}