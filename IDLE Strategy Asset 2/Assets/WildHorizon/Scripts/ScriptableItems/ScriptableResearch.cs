using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]
    public struct RequirementsForResearch
    {
        public uint coins;
        public uint workersNeed;

        public float researchTime;
        public bool decreaseTimeForAds;

        public ScriptableItemAndAmount[] ingredients;
        public ResearchAndLevel[] requiredResearches;
        public BuildingAndAmountOrLevel[] requiredBuildings;

        [Header("Bonuses")]
        public IncreasingStorages[] increasingStorages;
        [Tooltip("In Percent"), Range(0, 1)] public float buildingSpeedIncrease;
        [Tooltip("In Percent"), Range(0, 1)] public float increaseResearchSpeed;
        [Tooltip("In Percent"), Range(0, 1)] public float increaseCraftSpeed;
        [Range(0, 1)] public float increaseMaxWeight;
    }

    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Research", order = 999)]
    public class ScriptableResearch : ScriptableObject
    {
        public Sprite sprite;
        public RequirementsForResearch[] levels;

        [Header("Description by Language")]
        [SerializeField] private LocalizeText[] descriptionLocalize;
        public string GetDescriptionByLanguage(SystemLanguage lang)
        {
            for (int i = 0; i < descriptionLocalize.Length; i++)
            {
                if (descriptionLocalize[i].language == lang) return descriptionLocalize[i].description;
            }
            return null;
        }


        // caching /////////////////////////////////////////////////////////////////
        // we can only use Resources.Load in the main thread. we can't use it when
        // declaring static variables. so we have to use it as soon as 'dict' is
        // accessed for the first time from the main thread.
        // -> we save the hash so the dynamic item part doesn't have to contain and
        //    sync the whole name over the network
        static Dictionary<int, ScriptableResearch> cache;
        public static Dictionary<int, ScriptableResearch> dict
        {
            get
            {
                // not loaded yet?
                if (cache == null)
                {
                    // get all ScriptableItems in resources
                    ScriptableResearch[] items = Resources.LoadAll<ScriptableResearch>("");

                    // check for duplicates, then add to cache
                    List<string> duplicates = items.ToList().FindDuplicates(item => item.name);
                    if (duplicates.Count == 0)
                    {
                        cache = items.ToDictionary(item => item.name.GetStableHashCode(), item => item);
                    }
                    else
                    {
                        foreach (string duplicate in duplicates)
                            Debug.LogError("Resources folder contains multiple ScriptableItems with the name " + duplicate + ". If you are using subfolders like 'Warrior/Ring' and 'Archer/Ring', then rename them to 'Warrior/(Warrior)Ring' and 'Archer/(Archer)Ring' instead.");
                    }
                }
                return cache;
            }
        }
    }
}