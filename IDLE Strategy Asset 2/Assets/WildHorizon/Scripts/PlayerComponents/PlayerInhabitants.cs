using UnityEngine;

namespace IdleStrategyKit
{
    public class PlayerInhabitants : MonoBehaviour
    {
        [Header("Components")]
        public PlayerBuildings buildings;
        public PlayerResearches researches;
        public PlayerHeroes heroes;
        public PlayerEnemyCamps camps;
        public PlayerArmy army;

        [Header("Settings")]
        public ScriptableItem scriptableItem;
        [SerializeField] private uint defaultMax = 100;
        [SerializeField] private uint addingInhabitantsOnStart = 60;
        [SerializeField] private float arrivalTime = 60f;
        [SerializeField] private uint workersDefaultWeight = 3;

        private uint current = 0;
        private uint inReserve = 0;
        [HideInInspector] public float lastTimeInhabitansIncrease = 0;

        public void StartNewGame()
        {
            SetCurrent = addingInhabitantsOnStart;
            SetReserve = 0;
        }

        public uint GetCurrent()
        {
            return current;
        }
        public uint SetCurrent
        {
            set { current = value; }
        }
        public void AddCurrent(uint value)
        {
            current += value;
        }
        public uint CanAddToCurrent()
        {
            return (buildings.StorageSizeByType(StorageType.inhabitants) + defaultMax) - current - army.InhabitantsInArmy();
        }

        public uint GetReserve()
        {
            return inReserve;
        }
        public uint SetReserve
        {
            set { inReserve = value; }
        }
        public void AddReserve(uint value)
        {
            inReserve += value;
        }

        public float GetArrivalTime()
        {
            return arrivalTime;
        }
        public uint GetDefaultWeight()
        {
            return workersDefaultWeight;
        }

        public uint InhabitantsFree()
        {
            uint inWork = 0;

            //how many are employed in construction, resource extraction and transportation, crafting
            foreach (Building building in buildings.buildings)
            {
                inWork += building.workersBuild;

                if (building.level > 0)
                {
                    for (int i = 0; i < building.sendWorkers.Length; i++)
                    {
                        inWork += building.sendWorkers[i].inhabitants;
                    }

                    inWork += building.workersMining + building.workersTransportation;

                    //craft
                    for (int i = 0; i < building.craftEnable.Length; i++)
                    {
                        if (building.data is ScriptableProductionBuilding productionBuilding && building.craftEnable[i])
                        {
                            inWork += productionBuilding.craftingRecipes[i].inhabitants;
                        }
                    }
                }
            }

            //research
            for (int i = 0; i < researches.researches.Count; i++)
                inWork += researches.researches[i].workers;

            //battles
            for (int i = 0; i < camps.enemyCamps.Count; i++)
            {
                if (camps.enemyCamps[i].state == CampState.War)
                {
                    /*for (int x = 0; x < battles.enemyCamps[i].sendArmy.Count; x++)
                    {
                        amount += (int)battles.enemyCamps[i].sendArmy[x].amount;
                    }*/
                }
                else
                {
                    inWork += camps.enemyCamps[i].inhabitants;
                }
            }

            //in army
            inWork += army.InhabitantsInArmy();

            return current - inWork;
        }
        public uint InhabitantsMax()
        {
            return defaultMax + buildings.StorageSizeByType(StorageType.inhabitants);
        }

        public uint GetSendWorkersAmountByType(MiningMethod type)
        {
            uint inWork = 0;

            for (int i = 0; i < buildings.buildings.Count; i++)
            {
                if (buildings.buildings[i].level > 0)
                {
                    for (int x = 0; x < buildings.buildings[i].sendWorkers.Length; x++)
                    {
                        if (buildings.buildings[i].sendWorkers[x].type == type) inWork += buildings.buildings[i].sendWorkers[x].inhabitants;
                    }
                }
            }

            return inWork;
        }

        public void MoveInhabitantsFromStock()
        {
            if (inReserve > 0)
            {
                uint max = defaultMax + buildings.StorageSizeByType(StorageType.inhabitants);

                if (current < max)
                {
                    if (current + inReserve <= max)
                    {
                        AddCurrent(inReserve);
                        SetReserve = 0;
                    }
                    else
                    {
                        SetCurrent = max;
                        SetReserve = max - current;
                    }
                }
            }
        }

        public string PopulationGrowth()
        {
            uint amount = 0;
            float bonusForManager = 0;

            for (int i = 0; i < buildings.buildings.Count; i++)
            {
                Building building = buildings.buildings[i];

                if (building.level > 0)
                {
                    if (building.data.inhabitantsIncreaseByTime.multiplier > 0)
                    {
                        amount += building.data.inhabitantsIncreaseByTime.Get(building.level);
                        bonusForManager += building.data.inhabitantsIncreaseByTime.Get(building.level) * heroes.BonusForManager(building.data.name, HeroBonusType.inhabitansIncrease);
                    }
                }
            }

            return amount.ToString() + "/" + (arrivalTime - bonusForManager) + "s";
        }

        private void OnValidate()
        {
            uint armyAmount = 0;
            for (int i = 0; i < army.addingArmyOnStart.Length; i++)
            {
                armyAmount += army.addingArmyOnStart[i].amount;
            }

            if (addingInhabitantsOnStart < armyAmount)
            {
                addingInhabitantsOnStart = armyAmount + 1;
            }
        }
    }
}


