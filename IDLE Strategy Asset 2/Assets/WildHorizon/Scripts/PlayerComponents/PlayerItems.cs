using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]
    public struct StartingStorageCapacity
    {
        public string name;
        public StorageType type;
        public uint capacity;
    }

    public class PlayerItems : MonoBehaviour
    {
        [Header("Components")]
        public Player player;
        public PlayerInhabitants inhabitants;
        public PlayerQuests quests;
        public PlayerBuildings buildings;
        public PlayerHeroes heroes;

        [Header("Storages capacity")]
        [SerializeField] private StartingStorageCapacity[] startingStorageCapacity;
        public uint GetStartingStorageCapacityByType(StorageType type)
        {
            for (int i = 0; i < startingStorageCapacity.Length; i++)
            {
                if (startingStorageCapacity[i].type == type) return startingStorageCapacity[i].capacity;
            }

            return 0;
        }

        [Header("Add On Start")]
        public ScriptableItemAndAmount[] addingResourcesOnStart = new ScriptableItemAndAmount[] { };
        public ScriptableItemAndAmount[] addingInstrumentsOnStart = new ScriptableItemAndAmount[] { };
        public ScriptableItemAndAmount[] addingAnimalsOnStart = new ScriptableItemAndAmount[] { };
        public ScriptableItemAndAmount[] addingWeaponsOnStart = new ScriptableItemAndAmount[] { };

        [Space(10)]
        public float increaseForTime = 60f;

        [Space(10)]
        [SerializeField] private ScriptableItem[] defaultTrackedItems = new ScriptableItem[] { };
        public ScriptableItem[] DefaultTrackedItems { get { return defaultTrackedItems; } }
        [HideInInspector] public List<ScriptableItem> itemsTracked = new List<ScriptableItem>();

        [Space(10)]
        public ScriptableItem[] rewardsForShowAds;
        public ItemSlot rewardForAds;

        public List<ItemSlot> items = new List<ItemSlot>();
        [HideInInspector] public List<ScriptableItemAndAmount> resourcesIdleTime = new List<ScriptableItemAndAmount>();

        public void StartNewGame()
        {
            items.Clear();
            LoadAllItemsInList();
            for (int i = 0; i < addingResourcesOnStart.Length; i++)
            {
                int index = FindItemIndex(addingResourcesOnStart[i].item);
                items[index] = new ItemSlot(new Item(addingResourcesOnStart[i].item), addingResourcesOnStart[i].amount);
            }
            for (int i = 0; i < addingInstrumentsOnStart.Length; i++)
            {
                int index = FindItemIndex(addingInstrumentsOnStart[i].item);
                items[index] = new ItemSlot(new Item(addingInstrumentsOnStart[i].item), addingInstrumentsOnStart[i].amount);
            }
            for (int i = 0; i < addingAnimalsOnStart.Length; i++)
            {
                int index = FindItemIndex(addingAnimalsOnStart[i].item);
                items[index] = new ItemSlot(new Item(addingAnimalsOnStart[i].item), addingAnimalsOnStart[i].amount);
            }
            for (int i = 0; i < addingWeaponsOnStart.Length; i++)
            {
                int index = FindItemIndex(addingWeaponsOnStart[i].item);
                items[index] = new ItemSlot(new Item(addingWeaponsOnStart[i].item), addingWeaponsOnStart[i].amount);
            }

            itemsTracked = new List<ScriptableItem>();
        }

        public void LoadAllItemsInList()
        {
            foreach (var item in ScriptableItem.All)
            {
                items.Add(new ItemSlot(new Item(item.Value), 0, 0));
            }
        }

        //find index
        public int FindItemIndex(ScriptableItem item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item.data.Equals(item)) return i;
            }

            return -1;
        }

        //get amount
        public uint GetItemAmount(ScriptableItem item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item.data.Equals(item)) return items[i].amount;
            }

            return 0;
        }
        public uint GetItemShopAmount(ScriptableItem item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item.data.Equals(item)) return items[i].amountShop;
            }

            return 0;
        }
        public uint TotalItemsByStorageType(StorageType storageType)
        {
            uint value = 0;
            foreach (ItemSlot slot in items)
            {
                if (slot.item.data.storageType == storageType)
                    value += slot.amount;
            }

            return value;
        }
        public long FreeSpaceInStorage(StorageType storageType)
        {
            if (storageType == StorageType.none) return -1;
            else return (GetStartingStorageCapacityByType(storageType) + buildings.StorageSizeByType(storageType)) - TotalItemsByStorageType(storageType);
        }

        //increase-decrease Amount
        public void IncreaseItemAmount(ScriptableItem item, uint amount)
        {
            //find this item in global list all items
            int index = FindItemIndex(item);
            if (index != -1)
            {
                ItemSlot slot = items[index];

                if (item.storageType != StorageType.none)
                {
                    // as many as possible
                    uint limit = (uint)Mathf.Clamp(amount, 0, FreeSpaceInStorage(item.storageType));

                    slot.amount += limit;
                    if (limit < amount) player.notifications.RpcAddNotification(Localization.Translate("StorageIsFull"), NotificationsType.storageIsFull);
                }
                else slot.amount += amount;

                items[index] = slot;
            }

            player.quests.CheckAllQuestEvent();
        }
        public void DecreaseItemAmount(ScriptableItem item, uint amount)
        {
            uint requiredAmount = amount;

            int index = FindItemIndex(item);
            if (index != -1)
            {
                ItemSlot slot = items[index];

                if (GetItemShopAmount(item) > 0)
                {
                    // as many as possible
                    uint limit = (uint)Mathf.Clamp(requiredAmount, 0, items[index].amount);
                    requiredAmount -= limit;

                    slot.amountShop -= limit;
                }

                if (requiredAmount > 0)
                {
                    // as many as possible
                    uint limit = (uint)Mathf.Clamp(requiredAmount, 0, slot.amount);

                    slot.amount -= limit;
                }

                items[index] = slot;
            }
        }

        //trade
        public List<ScriptableItemAndAmount> ItemsForTrade()
        {
            List<ScriptableItemAndAmount> list = new List<ScriptableItemAndAmount>();

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item.tradable)
                {
                    ScriptableItemAndAmount temp = new ScriptableItemAndAmount();
                    temp.item = items[i].item.data;
                    temp.amount = items[i].amount;
                    list.Add(temp);
                }
            }
            return list;
        }
        public List<ScriptableItemAndAmount> ItemsTradeByCamp()
        {
            List<ScriptableItemAndAmount> list = new List<ScriptableItemAndAmount>();

            //первое - нужно найти все телеги
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item.tradable && items[i].item.data is ScriptableInstrument instrument && instrument.carryingWeight > 0)
                {
                    ScriptableItemAndAmount temp = new ScriptableItemAndAmount();
                    temp.item = items[i].item.data;
                    temp.amount = items[i].amount;
                    list.Add(temp);
                }
            }

            //второе - остальные ресурсы
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].amount > 0 && items[i].item.tradable && ((items[i].item.data is ScriptableInstrument instrument && instrument.carryingWeight == 0) || items[i].item.data is ScriptableInstrument == false))
                {
                    ScriptableItemAndAmount temp = new ScriptableItemAndAmount();
                    temp.item = items[i].item.data;
                    temp.amount = items[i].amount;
                    list.Add(temp);
                }
            }
            return list;
        }

        //are there enough items ?
        public bool EnoughItems(ScriptableItemAndAmount[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].item.Equals(inhabitants.scriptableItem))
                {
                    if (items[i].amount > inhabitants.GetCurrent()) return false;
                }
                else if (GetItemAmount(items[i].item) + GetItemShopAmount(items[i].item) < items[i].amount) return false;
            }

            return true;
        }
        public bool EnoughItems(ExponentialUintItems[] ingredients, int level)
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                if (level >= ingredients[i].minlevel && level < ingredients[i].maxLevel)
                {
                    if (GetItemAmount(ingredients[i].item) + GetItemShopAmount(ingredients[i].item) < ingredients[i].Get(level)) return false;
                }
            }
            return true;
        }

        public List<ItemSlot> GetItemsListByType(ScriptableItem type)
        {
            List<ItemSlot> list = new List<ItemSlot>();

            for (int i = 0; i < items.Count; i++)
            {
                if (type.GetType().Equals(items[i].item.data.GetType()))
                {
                    if (items[i].amount > 0) list.Add(items[i]);
                    else
                    {
                        bool added = false;

                        //check all buildings in which produced
                        for (int x = 0; x < buildings.buildings.Count; x++)
                        {
                            //is the building already built?
                            if (buildings.buildings[x].level > 0)
                            {
                                //send workers
                                for (int z = 0; z < buildings.buildings[x].data.sendingWorkers.Length; z++)
                                {
                                    for (int y = 0; y < buildings.buildings[x].data.sendingWorkers[z].items.Length; y++)
                                    {
                                        if (items[i].item.data.Equals(buildings.buildings[x].data.sendingWorkers[z].items[y]) && list.Contains(items[i]) == false)
                                        {
                                            list.Add(items[i]);
                                            added = true;
                                            break;
                                        }
                                    }

                                    if (added) break;
                                }
                                if (added) break;

                                //mined
                                if (buildings.buildings[x].data is ScriptableMiningBuilding miningBuilding)
                                {
                                    for (int z = 0; z < miningBuilding.minedResources.Length; z++)
                                    {
                                        if (items[i].item.data == miningBuilding.minedResources[z].item && list.Contains(items[i]) == false)
                                        {
                                            list.Add(items[i]);
                                            added = true;
                                            break;
                                        }
                                    }
                                }

                                if (added) break;

                                //craft
                                if (buildings.buildings[x].data is ScriptableProductionBuilding productionBuilding)
                                {
                                    for (int z = 0; z < productionBuilding.craftingRecipes.Length; z++)
                                    {
                                        if (items[i].item.data == productionBuilding.craftingRecipes[z].resultItem && list.Contains(items[i]) == false)
                                        {
                                            list.Add(items[i]);
                                            added = true;
                                            break;
                                        }
                                    }
                                }

                                if (added) break;
                                else
                                {
                                    //check shop list
                                    if (GetItemShopAmount(items[i].item.data) > 0)
                                    {
                                        list.Add(items[i]);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }

        public bool IsItemIsTracked(ScriptableItem item)
        {
            for (int i = 0; i < itemsTracked.Count; i++)
            {
                if (itemsTracked[i].Equals(item)) return true;
            }

            return false;
        }

        public string DollarsGrowth()
        {
            float bonusForManager = 0;

            for (int i = 0; i < buildings.buildings.Count; i++)
            {
                Building building = buildings.buildings[i];

                if (building.level > 0)
                {
                    if (building.data.dollarsIncreaseByTime.multiplier > 0)
                    {
                        bonusForManager += building.data.dollarsIncreaseByTime.Get(building.level);
                        bonusForManager += building.data.dollarsIncreaseByTime.Get(building.level) * heroes.BonusForManager(building.data.name, HeroBonusType.dollarsIncrease);
                    }
                }
            }

            return (int)(inhabitants.GetCurrent() / 10) + "/" + (increaseForTime - bonusForManager) + "s";
        }

        public List<ScriptableItemAndAmount> IngredientsList(ExponentialUintItems[] ingredients, int level)
        {
            List<ScriptableItemAndAmount> temp = new List<ScriptableItemAndAmount>();

            for (int i = 0; i < ingredients.Length; i++)
            {
                if (level >= ingredients[i].minlevel && level < ingredients[i].maxLevel)
                {
                    ScriptableItemAndAmount resource = new ScriptableItemAndAmount();
                    resource.item = ingredients[i].item;
                    resource.amount = ingredients[i].Get(level);

                    temp.Add(resource);
                }
            }

            return temp;
        }


        /*public string GetResourceIncreaseValue(string resourceName)
    {
        float temp = 0;
        for (int i = 0; i < buildings.buildings.Count; i++)
        {
            if (buildings.buildings[i].data.minedResources.Count > 0 && buildings.buildings[i].workersMining > 0)
            {
                for (int r = 0; r < buildings.buildings[i].data.minedResources.Count; r++)
                {
                    if (buildings.buildings[i].data.minedResources[r].item.name == resourceName)
                    {
                        float bonusTimeForManager = buildings.buildings[i].data.miningRate.Get(buildings.buildings[i].level) * heroes.BonusForManager(buildings.buildings[i].data, HeroBonusType.miningSpeed);
                        temp += (buildings.buildings[i].data.miningRate.Get(buildings.buildings[i].level) - bonusTimeForManager);
                        break;
                    }
                }
            }
        }

        if (temp > 0) return "1/" + temp + "sec";
        else return "";
    }*/

        public void CmdGenerateRewardForAds()
        {
            int index = UnityEngine.Random.Range(0, rewardsForShowAds.Length);

            uint amount = (uint)FreeSpaceInStorage(rewardsForShowAds[index].storageType) / 3;
            if (rewardsForShowAds[index].Equals(player.coinsItem)) amount = (uint)UnityEngine.Random.Range(1, 5);

            rewardForAds.item = new Item(rewardsForShowAds[index]);
            rewardForAds.amount = amount;
        }
        public void CmdAddRewardForAds()
        {
            if (rewardForAds.amount > 0)
            {
                IncreaseItemAmount(rewardForAds.item.data, rewardForAds.amount);
                rewardForAds.amount = 0;
            }
        }

        private void OnValidate()
        {
            for (int i = 0; i < addingResourcesOnStart.Length; i++)
            {
                addingResourcesOnStart[i].name = addingResourcesOnStart[i].item.name;
            }

            for (int i = 0; i < addingInstrumentsOnStart.Length; i++)
            {
                addingInstrumentsOnStart[i].name = addingInstrumentsOnStart[i].item.name;
            }

            for (int i = 0; i < addingAnimalsOnStart.Length; i++)
            {
                addingAnimalsOnStart[i].name = addingAnimalsOnStart[i].item.name;
            }

            for (int i = 0; i < addingWeaponsOnStart.Length; i++)
            {
                addingWeaponsOnStart[i].name = addingWeaponsOnStart[i].item.name;
            }
        }
    }
}