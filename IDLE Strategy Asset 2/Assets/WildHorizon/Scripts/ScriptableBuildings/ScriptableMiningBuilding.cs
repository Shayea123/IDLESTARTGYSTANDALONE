using UnityEngine;

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Buildings/Mining", order = 999)]
    public class ScriptableMiningBuilding : ScriptableBuilding
    {
        [Header("Resource mining")]
        public LinearUint workersMax = new LinearUint { baseValue = 5, bonusPerLevel = 3 };
        public ScriptableItemForResources[] minedResources = new ScriptableItemForResources[] { };
        public ExponentialFloat miningRate = new ExponentialFloat { baseValue = 60, multiplier = 0.95f };
        public ScriptableItem[] miningTools;
        public ExponentialLong internalStorage = new ExponentialLong { baseValue = 1.1f, multiplier = 5000 };

        [Header("Resource transportation")]
        public ExponentialFloat deliverySpeed = new ExponentialFloat { baseValue = 60, multiplier = 0.95f };
        public ScriptableItem[] transportationTools;
    }
}


