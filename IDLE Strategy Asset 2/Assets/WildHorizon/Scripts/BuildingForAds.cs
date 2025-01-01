using UnityEngine;

namespace IdleStrategyKit
{
    public class BuildingForAds : Entity
    {
        public SpriteRenderer image;
        public float time = 600;
        private bool ready = false;
        public ScriptableItemAndAmount[] rewards;

        private void Start()
        {
            Invoke(nameof(ShowAnimation), time);
        }

        void ShowAnimation()
        {
            ready = true;
        }

        private void Update()
        {
            image.gameObject.SetActive(ready);
        }

        public void Preparation()
        {
            ready = false;
            Invoke(nameof(ShowAnimation), time);
        }

        public bool isReady()
        {
            return ready;
        }
    }
}


