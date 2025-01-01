using System;
using System.Collections.Generic;

namespace IdleStrategyKit
{
    [Serializable]
    public struct BattleResults
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int hash;

        public DateTime time;
        public battleResult result;

        // constructors
        public BattleResults(ScriptableBattlefield data, DateTime time, battleResult result)
        {
            hash = data.name.GetStableHashCode();

            this.time = time;
            this.result = result;
        }

        // wrappers for easier access
        public ScriptableBattlefield data
        {
            get
            {
                // show a useful error message if the key can't be found
                // note: ScriptableItem.OnValidate 'is in resource folder' check
                //       causes Unity SendMessage warnings and false positives.
                //       this solution is a lot better.
                if (!ScriptableBattlefield.dict.ContainsKey(hash))
                    throw new KeyNotFoundException("There is no ScriptableItem with hash=" + hash + ". Make sure that all ScriptableItems are in the Resources folder so they are loaded properly.");
                return ScriptableBattlefield.dict[hash];
            }
        }
    }
}


