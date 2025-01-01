using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum BuildingsType { Social, Storages, Mining, Factorys, Military }

    public class PlayerBuildings : MonoBehaviour
    {
        [Header("Components")]
        public Player player;
        public PlayerInhabitants inhabitants;
        public PlayerQuests quests;
        public PlayerItems items;
        public PlayerResearches researches;
        public PlayerHeroes heroes;
        public PlayerArmy army;

        [Header("Ads")]
        [Range(0, 1)] public float advertisingBonus = 0.3f;

        [HideInInspector] public List<Building> buildings = new List<Building>();

        private void Update()
        {
            UpdateServer();
        }
        private void UpdateServer()
        {
            //for increase inhabitants
            uint inhabitantsIncreaseAmount = 0;
            float inhabitantsIncreaseBonus = 0;

            //increase mining resource and dollars / move resources to storage / processing / craft
            for (int i = 0; i < buildings.Count; i++)
            {
                Building building = buildings[i];

                if (building.level > 0)
                {
                    //inhabitans
                    inhabitantsIncreaseAmount += building.data.inhabitantsIncreaseByTime.Get(building.level);
                    inhabitantsIncreaseBonus += building.data.inhabitantsIncreaseByTime.Get(building.level) * heroes.BonusForManager(building.data.name, HeroBonusType.inhabitansIncrease);

                    //increase dollars
                    if (building.data.dollarsIncreaseByTime.multiplier > 0)
                    {
                        //check capacity
                        if (items.TotalItemsByStorageType(StorageType.bank) < StorageSizeByType(StorageType.bank))
                        {
                            if (building.lastTimeDollarsIncrease <= 0)
                            {
                                items.IncreaseItemAmount(player.dollarsItem, (uint)(building.data.dollarsIncreaseByTime.Get(building.level) * (int)(inhabitants.GetCurrent() / 10)));

                                //set new time
                                float bonusTimeForManager = inhabitants.GetArrivalTime() * player.heroes.BonusForManager(building.data.name, HeroBonusType.dollarsIncrease);
                                building.lastTimeDollarsIncrease = inhabitants.GetArrivalTime() - 0;
                            }
                            else building.lastTimeDollarsIncrease -= Time.deltaTime;
                        }
                    }

                    //resource by sending workers
                    for (int x = 0; x < building.sendWorkers.Length; x++)
                    {
                        if (building.sendWorkers[x].timeLastGetResorce < 0)
                        {
                            building.sendWorkers[x].timeLastGetResorce = building.data.sendingWorkers[x].waitingTime;

                            for (int z = 0; z < building.data.sendingWorkers[x].items.Length; z++)
                            {
                                uint amount = (uint)((building.sendWorkers[x].inhabitants * building.data.sendingWorkers[x].items[z].amount) * player.boosts.amountBoost);
                                items.IncreaseItemAmount(building.data.sendingWorkers[x].items[z].item, amount);
                            }
                        }
                        else building.sendWorkers[x].timeLastGetResorce -= Time.deltaTime;
                    }

                    //mined and transport
                    if (building.data is ScriptableMiningBuilding miningBuilding)
                    {
                        //find current resources amount in building
                        uint resourcesMined = 0;
                        for (int c = 0; c < building.resources.Length; c++)
                            resourcesMined += building.resources[c];

                        //increase mining resource 
                        if (building.workersMining > 0)
                        {
                            //check capacity
                            if (resourcesMined < miningBuilding.internalStorage.Get(building.level))
                            {
                                if (building.lastTimeResourceMining <= 0)
                                {
                                    for (int z = 0; z < building.workersMining; z++)
                                    {
                                        float rnd = UnityEngine.Random.Range(0, 100);
                                        for (int x = miningBuilding.minedResources.Length - 1; x >= 0; x--)
                                        {
                                            if (rnd <= miningBuilding.minedResources[x].yield)
                                            {
                                                building.resources[x] += miningBuilding.minedResources[x].amount;
                                                break;
                                            }
                                        }
                                    }

                                    //set new mining time
                                    float bonusTimeForManager = miningBuilding.miningRate.Get(building.level) * player.heroes.BonusForManager(building.data.name, HeroBonusType.miningSpeed);
                                    building.lastTimeResourceMining = (miningBuilding.miningRate.Get(building.level) - bonusTimeForManager);
                                }
                                else building.lastTimeResourceMining -= Time.deltaTime;
                            }
                        }

                        //move resources to storage
                        if (building.workersTransportation > 0 && resourcesMined > 0)
                        {
                            if (building.lastTimeResourceTransportation <= 0)
                            {
                                float bonusWeightForManager = miningBuilding.deliverySpeed.Get(building.level) * player.heroes.BonusForManager(building.data.name, HeroBonusType.transportationWeight);
                                uint transferredCargo = (uint)(building.workersTransportation * inhabitants.GetDefaultWeight());
                                transferredCargo += (uint)(transferredCargo * (researches.IncreasesMaximumWeight() + bonusWeightForManager));

                                for (int r = miningBuilding.minedResources.Length - 1; r >= 0; r--)
                                {
                                    //check free space on storage
                                    if (building.resources[r] > 0 && building.resources[r] >= transferredCargo)
                                    {
                                        building.resources[r] -= transferredCargo;
                                        items.IncreaseItemAmount(miningBuilding.minedResources[r].item, transferredCargo);

                                        break;
                                    }
                                    else
                                    {
                                        transferredCargo -= building.resources[r];
                                        items.IncreaseItemAmount(miningBuilding.minedResources[r].item, building.resources[r]);
                                        building.resources[r] = 0;
                                    }
                                }

                                //set new transport time
                                float bonusTimeForManager = miningBuilding.deliverySpeed.Get(building.level) * player.heroes.BonusForManager(building.data.name, HeroBonusType.transportationSpeed);
                                building.lastTimeResourceTransportation += (miningBuilding.deliverySpeed.Get(building.level) - bonusTimeForManager);
                            }
                            else building.lastTimeResourceTransportation -= Time.deltaTime;
                        }
                    }

                    //craft
                    if (building.data is ScriptableProductionBuilding productionBuilding)
                    {
                        for (int x = 0; x < building.craftEnable.Length; x++)
                        {
                            if (building.craftEnable[x])
                            {
                                //enough resources?
                                if (items.EnoughItems(productionBuilding.craftingRecipes[x].ingredients))
                                {
                                    if (building.lastTimeCraft[x] <= 0)
                                    {
                                        float bonusTimeForManager = productionBuilding.craftingRecipes[x].time * heroes.BonusForManager(building.data.name, HeroBonusType.craftingSpeed);
                                        building.lastTimeCraft[x] += productionBuilding.craftingRecipes[x].time - bonusTimeForManager;

                                        //free space
                                        if (items.FreeSpaceInStorage(productionBuilding.craftingRecipes[x].resultItem.storageType) >= productionBuilding.craftingRecipes[x].resultAmount)
                                        {
                                            //decrease ingredients
                                            for (int y = 0; y < productionBuilding.craftingRecipes[x].ingredients.Length; y++)
                                            {
                                                if (productionBuilding.craftingRecipes[x].ingredients[y].item.Equals(player.inhabitants.scriptableItem) == false)
                                                    items.DecreaseItemAmount(productionBuilding.craftingRecipes[x].ingredients[y].item, productionBuilding.craftingRecipes[x].ingredients[y].amount);
                                            }

                                            items.IncreaseItemAmount(productionBuilding.craftingRecipes[x].resultItem, (uint)productionBuilding.craftingRecipes[x].resultAmount);
                                        }
                                        else
                                        {
                                            //stop craft
                                            building.craftEnable = new bool[productionBuilding.craftingRecipes.Length];
                                            for (int y = 0; y < building.craftEnable.Length; y++)
                                            {
                                                building.craftEnable[y] = buildings[i].craftEnable[y];
                                            }
                                            building.craftEnable[x] = false;

                                            player.notifications.RpcAddNotification(Localization.Translate("StorageIsFull"), NotificationsType.storageIsFull);
                                        }
                                    }
                                    else
                                    {
                                        //change craft time
                                        building.lastTimeCraft = new float[productionBuilding.craftingRecipes.Length];
                                        for (int y = 0; y < building.lastTimeCraft.Length; y++)
                                        {
                                            building.lastTimeCraft[y] = buildings[i].lastTimeCraft[y];
                                        }
                                        building.lastTimeCraft[x] -= Time.deltaTime;

                                        buildings[i] = building;
                                    }
                                }
                                else
                                {
                                    //stop craft
                                    building.craftEnable = new bool[productionBuilding.craftingRecipes.Length];
                                    for (int y = 0; y < building.craftEnable.Length; y++)
                                    {
                                        building.craftEnable[y] = buildings[i].craftEnable[y];
                                    }
                                    building.craftEnable[x] = false;

                                    player.notifications.RpcAddNotification(Localization.Translate("NotEnoughResourcesForCraft"), NotificationsType.none);
                                }
                            }
                        }
                    }

                    //train army
                    if (building.trainArmyEnable != null && building.trainArmyEnable.Length > 0)
                    {
                        for (int x = 0; x < building.trainArmyEnable.Length; x++)
                        {
                            if (building.trainArmyEnable[x])
                            {
                                //enough resources?
                                if (items.EnoughItems(building.data.trainArmy[x].ingredients))
                                {
                                    if (building.lastTimeTrainArmy[x] <= 0)
                                    {
                                        float bonusTimeForManager = building.data.trainArmy[x].time * player.heroes.BonusForManager(building.data.name, HeroBonusType.craftingSpeed);
                                        building.lastTimeTrainArmy[x] += building.data.trainArmy[x].time - bonusTimeForManager;

                                        //decrease ingredients
                                        for (int y = 0; y < building.data.trainArmy[x].ingredients.Length; y++)
                                        {
                                            items.DecreaseItemAmount(building.data.trainArmy[x].ingredients[y].item, building.data.trainArmy[x].ingredients[y].amount);
                                        }


                                        //add army
                                        army.AddArmy(building.data.trainArmy[x].result, building.data.trainArmy[x].resultAmount);
                                    }
                                    else building.lastTimeTrainArmy[x] -= Time.deltaTime;
                                }
                                else
                                {
                                    //show error panel
                                    //panelError.SetActive(true);
                                    //textError.text = "Not enough resources";

                                    //stop craft
                                    building.trainArmyEnable[x] = false;
                                }
                            }
                        }
                    }

                    buildings[i] = building;
                }

                //update build time
                if (building.underConstruction)
                {
                    if (building.time > 0)
                    {
                        float bonus = player.prayType == PreyType.buildSpeed ? PrayBonusBuildingSpeed() : 0;
                        building.time -= (Time.deltaTime + (Time.deltaTime * bonus));
                    }
                    else
                    {
                        building.level += 1;
                        building.time = 0;
                        building.underConstruction = false;

                        if (building.data is ScriptableMiningBuilding miningBuilding)
                        {
                            if (building.level == 1) building.workersMining = building.workersBuild;

                            if (building.workersMining > 0)
                            {
                                float bonusTimeForManager = miningBuilding.miningRate.Get(building.level) * player.heroes.BonusForManager(building.data.name, HeroBonusType.miningSpeed);
                                building.lastTimeResourceMining = (miningBuilding.miningRate.Get(building.level) - bonusTimeForManager);
                            }
                        }

                        building.workersBuild = 0;

                        //check all quests
                        quests.CheckAllQuestEvent();

                        //try move inhabitans from stock
                        inhabitants.MoveInhabitantsFromStock();

                        //show info panel
                        player.notifications.RpcAddNotification(Localization.Translate("ConstructionCompleted") + ": " + Localization.Translate(building.data.name), NotificationsType.building);
                    }

                    buildings[i] = building;
                }
            }

            //check capacity
            if (inhabitants.GetCurrent() < inhabitants.InhabitantsMax())
            {
                if (inhabitants.lastTimeInhabitansIncrease <= 0)
                {
                    inhabitants.AddCurrent(inhabitantsIncreaseAmount);
                    inhabitants.lastTimeInhabitansIncrease = inhabitants.GetArrivalTime() - inhabitantsIncreaseBonus;
                }
                else inhabitants.lastTimeInhabitansIncrease -= Time.deltaTime;
            }
        }

        public void CreateNewlist()
        {
            buildings.Clear();

            foreach (var building in ScriptableBuilding.dict)
            {
                ScriptableBuilding data = building.Value;

                SendWorkers[] sendWorkers = new SendWorkers[] { };
                if (data.sendingWorkers != null && data.sendingWorkers.Length > 0)
                {
                    sendWorkers = new SendWorkers[data.sendingWorkers.Length];
                    for (int i = 0; i < sendWorkers.Length; i++)
                    {
                        sendWorkers[i].type = data.sendingWorkers[i].miningMethod;
                        sendWorkers[i].timeLastGetResorce = 0;
                    }
                }

                //mining
                uint[]miningResources = new uint[0];
                if(data is ScriptableMiningBuilding miningBuilding)
                {
                    miningResources = new uint[miningBuilding.minedResources.Length];
                }

                //craft
                bool[] craftEnable = new bool[0];
                float[] craftTime = new float[0];
                if (data is ScriptableProductionBuilding productionBuilding)
                {
                    craftEnable = new bool[productionBuilding.craftingRecipes.Length];
                    craftTime = new float[productionBuilding.craftingRecipes.Length];
                }

                buildings.Add(new Building(data, 0, false, 0,
                    //workers
                    0, 0, 0, 0,
                    sendWorkers,
                    miningResources, 0, 0, 0, 0,
                    craftEnable, craftTime,
                    new bool[data.trainArmy.Length], new float[data.trainArmy.Length],
                    0));
            }
        }
        public int FindIndex(ScriptableBuilding item)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].data.Equals(item)) return i;
            }

            return -1;
        }

        public uint StorageSizeByType(StorageType storageType)
        {
            uint value = 0;
            foreach (Building v in buildings)
            {
                if (v.level > 0 && v.data.increasingStorages.Length > 0)
                {
                    for (int i = 0; i < v.data.increasingStorages.Length; i++)
                    {
                        if (v.data.increasingStorages[i].storage == storageType) value += v.data.increasingStorages[i].values.Get(v.level);
                    }
                }
            }

            return value;
        }
        //public uint GetIncreasingStorageSizeByType(IncreasingStorages[] storages, StorageType storageType, int level)
        //{
        //    uint value = 0;
        //    for (int i = 0; i < storages.Length; i++)
        //    {
        //        if (storages[i].storage == storageType) value += storages[i].values.Get(level);
        //    }

        //    return value;
        //}

        public float PrayBonusBuildingSpeed()
        {
            float bonus = 0;
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].level > 0) bonus += buildings[i].data.prayBonusBuildingSpeed[buildings[i].level];
            }

            return bonus;
        }

        public List<ScriptableBuildingAndAmountOrLevel> requiredBuildings(ScriptableBuildingAndAmountOrLevel[] requiredBuildings, int level)
        {
            List<ScriptableBuildingAndAmountOrLevel> list = new List<ScriptableBuildingAndAmountOrLevel>();

            for (int i = 0; i < requiredBuildings.Length; i++)
            {
                if (requiredBuildings[i].forLevel == level + 1)
                {
                    list.Add(requiredBuildings[i]);
                }
            }

            return list;
        }

        public void CmdStartBuilding(string name, bool forCoins)
        {
            ScriptableBuilding data;
            if (ScriptableBuilding.dict.TryGetValue(name.GetStableHashCode(), out data))
            {
                int index = FindIndex(data);
                Building building = buildings[index];

                building.underConstruction = true;

                if (forCoins)
                {
                    if (building.data.coins.Get(building.level) <= items.GetItemAmount(player.coinsItem))
                    {
                        //decrease coins
                        items.DecreaseItemAmount(player.coinsItem, data.coins.Get(building.level));

                        building.time = 0;
                        building.workersBuild = 0;
                    }
                }
                else
                {
                    //decrease items
                    for (int i = 0; i < data.ingredients.Length; i++)
                    {
                        if (data.ingredients[i].minlevel <= building.level && data.ingredients[i].maxLevel > building.level)
                        {
                            items.DecreaseItemAmount(data.ingredients[i].item, data.ingredients[i].Get(building.level));
                        }
                    }

                    //set build time
                    building.time = ConstructionTime(building);

                    building.workersBuild = data.workersNeed.Get(building.level);
                }

                building.adsAmount = 2;
                buildings[index] = building;
            }
        }
        public void CmdDecreaseBuildingConstructionTime(float availableSeconds)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].time > 0)
                {
                    if (buildings[i].time <= availableSeconds)
                    {
                        Building temp = buildings[i];
                        temp.level += 1;
                        temp.time = 0;
                        temp.underConstruction = false;
                        if (temp.data.buildingType == BuildingsType.Mining) temp.workersMining = temp.workersBuild;
                        temp.workersBuild = 0;
                        buildings[i] = temp;

                        //show info panel
                        //UIInfo.singleton.Show("Completed construction of the " + temp.data.name);
                        player.notifications.RpcAddNotification(Localization.Translate("ConstructionCompleted") + ": " + Localization.Translate(temp.data.name), NotificationsType.building);

                        //check all quests
                        quests.CheckAllQuestEvent();
                    }
                    else
                    {
                        Building temp = buildings[i];
                        temp.time -= availableSeconds;
                        buildings[i] = temp;
                    }
                }
            }
        }
        public float ConstructionTime(Building building)
        {
            float bonus = researches.ConstructionSpeedBonus();
            if (player.adsDisabled) bonus += advertisingBonus;

            return building.data.buildTime[building.level] * (1 - bonus);
        }

        public bool CheckRequiredBuildings(ScriptableBuildingAndAmountOrLevel[] requiredBuildings, int currentLevel)
        {
            for (int i = 0; i < requiredBuildings.Length; i++)
            {
                if (requiredBuildings[i].forLevel == currentLevel + 1)
                {
                    int required = FindIndex(requiredBuildings[i].item);
                    if (buildings[required].level < requiredBuildings[i].requiredBuildingLevel) return false;
                }
            }
            return true;
        }
        public bool CheckRequiredBuildings(BuildingAndAmountOrLevel[] requiredBuildings)
        {
            for (int i = 0; i < requiredBuildings.Length; i++)
            {
                int required = FindIndex(requiredBuildings[i].item);
                if (required != -1 && buildings[required].level < requiredBuildings[i].requiredLevel) return false;
            }
            return true;
        }

        public bool CheckBuildForQuest(Building building)
        {
            ScriptableQuest quest = player.quests.FindCurrentStoryQuest();
            if (quest != null)
            {
                for (int i = 0; i < quest.buildings.Length; i++)
                {
                    if (building.data.Equals(quest.buildings[i].item))
                    {
                        if (building.level < quest.buildings[i].requiredBuildingLevel) return true;
                    }
                }
            }
            return false;
        }

        public int GetBuildingLevel(ScriptableBuilding item)
        {
            int index = FindIndex(item);
            if (index != -1)
            {
                return buildings[index].level;
            }

            return -1;
        }

        //ads
        public void ShowAdsForBuilding(string name)
        {
            ScriptableBuilding buildingData;
            if (ScriptableBuilding.dict.TryGetValue(name.GetStableHashCode(), out buildingData))
            {
                int buildingindex = FindIndex(buildingData);
                Building temp = buildings[buildingindex];

                temp.adsAmount += 1;
                buildings[buildingindex] = temp;
            }
        }
        public void DecreaseBuildingTimeByAds(string name)
        {
            int buildingindex = buildings.FindIndex(x => x.data.name == name);

            //add new building or upgarde level
            if (buildings[buildingindex].adsAmount > 0)
            {
                Building temp = buildings[buildingindex];

                float time = (float)(buildings[buildingindex].data.buildTime[buildings[buildingindex].level] * 0.15);
                temp.time = time >= temp.time ? 0 : temp.time - time;
                temp.adsAmount -= 1;
                buildings[buildingindex] = temp;
            }
        }

        //minning
        public void CmdIncreaseWorkerForMining(int buildingIndex, uint amount)
        {
            Building building = buildings[buildingIndex];

            //check inhabitants
            if (building.workersMining < ((ScriptableMiningBuilding)building.data).workersMax.Get(building.level))
            {
                building.workersMining += amount;
                buildings[buildingIndex] = building;
            }
        }
        public void CmdIncreaseWorkerForTransportation(int buildingIndex, uint amount)
        {
            Building building = buildings[buildingIndex];

            //check inhabitants
            if (building.workersTransportation < ((ScriptableMiningBuilding)building.data).workersMax.Get(building.level))
            {
                building.workersTransportation += amount;
                buildings[buildingIndex] = building;
            }
        }
        public void CmdDecreaseWorkerForMining(int buildingIndex, uint amount)
        {
            Building temp = buildings[buildingIndex];
            if (temp.workersMining > 0)
            {
                temp.workersMining -= amount;
                buildings[buildingIndex] = temp;
            }
        }
        public void CmdDecreaseWorkerForTransportation(int buildingIndex, uint amount)
        {
            Building temp = buildings[buildingIndex];
            if (temp.workersTransportation > 0)
            {
                temp.workersTransportation -= amount;
                buildings[buildingIndex] = temp;
            }
        }
        public void CmdSetToolForMining(int buildingIndex, int id)
        {
            Building temp = buildings[buildingIndex];
            temp.mineTool = id;
            buildings[buildingIndex] = temp;
        }
        public void CmdSetToolForTransportation(int buildingIndex, int id)
        {
            Building temp = buildings[buildingIndex];
            temp.transportationTool = id;
            buildings[buildingIndex] = temp;
        }

        //IdleTime
        public void UpdateBuildingsByIdleTime(float seconds)
        {
            IncreaseInhabitantsByIdleTime(seconds);

            for (int i = 0; i < buildings.Count; i++)
            {
                ReducedConstructionTimeByIdleTime(buildings[i], seconds);

                if (buildings[i].level > 0)
                {
                    IncreaseDollarsByIdleTime(buildings[i], seconds);
                    IncreaseItemsBySendingWorkersByIdleTime(buildings[i], seconds);
                    AddMiningItemsByIdleTime(buildings[i], seconds);
                    MovingMiningItemsIntoStorage(buildings[i], seconds);
                    AddCraftItemsByIdleTime(buildings[i], seconds);
                }
            }
        }
        void ReducedConstructionTimeByIdleTime(Building building, float totalSeconds)
        {
            if (building.underConstruction)
            {
                building.time = Mathf.Clamp(building.time - totalSeconds, 0, building.time);
            }
        }
        void IncreaseInhabitantsByIdleTime(double totalSeconds)
        {
            //Debug.Log(inhabitants.GetCurrent() + " / " +  inhabitants.InhabitantsFree());
            if (inhabitants.GetCurrent() < inhabitants.InhabitantsFree())
            {
                uint amount = 0;           // колво прироста
                float bonusForManager = 0; // понижение отката прироста

                //get max value for inhabitants in town   
                for (int i = 0; i < buildings.Count; i++)
                {
                    Building building = buildings[i];

                    if (building.level > 0)
                    {
                        if (building.data.inhabitantsIncreaseByTime.multiplier > 0)
                        {
                            amount += building.data.inhabitantsIncreaseByTime.Get(building.level);
                            bonusForManager += building.data.inhabitantsIncreaseByTime.Get(building.level) * heroes.BonusForManager(building.data.name, HeroBonusType.inhabitansIncrease);
                        }
                    }
                }

                uint increase = amount * (uint)(totalSeconds / (inhabitants.GetArrivalTime() - bonusForManager));

                //находим максимальное значение которое можем добавить
                uint canAdd = inhabitants.CanAddToCurrent();
                Debug.Log(canAdd);

                if (increase > canAdd) AddItemToIdleTimeResourcesList(player.inhabitants.scriptableItem, canAdd);
                else AddItemToIdleTimeResourcesList(player.inhabitants.scriptableItem, increase);
            }
        }
        void IncreaseDollarsByIdleTime(Building building, double totalSeconds)
        {
            //check capacity
            if (building.data.dollarsIncreaseByTime.multiplier > 0 && items.GetItemAmount(player.dollarsItem) < StorageSizeByType(StorageType.bank))
            {
                if (totalSeconds < building.lastTimeDollarsIncrease)
                {
                    building.lastTimeDollarsIncrease -= (float)totalSeconds;
                }
                else if (totalSeconds == building.lastTimeDollarsIncrease)
                {
                    AddItemToIdleTimeResourcesList(player.dollarsItem, inhabitants.GetCurrent() / 10);
                    building.lastTimeDollarsIncrease = 0;
                }
                else
                {
                    if (building.data is ScriptableMiningBuilding miningBuilding)
                    {
                        float bonusTimeForManager = miningBuilding.miningRate.Get(building.level) * heroes.BonusForManager(building.data.name, HeroBonusType.dollarsIncrease);

                        int repetitionCount = (int)(totalSeconds / (inhabitants.GetArrivalTime() - bonusTimeForManager));

                        AddItemToIdleTimeResourcesList(player.dollarsItem, (uint)(repetitionCount * (int)(inhabitants.GetCurrent() / 10)));
                    }
                }
            }
        }
        void IncreaseItemsBySendingWorkersByIdleTime(Building building, double totalSeconds)
        {
            for (int x = 0; x < building.sendWorkers.Length; x++)
            {
                if (building.sendWorkers[x].inhabitants > 0)
                {
                    int repetitionCount = (int)(totalSeconds / building.data.sendingWorkers[x].waitingTime);
                    float timeleft = (float)(totalSeconds / repetitionCount);

                    building.sendWorkers[x].timeLastGetResorce = building.data.sendingWorkers[x].waitingTime - timeleft;

                    for (int z = 0; z < building.data.sendingWorkers[x].items.Length; z++)
                    {
                        uint amount = (uint)((building.sendWorkers[x].inhabitants * building.data.sendingWorkers[x].items[z].amount) * player.boosts.amountBoost);
                        AddItemToIdleTimeResourcesList(building.data.sendingWorkers[x].items[z].item, (uint)(amount * repetitionCount));
                    }
                }
            }
        }
        void AddMiningItemsByIdleTime(Building building, double totalSeconds)
        {
            if (building.data is ScriptableMiningBuilding miningBuilding)
            {
                float bonusTimeForManager = miningBuilding.miningRate.Get(building.level) * heroes.BonusForManager(building.data.name, HeroBonusType.miningSpeed);
                int repetitionCount = (int)(totalSeconds / (miningBuilding.miningRate.Get(building.level) - bonusTimeForManager));

                for (int i = 0; i < repetitionCount; i++)
                {
                    int rnd = UnityEngine.Random.Range(0, 100);
                    for (int x = 0; x < miningBuilding.minedResources.Length; x++)
                    {
                        if (rnd < miningBuilding.minedResources[x].yield)
                        {
                            building.resources[x] += (miningBuilding.minedResources[x].amount * building.workersMining);
                            AddItemToIdleTimeResourcesList(miningBuilding.minedResources[x].item, (miningBuilding.minedResources[x].amount * building.workersMining));
                            break;
                        }
                    }
                }
            }

        }
        void AddItemToIdleTimeResourcesList(ScriptableItem item, uint amount)
        {
            //Debug.Log("Try add item " + item.name + " / " + amount);
            if (amount > 0)
            {
                int index = Player.resourcesIdleTime.FindIndex(x => x.item.Equals(item));

                if (index != -1)
                {
                    ScriptableItemAndAmount temp = Player.resourcesIdleTime[index];
                    temp.amount += amount;
                    Player.resourcesIdleTime[index] = temp;
                }
                else
                {
                    ScriptableItemAndAmount temp = new ScriptableItemAndAmount();
                    temp.item = item;
                    temp.amount = amount;
                    Player.resourcesIdleTime.Add(temp);
                }
            }
        }
        void MovingMiningItemsIntoStorage(Building building, double totalSeconds)
        {
            if (building.data is ScriptableMiningBuilding miningBuilding)
            {
                //find current resources amount in building
                //подсчитываем общее колво всех добытых ресурсов 
                uint resourcesMined = 0;
                for (int c = 0; c < building.resources.Length; c++)
                    resourcesMined += building.resources[c];

                if (building.workersTransportation > 0 && resourcesMined > 0)
                {
                    float bonusWeightForManager = miningBuilding.deliverySpeed.Get(building.level) * player.heroes.BonusForManager(building.data.name, HeroBonusType.transportationWeight);
                    uint transferredCargo = (uint)(building.workersTransportation * inhabitants.GetDefaultWeight());
                    transferredCargo += (uint)(transferredCargo * (researches.IncreasesMaximumWeight() + bonusWeightForManager));

                    for (int r = miningBuilding.minedResources.Length - 1; r >= 0; r--)
                    {
                        //check free space on storage
                        if (building.resources[r] > 0 && building.resources[r] >= transferredCargo)
                        {
                            building.resources[r] -= transferredCargo;
                            items.IncreaseItemAmount(miningBuilding.minedResources[r].item, transferredCargo);

                            break;
                        }
                        else
                        {
                            transferredCargo -= building.resources[r];
                            items.IncreaseItemAmount(miningBuilding.minedResources[r].item, building.resources[r]);
                            building.resources[r] = 0;
                        }
                    }

                    //set new transport time
                    float bonusTimeForManager = miningBuilding.deliverySpeed.Get(building.level) * player.heroes.BonusForManager(building.data.name, HeroBonusType.transportationSpeed);
                    building.lastTimeResourceTransportation += (miningBuilding.deliverySpeed.Get(building.level) - bonusTimeForManager);

                }
            }
        }
        void AddCraftItemsByIdleTime(Building building, double totalSeconds)
        {
            if (building.data is ScriptableProductionBuilding productionBuilding)
            {
                float bonusTimeForManager = productionBuilding.craftingRate.Get(building.level) * heroes.BonusForManager(building.data.name, HeroBonusType.craftingSpeed);

                for (int i = 0; i < building.craftEnable.Length; i++)
                {
                    if (building.craftEnable[i] == true)
                    {
                        float forOne = productionBuilding.craftingRecipes[i].time - bonusTimeForManager;
                        int repetitionCount = (int)(totalSeconds / forOne);

                        //check ingredients
                        items.IncreaseItemAmount(productionBuilding.craftingRecipes[i].resultItem, (uint)(repetitionCount * productionBuilding.craftingRecipes[i].resultAmount));

                        building.lastTimeCraft[i] -= (float)totalSeconds - (forOne * repetitionCount);
                    }
                }
            }
        }
    }
}