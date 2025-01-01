using System;
using UnityEngine;

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Rewards", order = 999)]
    public class ScriptableRussianRouletteRewards : ScriptableObject
    {
        [Serializable]public struct Rewards
        {
            public string name;
            public bool turnForAd;
            public ScriptableItemAndAmount itemForTurn;
            public ScriptableItemAndAmount[] items;
        }

        public Rewards[] rewards;
    }
}

