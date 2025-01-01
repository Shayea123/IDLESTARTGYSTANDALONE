using UnityEngine;

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Boost", order = 999)]
    public class ScriptableBoost : ScriptableItem
    {
        [Header("Boost Stats")]
        public bool increaseIdleAwayTime;
        public float speed;
        public float amount;

        [Header("Boost time")]
        public int addSeconds = 0;

        [Header("Method of obtaining Boost")]
        public bool buy = true;
        public bool watchAd = false;
    }
}


