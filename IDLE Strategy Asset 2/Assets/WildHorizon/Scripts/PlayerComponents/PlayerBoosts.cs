using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public class PlayerBoosts : MonoBehaviour
    {
        [Header("Components")]
        public Player player;
        public PlayerItems _items;

        //boosts
        public float multiplier = 1;
        public float amountBoost = 1;
        public float speedBoost = 1;

        [HideInInspector] public List<Boost> boosts = new List<Boost>();
        [HideInInspector] public List<Boost> boostsActive = new List<Boost>();

        void Update()
        {
            /*float _speedBoost = 1;
    float _amountBoost = 1;

    player.boosts.speedBoost = _speedBoost;
    player.boosts.amountBoost = _amountBoost;*/

            //check all boosts
            for (int i = 0; i < boostsActive.Count; i++)
            {
                speedBoost = 1;
                amountBoost = 1;
                //add boost bonuses
                speedBoost += boosts[i].data.speed;
                amountBoost += boosts[i].data.amount;

                Boost boost = boostsActive[i];
                boost.time -= Time.deltaTime;
                if (boost.time <= 0) boostsActive.RemoveAt(i);
                else boostsActive[i] = boost;
            }
        }


        public void CreateNewlist()
        {
            boosts.Clear();

            foreach (var item in ScriptableItem.All)
            {
                if (item.Value is ScriptableBoost boost)
                {
                    boosts.Add(new Boost(boost, 0, 0));
                }
            }
        }

        public int FindIndex(ScriptableBoost item)
        {
            for (int i = 0; i < boosts.Count; i++)
            {
                if (boosts[i].data.Equals(item)) return i;
            }

            return -1;
        }
        public int FindIndexInActive(ScriptableBoost item)
        {
            for (int i = 0; i < boostsActive.Count; i++)
            {
                if (boostsActive[i].data.Equals(item)) return i;
            }

            return -1;
        }
        public uint GetAmount(ScriptableBoost item)
        {
            for (int i = 0; i < boosts.Count; i++)
            {
                if (boosts[i].data.Equals(item)) return boosts[i].amount;
            }

            return 0;
        }

        public void CmdBuyBoost(int index)
        {
            //check coins
            if (_items.GetItemAmount(player.coinsItem) >= boosts[index].data.shopPrice)
            {
                Boost boost = boosts[index];
                boost.amount += 1;
                boosts[index] = boost;

                //decrease coins
                _items.DecreaseItemAmount(player.coinsItem, boost.data.shopPrice);
            }
        }
        public void CmdUseBoost(int index)
        {
            if (index <= boosts.Count && boosts[index].amount > 0)
            {
                //decrease amount
                Boost boost = boosts[index];
                boost.amount -= 1;
                boosts[index] = boost;


                if (boosts[index].data.increaseIdleAwayTime == false)
                {
                    //add boost to active boosts
                    int activeIndex = FindIndexInActive(boosts[index].data);
                    if (activeIndex != -1)
                    {
                        Boost boostActive = boostsActive[activeIndex];
                        boost.time += boosts[index].data.addSeconds;
                        boostsActive[index] = boostActive;
                    }
                    else
                    {
                        Boost newBoost = new Boost();
                        newBoost.hash = boosts[index].data.name.GetStableHashCode();
                        newBoost.time = boosts[index].data.addSeconds;
                    }
                }
                else
                {
                    float limit = Mathf.Clamp(boosts[index].data.addSeconds, 0, player.idleAwaySecondsMax - player.availableIdleAwaySeconds);
                    player.availableIdleAwaySeconds += limit;
                }
            }
        }
    }
}


