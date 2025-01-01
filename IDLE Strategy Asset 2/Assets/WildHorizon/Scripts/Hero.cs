using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]
    public struct Hero
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int hash;

        public ushort parts;
        public byte level;
        public string placeId;

        // constructors
        public Hero(ScriptableHero data, ushort parts, byte level, string placeId)
        {
            hash = data.name.GetStableHashCode();
            this.parts = parts;
            this.level = level;
            this.placeId = placeId;
        }

        // wrappers for easier access
        public ScriptableHero data
        {
            get
            {
                // show a useful error message if the key can't be found
                // note: ScriptableItem.OnValidate 'is in resource folder' check
                //       causes Unity SendMessage warnings and false positives.
                //       this solution is a lot better.
                if (!ScriptableItem.All.ContainsKey(hash))
                    throw new KeyNotFoundException("There is no ScriptableItem with hash=" + hash + ". Make sure that all ScriptableItems are in the Resources folder so they are loaded properly.");
                return (ScriptableHero)ScriptableItem.All[hash];
            }
        }
        public string name => data.name;
        public uint buyPrice => data.buyPrice;
        public uint sellPrice => data.sellPrice;
        public bool sellable => data.sellable;
        public bool tradable => data.tradable;
        public Sprite image => data.image;
    }
}


