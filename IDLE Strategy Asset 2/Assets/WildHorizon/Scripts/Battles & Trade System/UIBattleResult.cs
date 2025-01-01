using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIBattleResult : MonoBehaviour
    {
        public GameObject panel;
        public Text textName;
        public Image imageRarity;
        public Image imageLogo;

        [Header("Enemy Army")]
        public Transform enemyArmyContent;
        public GameObject enemyArmyPrefab;

        [Header("Our Army")]
        public Transform ourArmyContent;
        public GameObject ourArmyPrefab;

        [Header("Rewards")]
        public Transform rewardContent;
        public GameObject rewardPrefab;

        //public static BattleResults selectedBBattleResults;

        private void Update()
        {
            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(enemyArmyPrefab, 0, enemyArmyContent);

            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(ourArmyPrefab, 0, ourArmyContent);

            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(rewardPrefab, 0, rewardContent);
        }
    }
}


