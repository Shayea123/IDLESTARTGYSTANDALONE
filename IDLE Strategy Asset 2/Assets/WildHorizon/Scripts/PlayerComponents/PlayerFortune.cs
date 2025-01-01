using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    //[Serializable]
    //public struct Rewards
    //{
    //    public string name;
    //    public bool turnForAd;
    //    public ScriptableItemAndAmount itemForTurn;
    //    public ScriptableItemAndAmount[] items;
    //}

    public class PlayerFortune : MonoBehaviour
    {
        [Header("Components")]
        public Player player;
        public PlayerItems items;
        public PlayerHeroes heroes;

        //public List<Rewards> rewards = new List<Rewards>();
        public ScriptableRussianRouletteRewards rewards;

        public int timeAdRefresh = 30;

        public void CmdSetTimeAds()
        {
            player.timeLastShowAdsForFortune = DateTime.UtcNow;
        }
        public void CmdTryTurnForItem(int type)
        {
            //check required item
            if (items.GetItemAmount(rewards.rewards[type].itemForTurn.item) >= rewards.rewards[type].itemForTurn.amount)
            {
                items.DecreaseItemAmount(rewards.rewards[type].itemForTurn.item, rewards.rewards[type].itemForTurn.amount);
                RpcTurnForItemSucces();
            }
        }
        public void RpcTurnForItemSucces()
        {
            UIFortune.singleton.TurnForItem();
        }

        public void CmdAddReward(int rarity, int index)
        {
            ScriptableItemAndAmount list = rewards.rewards[rarity].items[index];

            if (list.item.Equals(player.inhabitants.scriptableItem))
            {
                //OpenPanelWhatBuying("Inhabitants", (int)list[index].amount, list[index].item.image);
                player.inhabitants.AddCurrent(list.amount);

                //show info panel
                player.notifications.RpcAddNotification("You win Workers: " + list.amount, NotificationsType.none);
            }
            //hero part
            else if (list.item is ScriptableHero part)
            {
                //OpenPanelWhatBuying(part.name, (int)list[index].amount, list[index].item.image);
                heroes.AddHeroPart(part, Convert.ToUInt16(list.amount));
            }
            //resource
            else if (list.item is ScriptableResource resource)
            {
                //OpenPanelWhatBuying(resource.name, (int)list[index].amount, list[index].item.image);

                player.items.IncreaseItemAmount(resource, list.amount);

                //show info panel
                player.notifications.RpcAddNotification("You Win: " + resource.name + " : " + list.amount, NotificationsType.resource);
            }
            //boost
            else if (list.item is ScriptableBoost boost)
            {
                //OpenPanelWhatBuying(boost.name, (int)list[index].amount, list[index].item.image);

                //Global.boosts[boost.name] += (int)list[index].amount;
                player.items.IncreaseItemAmount(boost, list.amount);

                //show info panel
                player.notifications.RpcAddNotification("You Win: " + boost.name + " : " + list.amount, NotificationsType.boosts);
            }
            else
            {
                player.items.IncreaseItemAmount(list.item, list.amount);
            }
        }
    }
}


