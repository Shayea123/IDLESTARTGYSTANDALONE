using System;
using System.Collections.Generic;


namespace IdleStrategyKit
{
    [Serializable]
    public struct Boost
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int hash;

        public uint amount;
        public float time;

        // constructors
        public Boost(ScriptableBoost data, uint amount, float time)
        {
            hash = data.name.GetStableHashCode();

            this.amount = amount;
            this.time = time;
        }

        // wrappers for easier access
        public ScriptableBoost data
        {
            get
            {
                // show a useful error message if the key can't be found
                // note: ScriptableItem.OnValidate 'is in resource folder' check
                //       causes Unity SendMessage warnings and false positives.
                //       this solution is a lot better.
                if (!ScriptableItem.All.ContainsKey(hash))
                    throw new KeyNotFoundException("There is no ScriptableItem with hash=" + hash + ". Make sure that all ScriptableItems are in the Resources folder so they are loaded properly.");
                return (ScriptableBoost)ScriptableItem.All[hash];
            }
        }
    }
}


