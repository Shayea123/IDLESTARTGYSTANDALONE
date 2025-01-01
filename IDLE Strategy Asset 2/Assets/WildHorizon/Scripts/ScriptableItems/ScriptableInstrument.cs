using System;
using UnityEngine;

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Instrument", order = 999)]
    public class ScriptableInstrument : ScriptableItem
    {
        [Header("Instrument Stats")]
        public float mineSpeedBonus = 0;
        public float transportationSpeedBonus = 0;
        public float transportationWeightBonus = 0;

        public int durability = 0;

        [Header("Carts")]
        public int carryingWeight = 0;
        public float moveSpeed = 0;
        public int workers = 0;
    }

    [Serializable] public struct ScriptableInstrumentAmountAndPrice
    {
        [HideInInspector] public string name;
        public ScriptableInstrument item;
        [Min(1)] public uint amount;
        [Min(1)] public uint price;
    }
}


