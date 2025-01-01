// level based values for skill levels, player level based health, etc.
// -> easier than managing huge arrays of level stats
// -> easier than abstract GetManaForLevel functions etc.
//
// note: levels are 1-based. we use level-1 in the calculations so that level 1
//       has 0 bonus
//
using System;
using UnityEngine;

// formula: linear function
//  f(x) = mx + n
//       = bonusPerLevel * level + baseValue
//
// examples:
//   linear growth (bonus=10, base=10) => +10 per level
//     level  1 = 10 * 0 + 10 = 10
//     level  2 = 10 * 1 + 10 = 20
//     level  3 = 10 * 2 + 10 = 30
//     level  4 = 10 * 3 + 10 = 40
//     level  5 = 10 * 4 + 10 = 50
//     level  6 = 10 * 5 + 10 = 60
//     level  7 = 10 * 6 + 10 = 70
//     level  8 = 10 * 7 + 10 = 80
//     level  9 = 10 * 8 + 10 = 90
//     level 10 = 10 * 9 + 10 = 100

namespace IdleStrategyKit
{
    [Serializable]
    public struct LinearInt
    {
        public int baseValue;
        public int bonusPerLevel;
        public int Get(int level) => bonusPerLevel * (level - 1) + baseValue;
    }
    [Serializable]
    public struct LinearFloat
    {
        public float baseValue;
        public float bonusPerLevel;
        public float Get(int level)
        {
            if (level == 0) return baseValue;
            else return bonusPerLevel * (level - 1) + baseValue;
        }
    }
    [Serializable]
    public struct LinearUint
    {
        public uint baseValue;
        public uint bonusPerLevel;
        public uint Get(int level)
        {
            if (level == 0) return baseValue;
            else return (uint)(bonusPerLevel * (level - 1) + baseValue);
        }
    }

    // formula: exponential function
    //  f(x) = m * n^x
    //       = multiplier * baseValue^level
    //
    // examples:
    //   exponential growth (multiplier=100, base=1.1) => +10% per level
    //     level  1 = 100 * 1.1^0 = 100
    //     level  2 = 100 * 1.1^1 = 110
    //     level  3 = 100 * 1.1^2 = 121
    //     level  4 = 100 * 1.1^3 = 133
    //     level  5 = 100 * 1.1^4 = 146
    //     level  6 = 100 * 1.1^5 = 161
    //     level  7 = 100 * 1.1^6 = 177
    //     level  8 = 100 * 1.1^7 = 194
    //     level  9 = 100 * 1.1^8 = 214
    //     level 10 = 100 * 1.1^9 = 235
    // => exponentially growing values are great for max experience in MMOs
    [Serializable]
    public struct ExponentialInt
    {
        public int multiplier;
        public float baseValue;
        public int Get(int level) => Convert.ToInt32(multiplier * Mathf.Pow(baseValue, (level - 1)));
    }
    [Serializable]
    public struct ExponentialLong
    {
        public float baseValue;
        public long multiplier;
        public long Get(int level) => Convert.ToInt64(multiplier * Mathf.Pow(baseValue, (level - 1)));
    }
    [Serializable]
    public struct ExponentialFloat
    {
        public float baseValue;
        public float multiplier;
        public float Get(int level) => baseValue * Mathf.Pow(multiplier, (level - 1));
    }

    [Serializable]
    public struct ExponentialUint
    {
        public float baseValue;
        public uint multiplier;
        public uint Get(int level) => (uint)(Convert.ToInt64(multiplier * Mathf.Pow(baseValue, (level - 1))));
    }

    /*[Serializable]
    public struct ExponentialLongItems
    {
        public ScriptableItem item;
        public long multiplier;
        public float baseValue;

        [Header("the conditions for the requirement of this resource")]
        public bool requiredPermanently;
        public int minlevel;
        public int maxLevel;

        public long Get(int level) => Convert.ToInt64(multiplier * Mathf.Pow(baseValue, level));
    }*/

    [Serializable]
    public struct ExponentialUintItems
    {
        public ScriptableItem item;
        public uint multiplier;
        public float baseValue;

        [Header("the conditions for the requirement of this resource")]
        public bool requiredPermanently;
        public int minlevel;
        public int maxLevel;

        public uint Get(int level) => (uint)(Convert.ToInt64(multiplier * Mathf.Pow(baseValue, level)));
    }
}

