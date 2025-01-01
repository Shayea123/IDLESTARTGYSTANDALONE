using UnityEngine;
using UnityEngine.Events;

namespace IdleStrategyKit
{
    public enum AdsType { none, idleTime, fortune, popUpWindow, giftPanel, buildingAd, construction,  building, researches  }

    public class PlayerAd : MonoBehaviour
    {
        public static UnityEvent AdsShowComplete = new UnityEvent();
        public static AdsType adsType = AdsType.none;

        [Header("Components")]
        public PlayerItems items;

        [Header("Rewards for Pop Up Window")]
        public ScriptableItemAndAmount[] popUpWindowRewards;
        public float timeForPopUpWindow;
        [HideInInspector] public ItemSlot popUpWindowAdReward;

        [Header("Rewards for Gift Panel")]
        public ScriptableItemAndAmount[] rewardsForGiftPanel;
        public float timeForGiftPanel;
        [HideInInspector] public ItemSlot giftPanelSlot;

        [Header("Ad Building")]
        [HideInInspector] public BuildingForAds buildingAd;
        [HideInInspector] public ItemSlot buildingAdSlot;

        [Header("Construction")]
        [HideInInspector] public ScriptableItemAndAmount[] constructionRewards;

        protected void Start()
        {
            Invoke(nameof(GenerateRewardPopUpWindow), timeForPopUpWindow);
            Invoke(nameof(GenerateRewardForGiftPanel), timeForGiftPanel);
            AdsShowComplete.AddListener(ShowComplete);
        }

        private void ShowComplete()
        {
            if (adsType == AdsType.popUpWindow)
            {
                CmdAddRewardForBigPanel();
            }
            else if (adsType == AdsType.giftPanel) CmdAddRewardForGiftPanel();
        }

        //Gift panel
        private void GenerateRewardForGiftPanel()
        {
            int index = Random.Range(0, rewardsForGiftPanel.Length);

            ItemSlot temp = new ItemSlot();
            temp.item = new Item(rewardsForGiftPanel[index].item);
            temp.amount = rewardsForGiftPanel[index].amount;

            giftPanelSlot = temp;
        }
        public void CmdGenerateRewardForGiftPanel()
        {
            giftPanelSlot = new ItemSlot();
            Invoke(nameof(GenerateRewardForGiftPanel), timeForGiftPanel);
        }
        public void CmdAddRewardForGiftPanel()
        {
            if (giftPanelSlot.amount > 0)
            {
                items.IncreaseItemAmount(giftPanelSlot.item.data, giftPanelSlot.amount);
                giftPanelSlot = new ItemSlot();
            }

            Invoke(nameof(GenerateRewardForGiftPanel), timeForGiftPanel);
        }

        // Pop-up window
        private void GenerateRewardPopUpWindow()
        {
            int index = Random.Range(0, popUpWindowRewards.Length);

            ItemSlot reward = new ItemSlot();
            reward.item = new Item(popUpWindowRewards[index].item);
            reward.amount = popUpWindowRewards[index].amount;
            popUpWindowAdReward = reward;
        }
        public void CmdGenerateRewardForBigPanel()
        {
            popUpWindowAdReward = new ItemSlot();
            Invoke(nameof(GenerateRewardPopUpWindow), timeForPopUpWindow);
        }
        public void CmdAddRewardForBigPanel()
        {
            if (popUpWindowAdReward.amount > 0)
            {
                items.IncreaseItemAmount(popUpWindowAdReward.item.data, popUpWindowAdReward.amount);
                popUpWindowAdReward = new ItemSlot();
            }

            Invoke(nameof(GenerateRewardPopUpWindow), timeForPopUpWindow);
        }

        //reward for building constartion
        public void CmdAddRewardForConstruction()
        {
            for (int i = 0; i < constructionRewards.Length; i++)
            {
                items.IncreaseItemAmount(constructionRewards[i].item, constructionRewards[i].amount);
            }
        }

        //ad building
        public void CmdSetRewardForAdBuilding(string itemname, uint amount)
        {
            if (ScriptableItem.All.TryGetValue(itemname.GetStableHashCode(), out ScriptableItem itemData))
            {
                ItemSlot temp = new ItemSlot();
                temp.item = new Item(itemData);
                temp.amount = amount;

                buildingAdSlot = temp;
            }
        }

        public void CmdAddRewardForAdBuilding()
        {
            if (buildingAdSlot.amount > 0)
            {
                items.IncreaseItemAmount(buildingAdSlot.item.data, buildingAdSlot.amount);
                buildingAdSlot = new ItemSlot();
            }
        }
    }
}