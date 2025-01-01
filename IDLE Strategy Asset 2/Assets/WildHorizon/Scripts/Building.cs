using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]
    public struct Building
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int hash;

        public byte level;

        public bool underConstruction;
        public float time;

        //workers
        public uint workersBuild;
        public uint workersMining;
        public uint workersTransportation;

        //dollars
        public float lastTimeDollarsIncrease;

        //workers on default resources
        public SendWorkers[] sendWorkers;

        //mine
        public uint[] resources;
        public float lastTimeResourceMining;
        public int mineTool;

        //Transportation
        public float lastTimeResourceTransportation;
        public int transportationTool;

        //craft
        public bool[] craftEnable;
        public float[] lastTimeCraft;

        //train the army
        public bool[] trainArmyEnable;
        public float[] lastTimeTrainArmy;

        public uint adsAmount;

        // constructors
        public Building(ScriptableBuilding data, byte level, bool underConstruction, float time,
            uint workersBuild, uint workersMining, uint workersTransportation, float lastTimeDollarsIncrease,
            SendWorkers[] sendWorkers, uint[] resources,
            float lastTimeResourceMining, float lastTimeResourceTransportation, int mineTool, int transportationTool,
            bool[] craftEnable, float[] lastTimeCraft,
            bool[] trainArmyEnable, float[] lastTimeTrainArmy,
            uint adsAmount)
        {
            hash = data.name.GetStableHashCode();

            this.level = level;

            this.underConstruction = underConstruction;
            this.time = time;

            this.workersBuild = workersBuild;
            this.workersMining = workersMining;
            this.workersTransportation = workersTransportation;

            this.lastTimeDollarsIncrease = lastTimeDollarsIncrease;

            this.sendWorkers = sendWorkers;
            this.resources = resources;
            this.lastTimeResourceMining = lastTimeResourceMining;
            this.lastTimeResourceTransportation = lastTimeResourceTransportation;
            this.mineTool = mineTool;
            this.transportationTool = transportationTool;

            //craft
            this.craftEnable = craftEnable;
            this.lastTimeCraft = lastTimeCraft;

            //trian the army
            this.trainArmyEnable = trainArmyEnable;
            this.lastTimeTrainArmy = lastTimeTrainArmy;

            this.adsAmount = adsAmount;
        }

        // wrappers for easier access
        public ScriptableBuilding data
        {
            get
            {
                // show a useful error message if the key can't be found
                // note: ScriptableItem.OnValidate 'is in resource folder' check
                //       causes Unity SendMessage warnings and false positives.
                //       this solution is a lot better.
                if (!ScriptableBuilding.dict.ContainsKey(hash))
                    throw new KeyNotFoundException("There is no ScriptableItem with hash=" + hash + ". Make sure that all ScriptableItems are in the Resources folder so they are loaded properly.");
                return ScriptableBuilding.dict[hash];
            }
        }

        //inhabitants
        public uint InhabitantsInBuilding()
        {
            uint amount = workersBuild;

            if (level > 0)
            {
                //send workers
                for (int i = 0; i < sendWorkers.Length; i++)
                {
                    amount += sendWorkers[i].inhabitants;
                }

                amount += workersMining + workersTransportation;

                //craft
                for (int i = 0; i < craftEnable.Length; i++)
                {
                    if (craftEnable[i]) amount += ((ScriptableProductionBuilding)data).craftingRecipes[i].ingredients[0].amount;
                }
            }

            return amount;
        }
        public uint InhabitantsInBuildingCraft()
        {
            uint amount = 0;

            for (int i = 0; i < craftEnable.Length; i++)
            {
                if (craftEnable[i]) amount += ((ScriptableProductionBuilding)data).craftingRecipes[i].inhabitants;
            }

            return amount;
        }

        //send workers
        public uint GetSendWorkersAmountByType(MiningMethod type)
        {
            for (int i = 0; i < sendWorkers.Length; i++)
            {
                if (sendWorkers[i].type == type) return sendWorkers[i].inhabitants;
            }

            return 0;
        }
        public void CmdIncreaseSendWorkesAmount(MiningMethod type, uint value)
        {
            for (int i = 0; i < sendWorkers.Length; i++)
            {
                if (sendWorkers[i].type == type)
                {
                    sendWorkers[i].inhabitants += value;
                }
            }
        }
        public void CmdDecreaseSendWorkesAmount(MiningMethod type, uint value)
        {
            for (int i = 0; i < sendWorkers.Length; i++)
            {
                if (sendWorkers[i].type == type)
                {
                    if (sendWorkers[i].inhabitants >= value)
                        sendWorkers[i].inhabitants -= value;
                }
            }
        }
    }
}


