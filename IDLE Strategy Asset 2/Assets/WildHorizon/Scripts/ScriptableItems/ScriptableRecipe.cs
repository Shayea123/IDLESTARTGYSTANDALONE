using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum RecipeType { Processing, Instruments, Weapons }

    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Crafting Recipe", order = 999)]
    public class ScriptableRecipe : ScriptableObject
    {
        public Sprite image;
        public RecipeType recipeType;

        [Header("Ingredients")]
        public uint inhabitants = 1;
        public ScriptableItemAndAmount[] ingredients;

        [Header("Ñrafting time in seconds")]
        public float time = 1;

        [Header("Result Item if succes normal")]
        public ScriptableItem resultItem;
        public uint resultAmount = 1;
        //public int resultAmountMin = 1;
        //public int resultAmountMax = 1;
        //public bool amountRandom;
        [Range(0, 1)] public float probabilitySuccess = 1;

        /*[Header("Result Item if succes improved")]
        public ScriptableItem resultItemImproved;
        public int resultAmountMinImproved = 1;
        public int resultAmountMaxImproved = 1;
        public bool amountRandomImproved;
        [Range(0, 1)] public float probabilitySuccessImproved = 1;*/

        [Header("Result Items if Parse item into resources")]
        public bool disassembleToResources;

        [Header("Other settings")]
        public bool removeAllIngredientsIfFailed;
        public bool randomRemoveIngredientsIfFailed;
        [Range(0, 1)] public float chanceDestructionIngredients = 0;

        [Header("Requirements : Build something")]
        public List<ScriptableBuildingAndAmountOrLevel> buildings;

        [Header("Requirements : Research something")]
        public ScriptableResearchAndLevel[] researches;

        // caching /////////////////////////////////////////////////////////////////
        // we can only use Resources.Load in the main thread. we can't use it when
        // declaring static variables. so we have to use it as soon as 'dict' is
        // accessed for the first time from the main thread.
        // -> we save the hash so the dynamic item part doesn't have to contain and
        //    sync the whole name over the network
        static Dictionary<int, ScriptableRecipe> cache;
        public static Dictionary<int, ScriptableRecipe> dict
        {
            get
            {
                // not loaded yet?
                if (cache == null)
                {
                    // get all ScriptableQuests in resources
                    ScriptableRecipe[] recipes = Resources.LoadAll<ScriptableRecipe>("");

                    // check for duplicates, then add to cache
                    List<string> duplicates = recipes.ToList().FindDuplicates(quest => quest.name);
                    if (duplicates.Count == 0)
                    {
                        cache = recipes.ToDictionary(recipes => recipes.name.GetStableHashCode(), recipes => recipes);
                    }
                    else
                    {
                        foreach (string duplicate in duplicates)
                            Debug.LogError("Resources folder contains multiple ScriptableRecipes with the name " + duplicate + ". If you are using subfolders like 'Warrior/BeginnerQuest' and 'Archer/BeginnerQuest', then rename them to 'Warrior/(Warrior)BeginnerQuest' and 'Archer/(Archer)BeginnerQuest' instead.");
                    }
                }
                return cache;
            }
        }
    }
}


