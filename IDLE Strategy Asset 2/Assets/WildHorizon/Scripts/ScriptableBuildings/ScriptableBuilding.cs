using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]
    public struct ScriptableItemForSendWorkers
    {
        public MiningMethod miningMethod;
        public float waitingTime;
        public ScriptableItemAndAmount[] items;
    }
    [Serializable]
    public struct SendWorkers
    {
        public MiningMethod type;
        public uint inhabitants;
        public float timeLastGetResorce;
    }

    [Serializable]
    public struct IncreasingStorages
    {
        public string name;
        public StorageType storage;
        public ExponentialUint values;
    }

    [Serializable]
    public struct ScriptableItemForResources
    {
        public ScriptableItem item;
        public float yield;
        public uint amount;
    }


    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Building", order = 999)]
    public class ScriptableBuilding : ScriptableObject
    {
        public int maxLevel = 20;
        public BuildingsType buildingType;

        [Header("Sprites")]
        [Tooltip("Depends on max level")] public Sprite[] spriteForPreview;
        [Tooltip("Depends on max level")] public Sprite[] spritesByLevel;
        public Sprite spriteBurn;
        public Sprite spriteConstruction;

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

        [Header("Options")]
        [SerializeField] private bool _openTownInfo;
        public bool openTownInfo => _openTownInfo;

        [SerializeField] private bool _managerAvailable;
        public bool managerAvailable => _managerAvailable;

        [SerializeField] private bool _sellAndBuyResources;
        public bool sellAndBuyResources => _sellAndBuyResources;

        [SerializeField] private bool _managedPrayer;
        public bool managedPrayer => _managedPrayer;

        [SerializeField] private bool _openResearches;
        public bool openResearches => _openResearches;

        [Header("Requirements for build")]
        public LinearUint coins = new LinearUint { baseValue = 5, bonusPerLevel = 3 };
        [Tooltip("Depends on max level")] public float[] buildTime = new float[1] { 10 };
        // required experience grows by 10% each level (like Runescape)
        public LinearUint workersNeed = new LinearUint { baseValue = 5, bonusPerLevel = 3 };
        public ExponentialUintItems[] ingredients;
        public ScriptableResearchAndLevel[] requiredResearches;
        public ScriptableBuildingAndAmountOrLevel[] requiredBuildings;

        [Header("Some Data")]
        [Tooltip("for sorting in Menu BuildSelect")] public byte sortValue = 0;
        public bool temporarilyInactive = false;

        [Header("Bonuses")]
        public IncreasingStorages[] increasingStorages;
        public ExponentialUint inhabitantsIncreaseByTime = new ExponentialUint { baseValue = 0f, multiplier = 0 };
        public ExponentialFloat dollarsIncreaseByTime = new ExponentialFloat { baseValue = 0f, multiplier = 0 };

        [Header("Management - Send workers")]
        public ScriptableItemForSendWorkers[] sendingWorkers;

        [Header("If building is market")]
        public ExponentialFloat salesTax = new ExponentialFloat { baseValue = 0f, multiplier = 0 };

        [Header("Train Army")]
        public ScriptableTrainArmyRecipe[] trainArmy = new ScriptableTrainArmyRecipe[] { };
        public int GetTrainArmyIndex(ScriptableTrainArmyRecipe recipe)
        {
            for (int i = 0; i < trainArmy.Length; i++)
            {
                if (trainArmy[i].Equals(recipe)) return i;
            }
            return -1;
        }


        [Header("If building is Church")]
        public float[] prayBonusBuildingSpeed;
        public float[] prayBonusResearchingSpeed;

        [Header("Sound")]
        public AudioClip sound;

        // caching /////////////////////////////////////////////////////////////////
        // we can only use Resources.Load in the main thread. we can't use it when
        // declaring static variables. so we have to use it as soon as 'dict' is
        // accessed for the first time from the main thread.
        // -> we save the hash so the dynamic item part doesn't have to contain and
        //    sync the whole name over the network
        static Dictionary<int, ScriptableBuilding> cache;
        public static Dictionary<int, ScriptableBuilding> dict
        {
            get
            {
                // not loaded yet?
                if (cache == null)
                {
                    // get all ScriptableItems in resources
                    ScriptableBuilding[] items = Resources.LoadAll<ScriptableBuilding>("");

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

        public bool IsManaged()
        {
            return managerAvailable || sellAndBuyResources || managedPrayer || openResearches;
        }

        private void OnValidate()
        {
            //check sprites array
            if (spritesByLevel.Length < maxLevel)
            {
                Sprite[] temp = new Sprite[maxLevel];
                if (spritesByLevel.Length > 0)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (i < spritesByLevel.Length)
                            temp[i] = spritesByLevel[i];
                        else temp[i] = spritesByLevel[spritesByLevel.Length - 1];
                    }
                }

                spritesByLevel = temp;
            }
            else if (spritesByLevel.Length > maxLevel)
            {
                Sprite[] temp = new Sprite[maxLevel];
                for (int i = 0; i < temp.Length; i++)
                    temp[i] = spritesByLevel[i];

                spritesByLevel = temp;
            }

            //check array time
            if (buildTime.Length < maxLevel)
            {
                float[] temp = new float[maxLevel];
                if (buildTime.Length > 0)
                {
                    for (int i = 0; i < buildTime.Length; i++)
                        temp[i] = buildTime[i];
                }

                buildTime = temp;
            }
            else if (buildTime.Length > maxLevel)
            {
                float[] temp = new float[maxLevel];
                for (int i = 0; i < temp.Length; i++)
                    temp[i] = buildTime[i];

                buildTime = temp;
            }

            //check array spriteForPreview
            if (spriteForPreview.Length < maxLevel)
            {
                Sprite[] temp = new Sprite[maxLevel];
                if (spriteForPreview.Length > 0)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (i < spriteForPreview.Length)
                            temp[i] = spriteForPreview[i];
                        else temp[i] = spriteForPreview[spriteForPreview.Length - 1];
                    }
                }

                spriteForPreview = temp;
            }
            else if (spriteForPreview.Length > maxLevel)
            {
                Sprite[] temp = new Sprite[maxLevel];
                for (int i = 0; i < temp.Length; i++)
                    temp[i] = spriteForPreview[i];

                spriteForPreview = temp;
            }

            //check name for increasingStorages
            for (int i = 0; i < increasingStorages.Length; i++)
            {
                if (increasingStorages[i].name != increasingStorages[i].storage.ToString()) increasingStorages[i].name = increasingStorages[i].storage.ToString();
            }
        }
    }
}

