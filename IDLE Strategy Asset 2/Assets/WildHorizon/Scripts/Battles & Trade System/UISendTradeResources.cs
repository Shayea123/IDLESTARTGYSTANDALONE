using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UISendTradeResources : MonoBehaviour
    {
        public GameObject panel;

        [Header("Colors")]
        public Color colorFailure;
        public Color colorNormal;

        [Header("Camp")]
        public Transform contentItemsInCamp;
        public GameObject prefabReadySlot;

        [Header("Sending items")]
        public Transform content;
        public GameObject prefab;
        public Button buttonClose;
        public Button buttonSend;
        public Text weight;
        public Text workersRequired;
        public Text workersCurrent;
        public Text time;

        [Header("Select items")]
        public GameObject panelSelectItems;
        private ScriptableItem selectedItem_1, selectedItem_2;
        private uint selectedAmount_1, selectedAmount_2;
        public Image imageSelectItem_1, imageSelectItem_2;
        public TMP_Dropdown dropdownSelectItem_1, dropdownSelectItem_2;
        public TextMeshProUGUI textAmountItem_1, textAmountItem_2;
        public InputField inputFieldSelectItem_1, inputFieldSelectItem_2;
        public Button buttonMinus;
        public Button buttonPlus;
        public Text textExchangeRateValue;
        public Button buttonSelectItemsApply;

        public int selectedSlot = -1;

        [Header("Panel Error")]
        public GameObject panelError;
        public Text textError;

        [Header("Components")]
        public GameObject panelBattleIsOver;
        public GameObject panelBattles;
        public UIAudio _audio;

        private void Start()
        {
            buttonClose.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                if (panelError.activeSelf) panelError.SetActive(false);
                else if (panelSelectItems.activeSelf)
                {
                    panelSelectItems.SetActive(false);
                    selectedItem_2 = null;
                }
                else
                {
                    panel.SetActive(false);
                    panelBattles.SetActive(true);
                }
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    //show all resources for trade that we can exchange
                    List<ScriptableItemAndAmount> resourcesForBarter = player.items.ItemsTradeByCamp();

                    List<ItemSlot> tradeItemsEdit = TradeItemsEdit(player, player.camps.selectedEnemyCamp.tradeItems);
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(prefabReadySlot, player.camps.selectedGoodsForTrade.Count + tradeItemsEdit.Count, contentItemsInCamp);
                    for (int i = 0; i < player.camps.selectedGoodsForTrade.Count; i++)
                    {
                        UITradeCampItemSlot slot = contentItemsInCamp.transform.GetChild(i).GetComponent<UITradeCampItemSlot>();

                        //image
                        slot.image_1.sprite = player.camps.selectedGoodsForTrade[i].slot1.item.image;
                        slot.image_2.sprite = player.camps.selectedGoodsForTrade[i].slot2.item.image;
                        slot.image_2.color = Color.white;

                        //name
                        slot.text_1_name.text = Localization.Translate(player.camps.selectedGoodsForTrade[i].slot1.item.name);
                        slot.text_2_name.text = Localization.Translate(player.camps.selectedGoodsForTrade[i].slot2.item.name);

                        //amount
                        slot.text_1_amount.text = player.camps.selectedGoodsForTrade[i].slot1.amount.ToString();
                        slot.text_2_amount.text = player.camps.selectedGoodsForTrade[i].slot2.amount.ToString();

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();

                            dropdownSelectItem_1.ClearOptions();
                            List<string> items = new List<string>() { };
                            for (int i = 0; i < player.camps.selectedEnemyCamp.tradeItems.Length; i++)
                                items.Add(Localization.Translate(player.camps.selectedEnemyCamp.tradeItems[i].item.name));
                        // + " : " + player.battles.selectedEnemyCamp.tradeItems[i].amount

                        dropdownSelectItem_1.AddOptions(items);
                            dropdownSelectItem_1.value = player.camps.FindItemIndexInCampTradeItems(player.camps.selectedEnemyCamp, player.camps.selectedGoodsForTrade[icopy].slot1.item.data);

                            FillDropdownSellItem(resourcesForBarter);
                            dropdownSelectItem_2.value = (FindItemIndexTradeItems(resourcesForBarter, player.camps.selectedGoodsForTrade[icopy].slot2.item.data) + 1);

                        //items
                        selectedItem_1 = player.camps.selectedGoodsForTrade[icopy].slot1.item.data;
                            selectedItem_2 = player.camps.selectedGoodsForTrade[icopy].slot2.item.data;

                        //amount
                        selectedAmount_1 = player.camps.selectedGoodsForTrade[icopy].slot1.amount;
                            selectedAmount_2 = player.camps.selectedGoodsForTrade[icopy].slot2.amount;

                            selectedSlot = icopy;

                            panelSelectItems.SetActive(true);
                        });
                    }
                    for (int i = 0; i < tradeItemsEdit.Count; i++)
                    {
                        UITradeCampItemSlot slot = contentItemsInCamp.transform.GetChild(i + player.camps.selectedGoodsForTrade.Count).GetComponent<UITradeCampItemSlot>();

                        //image
                        slot.image_1.sprite = tradeItemsEdit[i].item.image;
                        slot.image_2.sprite = null;
                        slot.image_2.color = Color.clear;

                        //name
                        slot.text_1_name.text = Localization.Translate(tradeItemsEdit[i].item.name);
                        slot.text_2_name.text = "";

                        //amount
                        uint amount = player.camps.GetFreeBuyItemAmountInSelectedGoodsForTrade(player.camps.selectedEnemyCamp, tradeItemsEdit[i].item.data);
                        slot.text_1_amount.text = amount.ToString();
                        slot.text_2_amount.text = "";

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();

                            dropdownSelectItem_1.ClearOptions();
                            List<string> items = new List<string>() { };
                            for (int i = 0; i < tradeItemsEdit.Count; i++)
                                items.Add(Localization.Translate(tradeItemsEdit[i].item.name));

                            dropdownSelectItem_1.AddOptions(items);
                            dropdownSelectItem_1.value = icopy;

                            selectedItem_1 = tradeItemsEdit[icopy].item.data;
                            selectedAmount_1 = 0;

                        //FillDropdownSend();
                        FillDropdownSellItem(resourcesForBarter);
                            dropdownSelectItem_2.value = 0;

                            selectedSlot = -1;

                            panelSelectItems.SetActive(true);
                        });
                    }


                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(prefab, resourcesForBarter.Count, content);
                    for (int i = 0; i < resourcesForBarter.Count; i++)
                    {
                        UITradeItemSlot slot = content.transform.GetChild(i).GetComponent<UITradeItemSlot>();

                        //image
                        slot.image.sprite = resourcesForBarter[i].item.image;

                        //name
                        slot.textName.text = Localization.Translate(resourcesForBarter[i].item.name);

                        //amount
                        slot.textAmount.text = (resourcesForBarter[i].amount - player.camps.GetSellItemAmountInSelectedGoodsForTrade(resourcesForBarter[i].item)).ToString();

                        //item
                        slot.item = resourcesForBarter[i].item;
                    }

                    buttonSend.gameObject.SetActive(!panelError.activeSelf);
                    buttonSend.onClick.SetListener(() =>
                    {
                        if (player.camps.selectedGoodsForTrade.Count > 0)
                        {
                            player.camps.CmdStartTrade(player.camps.selectedEnemyCamp._hash, 2);
                            panel.SetActive(false);
                            panelBattles.SetActive(true);
                        }
                        else
                        {
                            textError.text = "for trade you need to send a cargo";
                            panelError.SetActive(true);
                            panel.SetActive(false);
                        }

                    //проверяем телеги
                    /*if (currentMaxWeight == 0)
                    {
                        textError.text = "Need a cart to trade";
                        panelError.SetActive(true);
                    }
                    //проверяем отправляемый груз
                    else if (currentWeight == 0)
                    {
                        textError.text = "You can't trade with an empty cart.\nSend some cargo";
                        panelError.SetActive(true);
                    }
                    else if (requeredWorkers > player.inhabitants.InhabitantsFree())
                    {
                        textError.text = "Not enough free workers";
                        panelError.SetActive(true);
                    }
                    else
                    {
                        _audio.PlaySoundButtonClick();
                        //SendItemsForTrade(player, requeredWorkers, player.battles.selectedEnemyCamp._hash, SelectedItems(player.battles.selectedEnemyCamp.data));
                        player.battles.AddToBarterList(true, player.battles.selectedEnemyCamp.location, selectedItem_1.name.GetStableHashCode(), selectedAmount_1, selectedItem_2.name.GetStableHashCode(), selectedAmount_2);
                        panel.SetActive(false);
                        panelBattles.SetActive(true);
                    }*/


                    //проверяем выбранные предметы
                    /*if (PresentSelectedItem() == false)
                    {
                        //проверяем телеги
                        if (currentMaxWeight == 0)
                        {
                            textError.text = "Need a cart to trade";
                            panelError.SetActive(true);
                        }
                        //проверяем отправляемый груз
                        else if (currentWeight == 0)
                        {
                            textError.text = "You can't trade with an empty cart.\nSend some cargo";
                            panelError.SetActive(true);
                        }
                        else if (requeredWorkers > player.inhabitants.InhabitantsFree())
                        {
                            textError.text = "Not enough free workers";
                            panelError.SetActive(true);
                        }
                        else
                        {
                            _audio.PlaySoundButtonClick();
                            //SendItemsForTrade(player, requeredWorkers, player.battles.selectedEnemyCamp._hash, SelectedItems(player.battles.selectedEnemyCamp.data));
                            panel.SetActive(false);
                            panelBattles.SetActive(true);
                        }
                    }
                    else
                    {
                        textError.text = "You need to choose the products you need";
                        panelError.SetActive(true);
                    }*/
                    });

                    //if this camp is disappeared
                    if (player.camps.GetCampIndexByHash(player.camps.selectedEnemyCamp._hash) == -1)
                    {
                        panelBattleIsOver.SetActive(true);
                        panel.SetActive(false);
                    }

                    if (panelSelectItems.activeSelf)
                    {
                        //show slot1 descriptions
                        imageSelectItem_1.sprite = selectedItem_1.image;
                        //amount
                        uint amount = player.camps.GetFreeBuyItemAmountInSelectedGoodsForTrade(player.camps.selectedEnemyCamp, selectedItem_1);
                        textAmountItem_1.text = amount.ToString();

                        if (inputFieldSelectItem_1.isFocused == false) inputFieldSelectItem_1.text = selectedAmount_1.ToString();
                        else if (inputFieldSelectItem_1.text.ToUInt() > amount) inputFieldSelectItem_1.text = amount.ToString();
                        inputFieldSelectItem_1.onEndEdit.SetListener(delegate
                        {
                            if (selectedItem_1.sellPrice > selectedItem_2.sellPrice)
                            {
                                selectedAmount_1 = inputFieldSelectItem_1.text.ToUInt();
                                selectedAmount_2 = selectedAmount_1 * (uint)(Math.Ceiling(((double)selectedItem_1.sellPrice / selectedItem_2.sellPrice)));
                            }
                            else
                            {
                                uint amounttemp = inputFieldSelectItem_1.text.ToUInt();
                                amounttemp = (uint)(Math.Ceiling(((double)amounttemp / selectedItem_2.sellPrice)));
                                if (amounttemp < 1) amounttemp = 1;
                                selectedAmount_1 = selectedItem_2.sellPrice * amounttemp;
                                selectedAmount_2 = selectedItem_1.sellPrice * amounttemp;
                            }
                        });

                        if (selectedItem_2 != null)
                        {
                            //image
                            imageSelectItem_2.sprite = selectedItem_2.image;
                            imageSelectItem_2.color = Color.white;

                            //amount 
                            textAmountItem_2.text = player.items.GetItemAmount(selectedItem_2).ToString();

                            //input amount
                            inputFieldSelectItem_1.interactable = true;
                            inputFieldSelectItem_2.interactable = true;
                            if (inputFieldSelectItem_2.isFocused == false) inputFieldSelectItem_2.text = selectedAmount_2.ToString();
                            else if (inputFieldSelectItem_2.text.ToUInt() > player.items.GetItemAmount(selectedItem_2)) inputFieldSelectItem_2.text = player.items.GetItemAmount(selectedItem_2).ToString();

                            inputFieldSelectItem_2.onEndEdit.SetListener(delegate
                            {
                                if (selectedItem_1.sellPrice > selectedItem_2.sellPrice)
                                {
                                    uint amounttemp = inputFieldSelectItem_2.text.ToUInt();
                                    amounttemp = (uint)(Math.Ceiling(((double)amounttemp / selectedItem_1.sellPrice)));
                                    if (amounttemp < 1) amounttemp = 1;
                                    selectedAmount_1 = selectedItem_2.sellPrice * amounttemp;
                                    selectedAmount_2 = selectedItem_1.sellPrice * amounttemp;
                                }
                                else
                                {
                                    selectedAmount_2 = inputFieldSelectItem_2.text.ToUInt();
                                    selectedAmount_1 = selectedAmount_2 * (uint)(Math.Ceiling(((double)selectedItem_2.sellPrice / selectedItem_1.sellPrice)));
                                }
                            });

                            if (selectedItem_1.sellPrice > selectedItem_2.sellPrice)
                            {
                                textExchangeRateValue.text = "1 / " + Math.Ceiling(((double)selectedItem_1.sellPrice / selectedItem_2.sellPrice));
                            }
                            else
                            {
                                textExchangeRateValue.text = Math.Ceiling(((double)selectedItem_2.sellPrice / selectedItem_1.sellPrice)) + " / 1";
                            }

                            //change colors
                            if (selectedAmount_1 <= amount) inputFieldSelectItem_1.textComponent.color = colorNormal;
                            else inputFieldSelectItem_1.textComponent.color = colorFailure;

                            if (selectedAmount_2 <= player.items.GetItemAmount(selectedItem_2)) inputFieldSelectItem_2.textComponent.color = colorNormal;
                            else inputFieldSelectItem_2.textComponent.color = colorFailure;

                            //buttons
                            buttonPlus.interactable = true;
                            buttonMinus.interactable = true;
                            buttonMinus.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();

                                if (selectedItem_1.sellPrice > selectedItem_2.sellPrice)
                                {
                                    if (selectedAmount_1 > 1)
                                    {
                                        selectedAmount_1 -= 1;
                                        selectedAmount_2 = selectedAmount_1 * (uint)(Math.Ceiling(((double)selectedItem_1.sellPrice / selectedItem_2.sellPrice)));
                                    }
                                }
                                else
                                {
                                    if (selectedAmount_2 > 1)
                                    {
                                        selectedAmount_2 -= 1;
                                        selectedAmount_1 = selectedAmount_2 * (uint)(Math.Ceiling(((double)selectedItem_2.sellPrice / selectedItem_1.sellPrice)));
                                    }
                                }
                            });
                            buttonPlus.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();

                            //увеличиваем количество на 1 того слота у которого стоимость выше за 1 шт
                            if (selectedItem_1.sellPrice > selectedItem_2.sellPrice)
                                {
                                    selectedAmount_1 += 1;
                                    selectedAmount_2 = selectedAmount_1 * (uint)(Math.Ceiling(((double)selectedItem_1.sellPrice / selectedItem_2.sellPrice)));
                                }
                                else
                                {
                                    selectedAmount_2 += 1;
                                    selectedAmount_1 = selectedAmount_2 * (uint)(Math.Ceiling(((double)selectedItem_2.sellPrice / selectedItem_1.sellPrice)));
                                }
                            });
                        }
                        else
                        {
                            //image
                            imageSelectItem_2.color = Color.clear;
                            textExchangeRateValue.text = "";

                            //amount
                            textAmountItem_2.text = "0";

                            //input amount
                            inputFieldSelectItem_1.interactable = false;
                            inputFieldSelectItem_2.interactable = false;
                            inputFieldSelectItem_1.text = "0";
                            inputFieldSelectItem_2.text = "0";

                            inputFieldSelectItem_1.textComponent.color = colorNormal;
                            inputFieldSelectItem_2.textComponent.color = colorNormal;

                            //buttons
                            buttonPlus.interactable = false;
                            buttonMinus.interactable = false;
                        }

                        dropdownSelectItem_1.onValueChanged.SetListener(delegate
                        {
                            selectedItem_1 = player.camps.selectedEnemyCamp.tradeItems[dropdownSelectItem_1.value].item.data;

                            if (selectedItem_2 != null)
                            {
                                if (selectedItem_1.sellPrice > selectedItem_2.sellPrice)
                                {
                                    selectedAmount_1 = 1;
                                    selectedAmount_2 = (uint)(Math.Ceiling(((double)selectedItem_1.sellPrice / selectedItem_2.sellPrice)));
                                }
                                else
                                {
                                    selectedAmount_2 = 1;
                                    selectedAmount_1 = (uint)(Math.Ceiling(((double)selectedItem_2.sellPrice / selectedItem_1.sellPrice)));
                                }
                            }
                            else selectedAmount_1 = 1;
                        });
                        dropdownSelectItem_2.onValueChanged.SetListener(delegate
                        {
                            if (dropdownSelectItem_2.value > 0)
                            {
                                selectedItem_2 = resourcesForBarter[dropdownSelectItem_2.value - 1].item;

                                if (selectedItem_1.sellPrice > selectedItem_2.sellPrice)
                                {
                                    selectedAmount_1 = 1;
                                    selectedAmount_2 = (uint)(Math.Ceiling(((double)selectedItem_1.sellPrice / selectedItem_2.sellPrice)));
                                }
                                else
                                {
                                    selectedAmount_2 = 1;
                                    selectedAmount_1 = selectedAmount_2 * (uint)(Math.Ceiling(((double)selectedItem_2.sellPrice / selectedItem_1.sellPrice)));
                                }
                            }
                            else
                            {
                                selectedItem_2 = null;
                            }
                        });

                        buttonSelectItemsApply.gameObject.SetActive(selectedAmount_1 > 0 && selectedAmount_2 > 0 && selectedAmount_2 <= player.items.GetItemAmount(selectedItem_2) && selectedAmount_1 <= amount);
                        buttonSelectItemsApply.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            if (selectedAmount_1 > 0 && selectedAmount_2 > 0)
                            {
                                if (!selectedItem_1.Equals(selectedItem_2))
                                {
                                    player.camps.CmdAddToTradeList(player.camps.selectedEnemyCamp._hash, selectedItem_1.name, selectedAmount_1, selectedItem_2.name, selectedAmount_2);
                                    panelSelectItems.SetActive(false);
                                    selectedItem_2 = null;
                                }
                                else
                                {
                                //показываем сообщение что нельзя менять одинаковые товары
                                textError.text = "Нельзя менять одинаковые товары";
                                    panelError.SetActive(true);
                                }
                            }
                            else
                            {
                                if (selectedSlot != -1) player.camps.CmdRemoveFromTradeList(selectedSlot);
                            }
                        });
                    }
                }
                else panel.SetActive(false);
            }
        }

        void FillDropdownSellItem(List<ScriptableItemAndAmount> resourcesForBarter)
        {
            dropdownSelectItem_2.ClearOptions();

            List<string> items = new List<string>() { };

            items.Add("none");
            for (int i = 0; i < resourcesForBarter.Count; i++)
                if (!resourcesForBarter[i].item.Equals(selectedItem_1))
                    items.Add(Localization.Translate(resourcesForBarter[i].item.name));

            dropdownSelectItem_2.AddOptions(items);
        }
        int FindItemIndexTradeItems(List<ScriptableItemAndAmount> resourcesForBarter, ScriptableItem item)
        {
            for (int i = 0; i < resourcesForBarter.Count; i++)
            {
                if (resourcesForBarter[i].item.Equals(item)) return i;
            }

            return -1;
        }

        List<ItemSlot> TradeItemsEdit(Player player, ItemSlot[] tradeItems)
        {
            List<ItemSlot> list = new List<ItemSlot>();

            for (int i = 0; i < tradeItems.Length; i++)
            {
                uint amount = CheckItem(player, tradeItems[i].item.data);
                if (amount > 0)
                {
                    list.Add(new ItemSlot(tradeItems[i].item, tradeItems[i].amount - amount));
                }
                else list.Add(tradeItems[i]);
            }

            return list;
        }
        uint CheckItem(Player player, ScriptableItem item)
        {
            for (int i = 0; i < player.camps.selectedGoodsForTrade.Count; i++)
            {
                if (player.camps.selectedGoodsForTrade[i].slot1.item.data.Equals(item)) return player.camps.selectedGoodsForTrade[i].slot1.amount;
            }

            return 0;
        }
    }
}


