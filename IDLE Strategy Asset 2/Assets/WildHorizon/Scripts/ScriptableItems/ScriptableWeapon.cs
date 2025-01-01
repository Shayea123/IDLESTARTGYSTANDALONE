using System;
using UnityEngine;

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Weapon", order = 999)]
    public class ScriptableWeapon : ScriptableItem
    {
        public int damage;
        public int ammo;
    }

    [Serializable]public struct ScriptableWeaponAmountAndPrice
    {
        [HideInInspector] public string name;
        public ScriptableWeapon item;
        [Min(1)] public uint amount;
        [Min(1)] public uint price;
    }
}


