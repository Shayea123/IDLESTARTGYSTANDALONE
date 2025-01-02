using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIEnemyCampDescription : MonoBehaviour
    {
        public GameObject panel;
        public Text textName;
        public Image imageRarity;
        public Image imageLogo;

        [Header("Enemy Army")]
        public Transform enemyArmyContent;
        public GameObject enemyArmyPrefab;

        [Header("Rewards")]
        public Transform rewardContent;
        public GameObject rewardPrefab;

        [Header("Trade Items")]
        public Transform tradeItemsContent;
        public GameObject tradeItemsPrefab;

        [Header("Components")]
        public GameObject panelBattleIsOver;
        public UIAudio _audio;

        public static UIEnemyCampDescription singleton;
        public UIEnemyCampDescription()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            if (singleton == null) singleton = this;
        }

        // Update is called once per frame
         void Update()
         {
             if (panel.activeSelf)
             {
                 Player player = Player.localPlayer;
                 if (player != null)
                 {
                     if (UIEnemyCamps.singleton != null)
                     {
                         ScriptableBattlefield selectedEnemyCamp = UIEnemyCamps.selectedEnemyCamp;

                         //name
                         string name =selectedEnemyCamp.hero != null ?selectedEnemyCamp.hero.name :selectedEnemyCamp.name;
                         textName.text = Localization.Translate(name);
                         textName.font = Localization.fontClassic;

                         imageLogo.sprite =selectedEnemyCamp.hero != null ?selectedEnemyCamp.hero.image :selectedEnemyCamp.image;
                         imageRarity.color =selectedEnemyCamp.hero != null ? player.rarity.GetColor(selectedEnemyCamp.hero) : player.rarity.GetColorForEmptySlot();

                         // instantiate/destroy enough slots
                         UIUtils.BalancePrefabs(enemyArmyPrefab,selectedEnemyCamp.army.Length, enemyArmyContent);
                         for (int i = 0; i <selectedEnemyCamp.army.Length; i++)
                         {
                             UIQuestRewardSlot slot = enemyArmyContent.transform.GetChild(i).GetComponent<UIQuestRewardSlot>();

                             slot.image.sprite =selectedEnemyCamp.data.army[i].item.image;

                             //name
                             slot.textName.text = Localization.Translate(selectedEnemyCamp.data.army[i].item.name);
                             slot.textName.font = Localization.fontClassic;

                             //amount
                             slot.textAmount.text =selectedEnemyCamp.data.army[i].amount.ToString();

                             int icopy = i;
                             slot.buttonDescription.onClick.SetListener(() =>
                             {
                                 _audio.PlaySoundButtonClick();
                             });
                         }

                         // instantiate/destroy enough slots
                         UIUtils.BalancePrefabs(rewardPrefab,selectedEnemyCamp.rewards.Length, rewardContent);
                         for (int i = 0; i <selectedEnemyCamp.rewards.Length; i++)
                         {
                             UIQuestRewardSlot slot = rewardContent.transform.GetChild(i).GetComponent<UIQuestRewardSlot>();

                             slot.image.sprite =selectedEnemyCamp.data.rewards[i].item.image;

                             //name
                             slot.textName.text = Localization.Translate(selectedEnemyCamp.data.rewards[i].item.name);
                             slot.textName.font = Localization.fontClassic;

                             //amount
                             slot.textAmount.text =selectedEnemyCamp.data.rewards[i].amount.ToString();

                             int icopy = i;
                             slot.buttonDescription.onClick.SetListener(() =>
                             {
                                 _audio.PlaySoundButtonClick();
                                 UIDescriptionPanel.singleton.ShowScriptableItem(player,selectedEnemyCamp.data.rewards[icopy].item);
                             });
                         }

                         // instantiate/destroy enough slots
                         UIUtils.BalancePrefabs(tradeItemsPrefab,selectedEnemyCamp.tradeItems.Length, tradeItemsContent);
                         for (int i = 0; i <selectedEnemyCamp.tradeItems.Length; i++)
                         {
                             UIQuestRewardSlot slot = tradeItemsContent.transform.GetChild(i).GetComponent<UIQuestRewardSlot>();

                             slot.image.sprite =selectedEnemyCamp.data.tradeItems[i].item.image;

                             //name
                             slot.textName.text = Localization.Translate(selectedEnemyCamp.data.tradeItems[i].item.name);
                             slot.textName.font = Localization.fontClassic;

                             //amount
                             slot.textAmount.text =selectedEnemyCamp.data.tradeItems[i].amount.ToString();

                             int icopy = i;
                             slot.buttonDescription.onClick.SetListener(() =>
                             {
                                 _audio.PlaySoundButtonClick();
                                 UIDescriptionPanel.singleton.ShowScriptableItem(player,selectedEnemyCamp.data.tradeItems[icopy].item);
                             });
                         }

                         //if this battle is over
                         if (player.accessToTheBattles.IsCampPresent(selectedEnemyCamp.hash) == false)
                         {
                             //enemyCamp.camp = null;
                             panelBattleIsOver.SetActive(true);
                             panel.SetActive(false);
                         }
                     }
                     else panel.SetActive(false);
                 }
             }
         }
    }
}


