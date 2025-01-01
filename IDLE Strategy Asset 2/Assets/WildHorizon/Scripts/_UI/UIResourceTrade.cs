using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIResourceTrade : MonoBehaviour
    {
        public ScriptableBuilding requiredBuilding;

        public GameObject panel;
        public Transform content;
        public UIResourceSlot prefab;
        public Button buttonSell;
        public Button buttonBuy;
        public Dropdown dropdownSort;

        [Header("Colors")]
        public Color selectedColor;
        public Color normalColor;

        [Header("UI Elements : Trade item")]
        public Text textNameValue;
        public Text textSellDollarsValue;
        public Text textBuyDollarsValue;
        public Text textDollarsInStock;
        public InputField textAmountValue;
        public Button buttonPlus;
        public Button buttonMinus;

        [Header("Info Panel")]
        public GameObject panelInfo;
        public Text textInfo;

        private uint tradeAmount = 1;
        ScriptableItemAndAmount sellItem;
        private ScriptableItem selectedItem;

        [Header("Components")]
        public UIAudio _audio;

        // Update is called once per frame
        void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    List<ScriptableItemAndAmount> list = player.items.ItemsForTrade();

                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(prefab.gameObject, list.Count, content);

                    // refresh all slots
                    for (int i = 0; i < list.Count; i++)
                    {
                        UIResourceSlot slot = content.transform.GetChild(i).GetComponent<UIResourceSlot>();

                        slot.image.sprite = list[i].item.image;

                        //name by language
                        slot.textName.text = Localization.Translate(list[i].item.name);

                        slot.textValue.text = UIUtils.LongToString(list[i].amount);
                        slot.textPrice.text = UIUtils.LongToString(list[i].item.sellPrice) + "$ / " + UIUtils.LongToString(list[i].item.buyPrice) + "$";

                        int icopy = i;
                        slot.button.interactable = true;
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            selectedItem = list[icopy].item;
                        });

                        //change color
                        if (selectedItem != null && selectedItem.Equals(list[i].item)) slot.slotImage.color = selectedColor;
                        else slot.slotImage.color = normalColor;
                    }

                    if (selectedItem != null)
                    {
                        //name by language
                        textNameValue.text = Localization.Translate(selectedItem.name);

                        //amount
                        textAmountValue.text = tradeAmount.ToString();
                        textAmountValue.onValueChanged.SetListener(delegate { tradeAmount = Convert.ToUInt32(textAmountValue.text); });

                        buttonPlus.onClick.SetListener(() =>
                        {
                            tradeAmount += 1;
                        });
                        buttonMinus.onClick.SetListener(() =>
                        {
                            if (tradeAmount > 1) tradeAmount -= 1;
                        });

                        //dollars
                        uint dollars = player.items.GetItemAmount(player.dollarsItem);
                        textSellDollarsValue.text = UIUtils.LongToString(tradeAmount * selectedItem.sellPrice) + "$ / " + UIUtils.LongToString(dollars) + "$";
                        textBuyDollarsValue.text = UIUtils.LongToString(tradeAmount * selectedItem.buyPrice) + "$ / " + UIUtils.LongToString(dollars) + "$";
                    }
                    else
                    {
                        textNameValue.text = "-";
                        textAmountValue.text = "0";
                        textSellDollarsValue.text = "";
                        textBuyDollarsValue.text = "";
                        tradeAmount = 1;
                    }

                    buttonSell.interactable = textAmountValue.text.ToUInt() > 0 && player.items.GetItemAmount(selectedItem) > 0;
                    buttonSell.onClick.SetListener(() =>
                    {
                        sellItem.item = selectedItem;

                        //если хотим продать больше чем есть
                        uint amountSelectedItem = player.items.GetItemAmount(selectedItem);
                        if (tradeAmount > amountSelectedItem)
                        {
                            panelInfo.SetActive(true);
                            textInfo.text = "There are only " + amountSelectedItem + " pieces of this item in stock";
                            sellItem.amount = amountSelectedItem;
                        }
                        else
                        {
                            //колво денег которое мы получим
                            uint dollars = (tradeAmount * selectedItem.sellPrice);
                            uint dollarsOnPlayer = player.items.GetItemAmount(player.dollarsItem);

                            //если текущее кол во денег + получаемое меньше или равно макимальному
                            if (dollarsOnPlayer + dollars <= player.buildings.StorageSizeByType(StorageType.bank))
                            {
                                _audio.PlaySoundButtonClick();

                                player.items.DecreaseItemAmount(selectedItem, tradeAmount);

                                //add dollars
                                player.items.IncreaseItemAmount(player.dollarsItem, dollars);

                                player.quests.CheckAllQuestEvent();
                            }
                            else
                            {
                                panelInfo.SetActive(true);
                                uint amount = (player.buildings.StorageSizeByType(StorageType.bank) - dollarsOnPlayer) / selectedItem.sellPrice;
                                textInfo.text = "Bank stocks are limited\nonly " + amount + Localization.Translate(selectedItem.name) + " can be sold";
                                sellItem.amount = amount;
                            }
                        }
                    });

                    buttonBuy.interactable = textAmountValue.text.ToUInt() > 0;
                    buttonBuy.onClick.SetListener(() =>
                    {
                        uint dollars = (tradeAmount * selectedItem.buyPrice);

                        if (dollars <= player.items.GetItemAmount(player.dollarsItem))
                        {
                            _audio.PlaySoundButtonClick();

                            player.items.IncreaseItemAmount(selectedItem, tradeAmount);

                            //decrease dollars
                            player.items.DecreaseItemAmount(player.dollarsItem, dollars);

                            player.quests.CheckAllQuestEvent();
                        }
                        else
                        {

                        }
                    });

                    textDollarsInStock.text = UIUtils.LongToString(player.items.GetItemAmount(player.dollarsItem)) + "/" + UIUtils.LongToString(player.buildings.StorageSizeByType(StorageType.bank));
                }
            }
        }

        public void SellConfirm()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                panelInfo.SetActive(false);

                player.items.DecreaseItemAmount(sellItem.item, sellItem.amount);

                //add dollars
                player.items.IncreaseItemAmount(player.dollarsItem, sellItem.amount * sellItem.item.sellPrice);

                player.quests.CheckAllQuestEvent();
            }
        }
    }
}


