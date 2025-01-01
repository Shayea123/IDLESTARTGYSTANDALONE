using System;
using System.Collections.Generic;

namespace IdleStrategyKit
{
    [Serializable]
    public struct Research
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int hash;

        public string name;
        public byte level;

        public bool underStudy;
        public float time;

        public uint workers;
        public uint adsAmount;

        // constructors
        public Research(ScriptableResearch data, byte level, bool underStudy, float time, uint workers, uint adsAmount)
        {
            hash = data.name.GetStableHashCode();

            this.name = data.name;
            this.level = level;

            this.underStudy = underStudy;
            this.time = time;

            this.workers = workers;
            this.adsAmount = adsAmount;
        }

        // wrappers for easier access
        public ScriptableResearch data
        {
            get
            {
                // show a useful error message if the key can't be found
                // note: ScriptableItem.OnValidate 'is in resource folder' check
                //       causes Unity SendMessage warnings and false positives.
                //       this solution is a lot better.
                if (!ScriptableResearch.dict.ContainsKey(hash))
                    throw new KeyNotFoundException("There is no ScriptableItem with hash=" + hash + ". Make sure that all ScriptableItems are in the Resources folder so they are loaded properly.");
                return ScriptableResearch.dict[hash];
            }
        }
    }
}


