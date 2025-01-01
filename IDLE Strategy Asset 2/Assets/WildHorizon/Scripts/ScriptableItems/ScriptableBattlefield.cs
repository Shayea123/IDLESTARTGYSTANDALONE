using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ScriptableItem + Amount is useful for default items (e.g. spawn with 10 potions)
/*[Serializable]public struct ScriptableItemAndPercentRemove
{
    public ScriptableItem item;
    [Range(0, 1)] public float percentMax;
}*/

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/War Hero Item", order = 999)]
    public class ScriptableBattlefield : ScriptableObject
    {
        public Sprite image;
        public FactionType factionType;
        public float waitingTime = 300;
        [Range(0, 1)] public float aggressiveness;
        public ItemRarity rarity;

        public ScriptableHero hero;
        public ScriptableArmyAndAmount[] army = new ScriptableArmyAndAmount[] { };

        [Header("Remove inhabitants")]
        [Range(0, 1)] public float inhabitantsKill;

        //[Header("Remove Items")]
        //public float percentRemoveMin = 0.01f;
        //public float percentRemoveMax = 0.06f;


        [Header("Items For Random Generate")]
        [SerializeField] private ScriptableItemAndMinAndMaxAmount[] tradeItems = new ScriptableItemAndMinAndMaxAmount[] { };
        [SerializeField] private ScriptableItem[] requiredItems = new ScriptableItem[] { };
        public ItemSlot[] RandomGenerationTradeItems()
        {
            List<ItemSlot> list = new List<ItemSlot>();

            for (int i = 0; i < tradeItems.Length; i++)
            {
                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    list.Add(new ItemSlot(new Item(tradeItems[i].item), (uint)UnityEngine.Random.Range(tradeItems[i].amountMin, tradeItems[i].amountMax)));
                }
            }

            /*if (list.Count == 0) return RandomGenerationTradeItems();
            else*/
            return list.ToArray();

        }
        public Item[] RandomGenerationRequiredItems()
        {
            List<Item> list = new List<Item>();

            for (int i = 0; i < requiredItems.Length; i++)
            {
                if (UnityEngine.Random.Range(0, 2) == 1) list.Add(new Item(requiredItems[i]));
            }

            if (list.Count == 0) return RandomGenerationRequiredItems();
            else return list.ToArray();
        }

        // caching /////////////////////////////////////////////////////////////////
        // we can only use Resources.Load in the main thread. we can't use it when
        // declaring static variables. so we have to use it as soon as 'dict' is
        // accessed for the first time from the main thread.
        // -> we save the hash so the dynamic item part doesn't have to contain and
        //    sync the whole name over the network
        static Dictionary<int, ScriptableBattlefield> cache;
        public static Dictionary<int, ScriptableBattlefield> dict
        {
            get
            {
                // not loaded yet?
                if (cache == null)
                {
                    // get all ScriptableItems in resources
                    ScriptableBattlefield[] items = Resources.LoadAll<ScriptableBattlefield>("");

                    // check for duplicates, then add to cache
                    List<string> duplicates = items.ToList().FindDuplicates(item => item.name);
                    if (duplicates.Count == 0)
                    {
                        cache = items.ToDictionary(item => item.name.GetStableHashCode(), item => item);
                    }
                    else
                    {
                        foreach (string duplicate in duplicates)
                            Debug.LogError("Resources folder contains multiple ScriptableBattlefield with the name " + duplicate);
                    }
                }
                return cache;
            }
        }

        private void OnValidate()
        {
            for (int i = 0; i < army.Length; i++)
            {
                if (army[i].amount < 1) army[i].amount = 1;
            }

            for (int i = 0; i < tradeItems.Length; i++)
            {
                if (tradeItems[i].amountMin < 1) tradeItems[i].amountMin = 1;
                if (tradeItems[i].amountMax <= tradeItems[i].amountMin) tradeItems[i].amountMax = tradeItems[i].amountMin + 1;
            }
        }
    }
}


