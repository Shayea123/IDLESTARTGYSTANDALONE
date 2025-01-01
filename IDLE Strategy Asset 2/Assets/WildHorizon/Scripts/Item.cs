using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]
    public struct Item
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int hash;

        // constructors
        public Item(ScriptableItem data)
        {
            hash = data.name.GetStableHashCode();
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
        public uint buyPrice => data.buyPrice;
        public uint sellPrice => data.sellPrice;
        public bool sellable => data.sellable;
        public bool tradable => data.tradable;
        public Sprite image => data.image;

        // tooltip
        public string ToolTip()
        {
            // we use a StringBuilder so that addons can modify tooltips later too
            // ('string' itself can't be passed as a mutable object)
            StringBuilder tip = new StringBuilder(data.ToolTip());

            // addon system hooks
            Utils.InvokeMany(typeof(Item), this, "ToolTip_", tip);

            return tip.ToString();
        }
    }
}


