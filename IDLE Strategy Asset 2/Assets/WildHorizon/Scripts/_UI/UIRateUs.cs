using UnityEngine;
using UnityEngine.Events;

namespace IdleStrategyKit
{
    public class UIRateUs : MonoBehaviour
    {
        public static UnityEvent ShowRateUs = new UnityEvent();

        public GameObject panel;

        public float timeBetweenShows = 900;
        private bool alreadyShown = false;

        private void Start()
        {
            Invoke(nameof(Show), timeBetweenShows);
            ShowRateUs.AddListener(Show);
        }

        private void Show()
        {
            alreadyShown = true;
            panel.SetActive(true);
        }

        public void OpenMarket()
        {
            //for MacOs
            //Device.RequestStoreReview();

            //Application.OpenURL("market://details?id=" + Application.identifier);
            Application.OpenURL("market://details?id=com.GameForFun.WildHorizon");
        }
    }
}


