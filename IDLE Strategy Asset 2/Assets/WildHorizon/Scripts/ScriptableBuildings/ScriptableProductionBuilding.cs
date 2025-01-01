using UnityEngine;


namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Buildings/Production", order = 999)]
    public class ScriptableProductionBuilding : ScriptableBuilding
    {
        [Header("Crafting items")]
        public RecipeType[] recipeTypes = new RecipeType[] { };
        public ScriptableRecipe[] craftingRecipes = new ScriptableRecipe[] { };
        public ExponentialFloat craftingRate = new ExponentialFloat { baseValue = 60, multiplier = 0.95f };

        public int GetCraftRecipeIndex(ScriptableRecipe recipe)
        {
            for (int i = 0; i < craftingRecipes.Length; i++)
            {
                if (craftingRecipes[i].Equals(recipe)) return i;
            }
            return -1;
        }

        public bool CheckCraftRecipeTypes(RecipeType type)
        {
            for (int i = 0; i < recipeTypes.Length; i++)
            {
                if (recipeTypes[i] == type) return true;
            }

            return false;
        }
    }
}

