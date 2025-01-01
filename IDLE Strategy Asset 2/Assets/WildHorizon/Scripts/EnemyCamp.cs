using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]
    public struct EnemyCamp
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int scriptableBattlefieldHash;

        public int _hash;

        //for location on map
        public sbyte location; //position index on map
        public CampState state;

        public float timeToDisappear;
        public float actionEndTime;

        //attack
        //public List<ScriptableWeaponAndAmount> sendArmy;

        //trade or gift
        public ItemSlot[] tradeItems;  // предметы которые лагерь готов обменивать
                                       //public Item[] requiredItems;  // предметы которые нужны лагерю
                                       //public ItemSlot[] desiredItems;  // предметы которые мы выбрали отправить
        public SelectedGoodsForTrade[] barterItems;

        public uint inhabitants;

        // constructors
        public EnemyCamp(ScriptableBattlefield data, CampState _state, sbyte _location, uint _inhabitants, float _timeToDisappear, float _actionEndTime, ItemSlot[] _tradeItems, SelectedGoodsForTrade[] _barterItems)
        {
            scriptableBattlefieldHash = data.name.GetStableHashCode();
            _hash = (Time.realtimeSinceStartupAsDouble + data.name + _location).GetStableHashCode();
            this.state = _state;
            this.location = _location;
            this.inhabitants = _inhabitants;
            this.timeToDisappear = _timeToDisappear;
            this.actionEndTime = _actionEndTime;

            this.tradeItems = _tradeItems;
            //this.requiredItems = _requiredItems;
            //this.desiredItems = _desiredItems;
            this.barterItems = _barterItems;
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
                if (!ScriptableBattlefield.dict.ContainsKey(scriptableBattlefieldHash))
                    throw new KeyNotFoundException("There is no ScriptableItem with hash=" + scriptableBattlefieldHash + ". Make sure that all ScriptableItems are in the Resources folder so they are loaded properly.");
                return ScriptableBattlefield.dict[scriptableBattlefieldHash];
            }
        }
    }
}


