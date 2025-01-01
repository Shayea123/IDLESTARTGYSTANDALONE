using System;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum ShopItemsType { Coins, Inhabitants, Resources, Instruments, Animals, Weapon, Heroes}

    public class PlayerShop : MonoBehaviour
    {
        [Serializable]public struct HeroesForShop
        {
            [HideInInspector] public string name;
            public ScriptableHero hero;
            public ItemRarity rarity;
            public uint amount;

            public ScriptableItem sellfor;
            public uint price;
        }

        [Header("Components")]
        public Player _player;
        public PlayerInhabitants _inhabitants;
        public PlayerItems _items;
        public PlayerHeroes _heroes;

        [Space(20)]

        public ScriptableItemAmountAndPrice[] inhabitants = new ScriptableItemAmountAndPrice[] { };
        public ScriptableItemAmountAndPrice[] resources = new ScriptableItemAmountAndPrice[] { };
        public ScriptableInstrumentAmountAndPrice[] instruments = new ScriptableInstrumentAmountAndPrice[] { };
        public ScriptableAnimalAmountAndPrice[] animals = new ScriptableAnimalAmountAndPrice[] { };
        public ScriptableWeaponAmountAndPrice[] weapon = new ScriptableWeaponAmountAndPrice[] { };
        public HeroesForShop[] heroes = new HeroesForShop[] { };

        public void CmdBuyInhabitants(int index)
        {
            //check coins
            if (_items.GetItemAmount(_player.coinsItem) >= inhabitants[index].price)
            {
                //decrease coins
                _items.DecreaseItemAmount(_player.coinsItem, inhabitants[index].price);

                // as many as possible
                uint limit = (uint)(Mathf.Clamp(inhabitants[index].amount, 0, _inhabitants.InhabitantsMax() - _inhabitants.GetCurrent()));
                _inhabitants.AddCurrent(limit);
                _inhabitants.AddReserve(inhabitants[index].amount - limit);

                UIShop.singleton.MoveIconToPosition(_player, ShopItemsType.Inhabitants, index, limit, ItemRarity.Normal, inhabitants[index].item.image);
            }
            else UIShop.singleton.ShowErrorPanel();
        }

        public void CmdBuyItems(ShopItemsType itemType, int index)
        {
            uint price = 0;
            uint amount = 0;
            ScriptableItem item = null;

            if (itemType == ShopItemsType.Resources)
            {
                if (index <= resources.Length)
                {
                    price = resources[index].price;
                    amount = resources[index].amount;
                    item = resources[index].item;
                }
            }
            else if (itemType == ShopItemsType.Instruments)
            {
                if (index <= instruments.Length)
                {
                    price = instruments[index].price;
                    amount = instruments[index].amount;
                    item = instruments[index].item;
                }
            }
            else if (itemType == ShopItemsType.Animals)
            {
                if (index <= animals.Length)
                {
                    price = animals[index].price;
                    amount = animals[index].amount;
                    item = animals[index].item;
                }
            }
            else if (itemType == ShopItemsType.Weapon)
            {
                if (index <= weapon.Length)
                {
                    price = weapon[index].price;
                    amount = weapon[index].amount;
                    item = weapon[index].item;
                }
            }
            else return;

            //check coins
            if (_items.GetItemAmount(_player.coinsItem) >= price)
            {
                _items.DecreaseItemAmount(_player.coinsItem, price);
                _items.IncreaseItemAmount(item, amount);

                UIShop.singleton.MoveIconToPosition(_player, itemType, index, amount, item.rarity, item.image);
            }
            else UIShop.singleton.ShowErrorPanel();
        }

        public void BuyHeroParts(int index)
        {
            //check coins
            if (_items.GetItemAmount(heroes[index].sellfor) >= heroes[index].price)
            {
                Debug.Log(_items.GetItemAmount(heroes[index].sellfor) + " / " + heroes[index].price);

                //random ?
                if (heroes[index].hero == null)
                {
                    ScriptableHero randomHero = _heroes.RandomHeroPart(heroes[index].rarity);
                    if (randomHero != null)
                    {
                        //decrease coins
                        _items.DecreaseItemAmount(heroes[index].sellfor, heroes[index].price);

                        //add hero
                        _heroes.AddHeroPart(randomHero, Convert.ToUInt16(heroes[index].amount));

                        UIShop.singleton.MoveIconToPosition(_player, ShopItemsType.Heroes, index, heroes[index].amount, randomHero.rarity, randomHero.image);
                    }
                }
                else
                {
                    //decrease coins
                    _items.DecreaseItemAmount(heroes[index].sellfor, heroes[index].price);

                    //add hero
                    _heroes.AddHeroPart(heroes[index].hero, Convert.ToUInt16(heroes[index].amount));

                    UIShop.singleton.MoveIconToPosition(_player, ShopItemsType.Heroes, index, heroes[index].amount, heroes[index].rarity, heroes[index].hero.image);
                }
            }
            else UIShop.singleton.ShowErrorPanel();
        }

        public void CmdBuyStarterKit()
        {
            _player.adsDisabled = true;
        }

        private void OnValidate()
        {
            for (int i = 0; i < inhabitants.Length; i++)
            {
                if (inhabitants[i].name != inhabitants[i].amount.ToString()) inhabitants[i].name = inhabitants[i].amount.ToString();
            }

            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].name != resources[i].item.name) resources[i].name = resources[i].item.name;
            }

            for (int i = 0; i < instruments.Length; i++)
            {
                if (instruments[i].name != instruments[i].item.name) instruments[i].name = instruments[i].item.name;
            }

            for (int i = 0; i < animals.Length; i++)
            {
                if (animals[i].name != animals[i].item.name) animals[i].name = animals[i].item.name;
            }

            for (int i = 0; i < weapon.Length; i++)
            {
                if (weapon[i].name != weapon[i].item.name) weapon[i].name = weapon[i].item.name;
            }

            for (int i = 0; i < heroes.Length; i++)
            {
                heroes[i].name = heroes[i].hero != null ? heroes[i].hero.name : heroes[i].rarity.ToString();
            }
        }
    }
}