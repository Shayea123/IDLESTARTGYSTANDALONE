using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]
    public struct Army
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int hash;

        // dynamic stats (cooldowns etc. later)
        public uint amount;

        // constructors
        public Army(ScriptableArmy data, uint amount)
        {
            hash = data.name.GetStableHashCode();
            this.amount = amount;
        }

        // wrappers for easier access
        public ScriptableItem data
        {
            get
            {
                // show a useful error message if the key can't be found
                // note: ScriptableItem.OnValidate 'is in resource folder' check
                //       causes Unity SendMessage warnings and false positives.
                //       this solution is a lot better.
                if (!ScriptableItem.All.ContainsKey(hash))
                    throw new KeyNotFoundException("There is no ScriptableItem with hash=" + hash + ". Make sure that all ScriptableItems are in the Resources folder so they are loaded properly.");
                return ScriptableItem.All[hash];
            }
        }
        public string name => data.name;
        //public Sprite image => data.image;
    }
}


