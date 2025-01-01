using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Train Army Recipe", order = 999)]
    public class ScriptableTrainArmyRecipe : ScriptableObject
    {
        public Sprite image;

        [Header("Ingredients")]
        public ScriptableItemAndAmount[] ingredients;

        [Header("Training time in seconds")]
        public float time = 1;

        [Header("Result Item if succes normal")]
        public ScriptableArmy result;
        public uint resultAmount = 1;

        [Header("Requirements : Build something")]
        public ScriptableBuildingAndAmountOrLevel[] buildings;

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



