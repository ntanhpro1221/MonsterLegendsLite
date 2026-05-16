using System;
using System.Collections.Generic;

namespace MonsterLegendsLite.Data {
    [Serializable]
    public class BreedingPlaceDefData : BuildingDefData {
        public List<BreedingRecipeData> Recipes = new();
    }
}