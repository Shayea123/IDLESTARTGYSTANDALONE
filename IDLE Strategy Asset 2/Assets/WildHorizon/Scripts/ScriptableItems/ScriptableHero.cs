using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]
    public struct HeroBonus
    {
        public HeroBonusType bonusType;
        public float value;
    }

    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Hero", order = 999)]
    public partial class ScriptableHero : ScriptableItem
    {
        [Header("Hero Stats")]
        public Sprite imagePart;
        public ushort[] needParts;

        public List<HeroBonus> bonuses = new List<HeroBonus>();
    }
}



