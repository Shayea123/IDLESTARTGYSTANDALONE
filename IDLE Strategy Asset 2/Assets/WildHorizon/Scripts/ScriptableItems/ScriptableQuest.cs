using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum QuestType { story, achievement, guild }

    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Quest", order = 999)]
    public class ScriptableQuest : ScriptableObject
    {
        //public Sprite image;
        public QuestType questType;
        public ScriptableQuest nextPart;

        [Header("Description")]
        public LocalizeText[] descriptionLocalize;

        [Header("Requirements")]
        public ScriptableQuest predecessor; // this quest has to be completed first

        [Header("Requirements : Have Items and Amount in stock")]
        public ScriptableItemAndAmount[] items;

        [Header("Requirements : Spend Items")]
        public ScriptableItemAndAmount[] spendItems;

        [Header("Requirements : Sell Resources")]
        public ScriptableItemAndAmount[] sellItems;

        [Header("Requirements : Buy Resources")]
        public ScriptableItemAndAmount[] buyItems;

        [Header("Requirements : Build something")]
        public ScriptableBuildingAndAmountOrLevel[] buildings;

        [Header("Requirements : Research something")]
        public ScriptableResearch[] researches;

        [Header("Requirements : send workers from camp")]
        public SendWorkers[] sendWorkers = new SendWorkers[] { };

        [Header("Requirements : options")]
        public bool renameTown;

        [Header("Rewards")]
        public List<ScriptableItemAndAmount> rewardItems;

        [Header("Options")]
        public bool showCompletionPercentage = true;

        // caching /////////////////////////////////////////////////////////////////
        // we can only use Resources.Load in the main thread. we can't use it when
        // declaring static variables. so we have to use it as soon as 'dict' is
        // accessed for the first time from the main thread.
        // -> we save the hash so the dynamic item part doesn't have to contain and
        //    sync the whole name over the network
        static Dictionary<int, ScriptableQuest> cache;
        public static Dictionary<int, ScriptableQuest> dict
        {
            get
            {
                // not loaded yet?
                if (cache == null)
                {
                    // get all ScriptableQuests in resources
                    ScriptableQuest[] quests = Resources.LoadAll<ScriptableQuest>("");

                    // check for duplicates, then add to cache
                    List<string> duplicates = quests.ToList().FindDuplicates(quest => quest.name);
                    if (duplicates.Count == 0)
                    {
                        cache = quests.ToDictionary(quest => quest.name.GetStableHashCode(), quest => quest);
                    }
                    else
                    {
                        foreach (string duplicate in duplicates)
                            Debug.LogError("Resources folder contains multiple ScriptableQuests with the name " + duplicate + ". If you are using subfolders like 'Warrior/BeginnerQuest' and 'Archer/BeginnerQuest', then rename them to 'Warrior/(Warrior)BeginnerQuest' and 'Archer/(Archer)BeginnerQuest' instead.");
                    }
                }
                return cache;
            }
        }

        public string GetDescriptionByLanguage(SystemLanguage lang)
        {
            for (int i = 0; i < descriptionLocalize.Length; i++)
            {
                if (descriptionLocalize[i].language == lang) return descriptionLocalize[i].description;
            }
            return null;
        }
    }
}


