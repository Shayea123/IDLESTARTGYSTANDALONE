using UnityEngine;
//using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIShop : MonoBehaviour
    {
        [Header("Components")]
        public UIAudio _audio;

        public GameObject panel;
        public Button buttonClose;
        public Canvas canvas;
        public Text textCoinsValue;
        public Text textDollarsValue;

        [Header("Coins")]
        public Transform coinsTransform;

        [Header("Inhabitants")]
        public GameObject inhabitantsPrefab;
        public Transform inhabitantsTransform;

        [Header("Resources")]
        public GameObject resourcesPrefab;
        public Transform resourcesTransform;

        [Header("Instruments")]
        public GameObject instrumentsPrefab;
        public Transform instrumentsTransform;

        [Header("Animals")]
        public GameObject animalsPrefab;
        public Transform animalsTransform;

        [Header("Weapon")]
        public GameObject weaponPrefab;
        public Transform weaponTransform;

        [Header("Heroes")]
        public GameObject heroesPrefab;
        public Transform heroesTransform;

        [Header("Unsuccessful Purchase")]
        public GameObject panelError;
        public Text textError;

        [Header("Positions for Move icons")]
        public GameObject prefabItem;
        public Transform townPosition;
        public Transform inhabitantsPosition;
        public Transform coinsPosition;
        public Transform heroesPosition;

        public static UIShop singleton;
        public UIShop()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            if (singleton == null) singleton = this;
        }

        public void Show()
        {
            panel.SetActive(true);
        }

        public void ShowErrorPanel()
        {
            panelError.SetActive(true);
        }

        private void Start()
        {
            buttonClose.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                panel.SetActive(false);
            });
        }

        private void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    textCoinsValue.text = player.items.GetItemAmount(player.coinsItem).ToString();
                    textDollarsValue.text = player.items.GetItemAmount(player.dollarsItem).ToString();

                    //inhabitants
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(inhabitantsPrefab, player.shop.inhabitants.Length, inhabitantsTransform);
                    for (int i = 0; i < player.shop.inhabitants.Length; i++)
                    {
                        UIShopSlot slot = inhabitantsTransform.transform.GetChild(i).GetComponent<UIShopSlot>();
                        slot.image.sprite = player.shop.inhabitants[i].item.image;
                        slot.amount.text = player.shop.inhabitants[i].amount.ToString();
                        slot.price.text = player.shop.inhabitants[i].price.ToString();

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            if (player.items.GetItemAmount(player.coinsItem) >= player.shop.inhabitants[icopy].price)
                            {
                                player.shop.CmdBuyInhabitants(icopy);
                            }
                            else
                            {
                                panelError.SetActive(true);
                            }
                        });
                    }

                    //resources
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(resourcesPrefab, player.shop.resources.Length, resourcesTransform);
                    for (int i = 0; i < player.shop.resources.Length; i++)
                    {
                        UIShopSlot slot = resourcesTransform.transform.GetChild(i).GetComponent<UIShopSlot>();
                        slot.image.sprite = player.shop.resources[i].item.image;
                        slot.amount.text = player.shop.resources[i].amount.ToString();
                        slot.price.text = player.shop.resources[i].price.ToString();

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            if (player.items.GetItemAmount(player.coinsItem) >= player.shop.resources[icopy].price)
                            {
                                player.shop.CmdBuyItems(ShopItemsType.Resources, icopy);
                            }
                            else
                            {
                                panelError.SetActive(true);
                            }
                        });
                    }

                    //instruments
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(instrumentsPrefab, player.shop.instruments.Length, instrumentsTransform);
                    for (int i = 0; i < player.shop.instruments.Length; i++)
                    {
                        UIShopSlot slot = instrumentsTransform.transform.GetChild(i).GetComponent<UIShopSlot>();
                        slot.image.sprite = player.shop.instruments[i].item.image;
                        slot.amount.text = player.shop.instruments[i].amount.ToString();
                        slot.price.text = player.shop.instruments[i].price.ToString();

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            if (player.items.GetItemAmount(player.coinsItem) >= player.shop.instruments[icopy].price)
                            {
                                player.shop.CmdBuyItems(ShopItemsType.Instruments, icopy);
                            }
                            else
                            {
                                panelError.SetActive(true);
                            }
                        });
                    }

                    //animals
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(animalsPrefab, player.shop.animals.Length, animalsTransform);
                    for (int i = 0; i < player.shop.animals.Length; i++)
                    {
                        UIShopSlot slot = animalsTransform.transform.GetChild(i).GetComponent<UIShopSlot>();
                        slot.image.sprite = player.shop.animals[i].item.image;
                        slot.amount.text = player.shop.animals[i].amount.ToString();
                        slot.price.text = player.shop.animals[i].price.ToString();

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            if (player.items.GetItemAmount(player.coinsItem) >= player.shop.animals[icopy].price)
                            {
                                player.shop.CmdBuyItems(ShopItemsType.Animals, icopy);
                            }
                            else
                            {
                                panelError.SetActive(true);
                            }
                        });
                    }

                    //weapon
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(weaponPrefab, player.shop.weapon.Length, weaponTransform);
                    for (int i = 0; i < player.shop.weapon.Length; i++)
                    {
                        UIShopSlot slot = weaponTransform.transform.GetChild(i).GetComponent<UIShopSlot>();
                        slot.image.sprite = player.shop.weapon[i].item.image;
                        slot.amount.text = player.shop.weapon[i].amount.ToString();
                        slot.price.text = player.shop.weapon[i].price.ToString();

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            if (player.items.GetItemAmount(player.coinsItem) >= player.shop.weapon[icopy].price)
                            {
                                player.shop.CmdBuyItems(ShopItemsType.Weapon, icopy);
                            }
                            else
                            {
                                panelError.SetActive(true);
                            }
                        });
                    }

                    //heroes
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(heroesPrefab, player.shop.heroes.Length, heroesTransform);
                    for (int i = 0; i < player.shop.heroes.Length; i++)
                    {
                        UIShopSlot slot = heroesTransform.transform.GetChild(i).GetComponent<UIShopSlot>();
                        if (player.shop.heroes[i].hero == null)
                        {
                            slot.imageRarity.color = player.rarity.GetColor(player.shop.heroes[i].rarity);
                            //slot.image.sprite = player.shop.heroes[i].sprite;
                            //slot.image.color = Color.black;
                        }
                        else
                        {
                            slot.imageRarity.color = player.rarity.GetColor(player.shop.heroes[i].hero.rarity);
                            slot.image.sprite = player.shop.heroes[i].hero.image;
                            slot.image.color = Color.white;
                        }

                        slot.amount.text = player.shop.heroes[i].amount.ToString();
                        slot.price.text = UIUtils.LongToString(player.shop.heroes[i].price);
                        slot.imagePrice.sprite = player.shop.heroes[i].sellfor.image;

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            //check coins
                            if (player.items.GetItemAmount(player.coinsItem) >= player.shop.heroes[icopy].price)
                            {
                                _audio.PlaySoundButtonClick();
                                player.shop.BuyHeroParts(icopy);
                            }
                            else
                            {
                                panelError.SetActive(true);
                            }
                        });
                    }
                }
                else panel.SetActive(false);
            }
        }

        //public void OnPurchaseComplete(Product product)
        //{
        //    Player player = Player.localPlayer;
        //    if (player != null)
        //    {
        //        if (product.definition.id == "com.gameforfun.wildhorizon.noAds")
        //        {
        //            player.shop.CmdBuyStarterKit();
        //        }
        //        else if (product.definition.id == "com.gameforfun.wildhorizon.coins50")
        //        {
        //            player.items.IncreaseItemAmount(player.coinsItem, 50);
        //            MoveIconToPosition(player, ShopItemsType.Coins, 0, 50, ItemRarity.Normal, player.coinsItem.image);
        //        }
        //        else if (product.definition.id == "com.gameforfun.wildhorizon.coins200")
        //        {
        //            player.items.IncreaseItemAmount(player.coinsItem, 200);
        //            MoveIconToPosition(player, ShopItemsType.Coins, 1, 200, ItemRarity.Normal, player.coinsItem.image);
        //        }
        //        else if (product.definition.id == "com.gameforfun.wildhorizon.coins500")
        //        {
        //            player.items.IncreaseItemAmount(player.coinsItem, 500);
        //            MoveIconToPosition(player, ShopItemsType.Coins, 2, 500, ItemRarity.Normal, player.coinsItem.image);
        //        }
        //        else if (product.definition.id == "com.gameforfun.wildhorizon.coins1000")
        //        {
        //            player.items.IncreaseItemAmount(player.coinsItem, 1000);
        //            MoveIconToPosition(player, ShopItemsType.Coins, 3, 1000, ItemRarity.Normal, player.coinsItem.image);
        //        }
        //        else if (product.definition.id == "com.gameforfun.wildhorizon.coins2000")
        //        {
        //            player.items.IncreaseItemAmount(player.coinsItem, 2000);
        //            MoveIconToPosition(player, ShopItemsType.Coins, 4, 2000, ItemRarity.Normal, player.coinsItem.image);
        //        }
        //        else if (product.definition.id == "com.gameforfun.wildhorizon.coins4000")
        //        {
        //            player.items.IncreaseItemAmount(player.coinsItem, 4000);
        //            MoveIconToPosition(player, ShopItemsType.Coins, 5, 4000, ItemRarity.Normal, player.coinsItem.image);
        //        }
        //    }
        //}

        //public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        //{
        //    Debug.Log(reason);
        //    ShowErrorPanel();
        //}

        public void MoveIconToPosition(Player player, ShopItemsType type, int index, uint amount, ItemRarity rarity, Sprite sprite)
        {
            //анимация движения предмета 
            GameObject go = Instantiate(prefabItem);
            go.transform.SetParent(canvas.transform, false);

            go.GetComponent<ItemsPrefabForAnimation>().sprite.sprite = sprite;
            go.GetComponent<ItemsPrefabForAnimation>().textAmount.text = amount.ToString();

            if (type == ShopItemsType.Coins)
            {
                go.GetComponent<ItemsPrefabForAnimation>().target = coinsPosition;
                go.transform.position = coinsTransform.transform.GetChild(index).transform.position;
            }
            else if (type == ShopItemsType.Inhabitants)
            {
                go.GetComponent<ItemsPrefabForAnimation>().sprite.sprite = sprite;
                go.GetComponent<ItemsPrefabForAnimation>().textAmount.text = amount.ToString();
                go.GetComponent<ItemsPrefabForAnimation>().target = inhabitantsPosition;
                go.transform.position = inhabitantsTransform.transform.GetChild(index).transform.position;
            }
            else if (type == ShopItemsType.Resources)
            {
                go.GetComponent<ItemsPrefabForAnimation>().rarity.color = player.rarity.GetColor(rarity);
                go.GetComponent<ItemsPrefabForAnimation>().target = townPosition;
                go.transform.position = resourcesTransform.transform.GetChild(index).transform.position;
            }
            else if (type == ShopItemsType.Instruments)
            {
                go.GetComponent<ItemsPrefabForAnimation>().rarity.color = player.rarity.GetColor(rarity);
                go.GetComponent<ItemsPrefabForAnimation>().target = townPosition;
                go.transform.position = instrumentsTransform.transform.GetChild(index).transform.position;
            }
            else if (type == ShopItemsType.Animals)
            {
                go.GetComponent<ItemsPrefabForAnimation>().rarity.color = player.rarity.GetColor(rarity);
                go.GetComponent<ItemsPrefabForAnimation>().target = townPosition;
                go.transform.position = animalsTransform.transform.GetChild(index).transform.position;
            }
            else if (type == ShopItemsType.Weapon)
            {
                go.GetComponent<ItemsPrefabForAnimation>().rarity.color = player.rarity.GetColor(rarity);
                go.GetComponent<ItemsPrefabForAnimation>().target = townPosition;
                go.transform.position = weaponTransform.transform.GetChild(index).transform.position;
            }
            else if (type == ShopItemsType.Heroes)
            {
                go.GetComponent<ItemsPrefabForAnimation>().rarity.color = player.rarity.GetColor(rarity);
                go.GetComponent<ItemsPrefabForAnimation>().target = heroesPosition;
                go.transform.position = heroesTransform.transform.GetChild(index).transform.position;
            }
        }
    }
}