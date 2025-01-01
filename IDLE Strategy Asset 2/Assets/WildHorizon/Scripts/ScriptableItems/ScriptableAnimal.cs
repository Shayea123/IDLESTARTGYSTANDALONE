using System;
using UnityEngine;

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Animal", order = 999)]
    public class ScriptableAnimal : ScriptableItem
    {

    }

    [Serializable]public struct ScriptableAnimalAmountAndPrice
    {
        [HideInInspector] public string name;
        public ScriptableAnimal item;
        [Min(1)] public uint amount;
        [Min(1)] public uint price;
    }
}


