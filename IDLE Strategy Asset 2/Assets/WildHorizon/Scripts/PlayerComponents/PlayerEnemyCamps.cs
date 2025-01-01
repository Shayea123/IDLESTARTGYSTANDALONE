using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum FactionType { none, Indians, Cowboys, Bandits, Mexicans }
    public enum CampState { none, War, TradingProgress, Gift }
    public enum battleResult { enemyRetreated, enemyLost, enemyWon, tradeSucces, unsuccessfulTrade }

    [Serializable]
    public struct SelectedGoodsForTrade
    {
        //public int location;
        public ItemSlot slot1;
        public ItemSlot slot2;
    }

    public class PlayerEnemyCamps : MonoBehaviour
    {
        [Header("Components")]
        public Player player;
        public PlayerItems _items;
        public PlayerInhabitants _inhabitants;

        [Header("Relations with other factions")]
        public float friendshipChangeMultiplier = 0.02f;
        public float factionIndians = 0.15f;
        public float factionCowboys = 0.7f;
        public float factionBandits = 0.5f;
        public float factionMexicans = 0.5f;

        public int maximumSettlements = 8;
        public int placesForSettlements = 15;
        [HideInInspector] public int[] temp;

        [HideInInspector] public List<EnemyCamp> enemyCamps = new List<EnemyCamp>();
        [HideInInspector] public List<BattleResults> battleResults = new List<BattleResults>();

        public List<SelectedGoodsForTrade> selectedGoodsForTrade = new List<SelectedGoodsForTrade>();

        [HideInInspector] public EnemyCamp selectedEnemyCamp;

        // networkbehaviour ////////////////////////////////////////////////////////
        private void Start()
        {
            InvokeRepeating("GenerateCamps", 0, 30);
        }

        void Update()
        {
            UpdateServer();
        }
        void UpdateServer()
        {
            for (int i = 0; i < enemyCamps.Count; i++)
            {
                EnemyCamp camp = enemyCamps[i];

                if (camp.state == CampState.none)
                {
                    if (camp.timeToDisappear > 0)
                    {
                        camp.timeToDisappear -= Time.deltaTime;
                        enemyCamps[i] = camp;
                    }
                    else
                    {
                        //float relationshipLevel = player.FindFactionRelationshipLevel(camp.data.factionType);

                        //успешная ли торговля 
                        /*if (UnityEngine.Random.Range(0f, 1f) > relationshipLevel)
                        {
                            //remove inhabitants
                            int inhabitants = (int)(player.inhabitants.InhabitantsFree() * camp.data.inhabitantsKill);
                            player.items.DecreaseItemAmount(player.inhabitantsItem, inhabitants);

                            //remove items
                            for (int x = 0; x < camp.sendItems.Count; x++)
                            {
                                if (Global.items[x].amount > 0 && UnityEngine.Random.Range(0f, 1f) > 0.5f)
                                {
                                    if (Global.items[i].data is ScriptableResource || Global.items[i].data is ScriptableInstrument || Global.items[i].data is ScriptableWeapon)
                                    {
                                        Item item = Global.items[i];
                                        item.amount = item.amount - (int)(item.amount * 0.05f);
                                        Global.items[i] = item;
                                    }
                                }
                            }
                        }
                        else if (UnityEngine.Random.Range(0f, 1f) > relationshipLevel)
                        {

                        }*/


                        enemyCamps.RemoveAt(i);
                    }
                }
                else
                {
                    if (camp.actionEndTime > 0)
                    {
                        camp.actionEndTime -= Time.deltaTime;
                        enemyCamps[i] = camp;
                    }
                    else
                    {
                        float relationshipLevel = player.camps.FindFactionRelationshipLevel(camp.data.factionType);
                        if (camp.state == CampState.TradingProgress)
                        {
                            //trade is succes ?
                            if (UnityEngine.Random.Range(0f, 1f) > relationshipLevel)
                            {
                                player.camps.IncreaseFriendship(camp.data.factionType);

                                //return inhabitants
                                player.inhabitants.AddCurrent(camp.inhabitants);

                                //return carts
                                //player.items.IncreaseItemAmount(player.battles.enemyCamps[i],, );

                                //return items

                                for (int x = 0; x < camp.barterItems.Length; x++)
                                {
                                    _items.IncreaseItemAmount(camp.barterItems[x].slot1.item.data, camp.barterItems[x].slot1.amount);
                                }

                                //show info message
                                player.notifications.RpcAddNotification("Successful trading with " + camp.data.name, NotificationsType.battle);
                                //player.battles.battleResults.Add(new BattleResults(camp.data, DateTime.UtcNow, battleResult.tradeSucces));
                            }
                            else
                            {
                                //the likelihood of an attack on our merchants
                                if (UnityEngine.Random.Range(0f, 1f) < relationshipLevel)
                                {
                                    player.camps.DecreaseFriendship(camp.data.factionType);

                                    //show info message
                                    player.notifications.RpcAddNotification("Trade with " + camp.data.name + " failed : our merchants killed", NotificationsType.battle);
                                    //player.battles.battleResults.Add(new BattleResults(camp.data, DateTime.UtcNow, battleResult.unsuccessfulTrade));
                                }
                                else
                                {
                                    //return inhabitants
                                    player.inhabitants.AddCurrent(camp.inhabitants);

                                    //return items
                                    for (int x = 0; x < camp.barterItems.Length; x++)
                                    {
                                        _items.IncreaseItemAmount(camp.barterItems[x].slot2.item.data, camp.barterItems[x].slot2.amount);
                                    }

                                    //show info message
                                    player.notifications.RpcAddNotification("Trade with " + camp.data.name + " failed", NotificationsType.battle);
                                    //player.notifications.Add(new Notifications(player.battles.enemyCamps[i].data.image, "Trade with " + player.battles.enemyCamps[i].data.name + " failed", NotificationsType.battle));
                                    player.camps.battleResults.Add(new BattleResults(camp.data, DateTime.UtcNow, battleResult.unsuccessfulTrade));
                                }
                            }
                        }
                        else if (camp.state == CampState.War)
                        {
                            player.camps.DecreaseFriendship(camp.data.factionType);

                            //damage attacker
                            int people = 0;
                            //int damage = 0;
                            /*for (int x = 0; x < player.battles.enemyCamps[i].sendArmy.Count; x++)
                            {
                                people += player.battles.enemyCamps[i].sendArmy[x].amount;
                                damage += player.battles.enemyCamps[i].sendArmy[x].item.damage * player.battles.enemyCamps[i].sendArmy[x].amount;
                            }*/

                            //damage camp
                            int campPeople = 0;
                            //int campDamage = 0;
                            for (int x = 0; x < camp.data.army.Length; x++)
                            {
                                //campPeople += camp.data.army[x].amount;
                                //campDamage += camp.data.army[x].item.damage * camp.data.army[x].amount;
                            }

                            //шаг превый - нападающий бьет первым
                            campPeople -= people;
                            if (campPeople > 0)
                            {
                                people -= campPeople;

                                if (people > 0)
                                {

                                }
                                else
                                {
                                    //show info message
                                    //player.notifications.Add(new Notifications(player.battles.enemyCamps[i].data.image, "The attack on the camp " + player.battles.enemyCamps[i].data.name + " failed", NotificationsType.battle));
                                    //player.battles.battleResults.Add(new BattleResults(player.battles.enemyCamps[i].data, DateTime.UtcNow, battleResult.enemyWon));
                                }
                            }
                            else
                            {
                                //show info message
                                //player.notifications.Add(new Notifications(player.battles.enemyCamps[i].data.image, "Camp " + player.battles.enemyCamps[i].data.name + " successfully attacked", NotificationsType.battle));
                                //player.battles.battleResults.Add(new BattleResults(player.battles.enemyCamps[i].data, DateTime.UtcNow, battleResult.enemyLost));
                            }
                        }
                        else if (camp.state == CampState.Gift)
                        {
                            if (UnityEngine.Random.Range(0f, 1f) > relationshipLevel)
                            {
                                if (UnityEngine.Random.Range(0f, 1f) > (relationshipLevel / 2))
                                {
                                    player.camps.IncreaseFriendship(camp.data.factionType);

                                    //show info message

                                }
                                else
                                {
                                    //show info message

                                }
                            }

                        }

                        enemyCamps.RemoveAt(i);
                    }
                }
            }
        }

        public float FindFactionRelationshipLevel(FactionType factionType)
        {
            if (factionType == FactionType.Indians) return factionIndians;
            else if (factionType == FactionType.Cowboys) return factionCowboys;
            else if (factionType == FactionType.Bandits) return factionBandits;
            else if (factionType == FactionType.Mexicans) return factionMexicans;

            return 0.5f;
        }
        void DecreaseFriendship(FactionType factionType)
        {
            if (factionType == FactionType.Indians) factionIndians -= friendshipChangeMultiplier;
            else if (factionType == FactionType.Cowboys) factionCowboys -= friendshipChangeMultiplier;
            else if (factionType == FactionType.Bandits) factionBandits -= friendshipChangeMultiplier;
            else if (factionType == FactionType.Mexicans) factionMexicans -= friendshipChangeMultiplier;
        }
        void IncreaseFriendship(FactionType factionType)
        {
            if (factionType == FactionType.Indians) factionIndians += friendshipChangeMultiplier;
            else if (factionType == FactionType.Cowboys) factionCowboys += friendshipChangeMultiplier;
            else if (factionType == FactionType.Bandits) factionBandits += friendshipChangeMultiplier;
            else if (factionType == FactionType.Mexicans) factionMexicans += friendshipChangeMultiplier;
        }

        void GenerateCamps()
        {
            if (enemyCamps.Count < maximumSettlements)
            {
                for (int i = 0; i < maximumSettlements && enemyCamps.Count < maximumSettlements; i++)
                {
                    if (UnityEngine.Random.Range(0f, 1f) > 0.6f)
                    {
                        ScriptableBattlefield data = ScriptableBattlefield.dict.ElementAt(UnityEngine.Random.Range(0, ScriptableBattlefield.dict.Count)).Value;
                        enemyCamps.Add(new EnemyCamp(data, CampState.none, GetLocation(), 0, data.waitingTime, 0, data.RandomGenerationTradeItems(), new SelectedGoodsForTrade[] { }));
                    }
                }
            }
        }
        sbyte GetLocation()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == -1)
                {
                    list.Add(i);
                }
            }

            if (list.Count > 0) return (sbyte)(list[UnityEngine.Random.Range(0, list.Count)]);

            return -1;
        }

        public int GetCampIndexByHash(int hash)
        {
            for (int i = 0; i < enemyCamps.Count; i++)
            {
                if (enemyCamps[i]._hash == hash) return i;
            }
            return -1;
        }

        //Barter
        public void CmdCreateNullList()
        {
            selectedGoodsForTrade = new List<SelectedGoodsForTrade>() { };
        }
        public void CmdAddToTradeList(int camphash, string selectedItem_1, uint selectedAmount_1, string selectedItem_2, uint selectedAmount_2)
        {
            //проверяем существуют ли предметы в игре с именами которые мы хотим обменять
            if (ScriptableItem.All.TryGetValue(selectedItem_1.GetStableHashCode(), out ScriptableItem item_1) &&
                ScriptableItem.All.TryGetValue(selectedItem_2.GetStableHashCode(), out ScriptableItem item_2))
            {
                int campindex = GetCampIndexByHash(camphash);
                if (campindex != -1)
                {
                    EnemyCamp camp = enemyCamps[campindex];

                    //добавляем если колличество выбранных ресурсов в наличии
                    int slot1ItemIndex = FindItemIndexInCampTradeItems(camp, item_1);
                    if (slot1ItemIndex != -1)
                    {
                        if ((GetBuyItemAmountInSelectedGoodsForTrade(item_1) + selectedAmount_1) <= camp.tradeItems[slot1ItemIndex].amount)
                        {
                            SelectedGoodsForTrade temp = new SelectedGoodsForTrade();

                            temp.slot1 = new ItemSlot(new Item(item_1), selectedAmount_1);
                            temp.slot2 = new ItemSlot(new Item(item_2), selectedAmount_2);

                            selectedGoodsForTrade.Add(temp);
                        }
                    }
                }
            }
        }
        public void CmdRemoveFromTradeList(int selectedSlot)
        {
            selectedGoodsForTrade.RemoveAt(selectedSlot);
        }

        public void CmdStartTrade(int hash, uint workers)
        {
            //check workers
            if (workers <= _inhabitants.InhabitantsFree())
            {
                if (selectedGoodsForTrade.Count > 0)
                {
                    int campindex = GetCampIndexByHash(hash);
                    if (campindex != -1)
                    {
                        EnemyCamp camp = enemyCamps[campindex];
                        camp.state = CampState.TradingProgress;
                        camp.actionEndTime = 120;
                        camp.inhabitants = workers;

                        //remove barter items
                        for (int i = 0; i < selectedGoodsForTrade.Count; i++)
                        {
                            int itemindex = FindItemIndexInCampTradeItems(camp, selectedGoodsForTrade[i].slot1.item.data);
                            if (itemindex != -1)
                            {
                                camp.tradeItems[itemindex].amount -= selectedGoodsForTrade[i].slot1.amount;
                                _items.DecreaseItemAmount(selectedGoodsForTrade[i].slot2.item.data, selectedGoodsForTrade[i].slot2.amount);
                            }
                        }

                        camp.barterItems = selectedGoodsForTrade.ToArray();
                        enemyCamps[campindex] = camp;
                        selectedGoodsForTrade.Clear();
                    }
                }
            }
        }
        public int FindItemIndexInCampTradeItems(EnemyCamp camp, ScriptableItem item)
        {
            for (int i = 0; i < camp.tradeItems.Length; i++)
            {
                if (camp.tradeItems[i].item.data.Equals(item)) return i;
            }

            return -1;
        }


        public uint GetBuyItemAmountInSelectedGoodsForTrade(ScriptableItem item)
        {
            uint value = 0;
            for (int i = 0; i < selectedGoodsForTrade.Count; i++)
            {
                if (selectedGoodsForTrade[i].slot1.item.Equals(item)) value += selectedGoodsForTrade[i].slot1.amount;
            }

            return value;
        }
        public uint GetFreeBuyItemAmountInSelectedGoodsForTrade(EnemyCamp camp, ScriptableItem item)
        {
            uint value = 0;
            for (int i = 0; i < selectedGoodsForTrade.Count; i++)
            {
                if (selectedGoodsForTrade[i].slot1.item.data.Equals(item)) value += selectedGoodsForTrade[i].slot1.amount;
            }

            int index = FindItemIndexInCampTradeItems(camp, item);
            if (index != -1)
                return camp.tradeItems[index].amount - value;
            else return 0;
        }

        public uint GetSellItemAmountInSelectedGoodsForTrade(ScriptableItem item)
        {
            uint value = 0;
            for (int i = 0; i < selectedGoodsForTrade.Count; i++)
            {
                if (selectedGoodsForTrade[i].slot2.item.data.Equals(item)) value += selectedGoodsForTrade[i].slot2.amount;
            }

            return value;
        }

        bool HooWin(EnemyCamp camp)
        {
            //damage attacker
            int people = 0;
            //int damage = 0;
            /*for (int x = 0; x < camp.sendArmy.Count; x++)
            {
                people += camp.sendArmy[x].amount;
                damage += camp.sendArmy[x].item.damage * camp.sendArmy[x].amount;
            }*/

            //damage camp
            int campPeople = 0;
            //int campDamage = 0;
            for (int x = 0; x < camp.data.army.Length; x++)
            {
                //campPeople += camp.data.army[x].amount;
                //campDamage += camp.data.army[x].item.damage * camp.data.army[x].amount;
            }

            //шаг превый - нападающий бьет первым
            campPeople -= people;
            if (campPeople > 0)
            {
                people -= campPeople;

                if (people > 0)
                {

                }
                else
                {
                    //show info message
                    player.notifications.notifications.Add(new Notifications(camp.data.image, "The attack on the camp " + camp.data.name + " failed", NotificationsType.battle));
                    player.camps.battleResults.Add(new BattleResults(camp.data, DateTime.UtcNow, battleResult.enemyWon));
                }
            }
            else
            {
                //show info message
                player.notifications.notifications.Add(new Notifications(camp.data.image, "Camp " + camp.data.name + " successfully attacked", NotificationsType.battle));
                player.camps.battleResults.Add(new BattleResults(camp.data, DateTime.UtcNow, battleResult.enemyLost));
            }

            return false;
        }
        /*Item[] RandomRequiredItems(ScriptableBattlefield data)
    {
        List<Item> list = new List<Item>();

        for (int i = 0; i < requiredItems.Length; i++)
        {
            if (UnityEngine.Random.Range(0, 2) == 1) list.Add(new Item(data.requiredItems[i]));
        }

        return list.ToArray();
    }*/
    }
}


