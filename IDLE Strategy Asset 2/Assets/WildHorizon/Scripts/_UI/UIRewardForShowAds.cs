using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIRewardForShowAds : MonoBehaviour
    {
        [SerializeField] private List<ScriptableItem> rewards;
        public Button button;
        public GameObject panel;
        public Button buttonShowAd;
        public Text textInfo;
        public Image adsImage;
        public Text textAmount;
        private ScriptableItemAndAmount reward;

        public static UIRewardForShowAds singleton;
        public UIRewardForShowAds()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            //if (singleton == null) 
            singleton = this;
        }

        private void Start()
        {
            InvokeRepeating("ShowReward", 200, 300);

            button.onClick.AddListener(() =>
            {
                int index = Random.Range(0, rewards.Count);

                //uint amount = Global.FreeSpaceInStorage(rewards[index]) / 2;
                uint amount = 2;

                amount = amount > 0 ? amount : (uint)Random.Range(1, 5);

                //textInfo.text = "Would you like to watch an ad to get a reward?";
                textAmount.text = amount.ToString();

                adsImage.sprite = rewards[index].image;

                panel.SetActive(true);

                reward = new ScriptableItemAndAmount();
                reward.item = rewards[index];
                reward.amount = amount;
            });

            buttonShowAd.onClick.AddListener(() =>
            {
                //GameADS.singleton.ShowRewardedAd(AdsType.mainWindows);
                button.gameObject.SetActive(false);
                panel.SetActive(false);
            });
        }

        void ShowReward()
        {
            //if (Advertisement.IsReady(GameADS.singleton.rewardedVideo))
            //{
            //    button.gameObject.SetActive(true);
            //}
        }

        public void AddRewardForAds()
        {
            //Global.AddResource(reward);
        }
    }
}


