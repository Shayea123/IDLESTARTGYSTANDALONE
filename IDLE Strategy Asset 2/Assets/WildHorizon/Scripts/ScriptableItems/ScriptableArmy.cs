using System;
using UnityEngine;

namespace IdleStrategyKit
{

    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Army", order = 999)]
    public class ScriptableArmy : ScriptableItem
    {
        [Header("Arny Stats")]
        public int damage;
        public int defense;
        public float moveSpeed;
    }

    // ScriptableItem + Amount is useful for default items (e.g. spawn with 10 potions)
    [Serializable]
    public struct ScriptableArmyAndAmount
    {
        public ScriptableArmy item;
        public uint amount;
    }
}

