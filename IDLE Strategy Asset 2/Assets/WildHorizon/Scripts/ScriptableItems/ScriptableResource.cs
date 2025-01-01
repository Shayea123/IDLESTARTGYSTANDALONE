using UnityEngine;

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Resource", order = 999)]
    public class ScriptableResource : ScriptableItem
    {
        [Header("Resource Stats")]
        public MiningMethod miningMethod;
    }
}


