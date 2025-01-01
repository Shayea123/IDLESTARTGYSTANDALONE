using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UITown : MonoBehaviour
    {
        [Header("Components")]
        public UIAudio _audio;
        public UIBuild _build;

        private enum ShowItemsType { TownInfo, Resources, Instruments, Animals, Weapon, Army, Manage }
        private ShowItemsType showItemsType = ShowItemsType.TownInfo;
        private SortBy sortBy = SortBy.name;

        [Header("Panels")]
        public GameObject panel;
        public Button buttonClose;

        [Header("Town info")]
        public GameObject panelTownInfo;
        public Text textTownName;
        public Text textType;
        public Text textTownDollarsValue;
        public Text textItemsAmountMaxValue;
        public Text textWaterAmountMaxValue;
        public Text textIncomeFromTaxesValue;

        [Header("Inhabitants")]
        public Text textInhabitantsValue;
        public Text textInhabitantsMaxValue;
        public Text textInhabitantsInArmyValue;
        public Text textPopulationGrowthValue;

        [Header("relations with other factions")]
        public Slider sliderFactionIndians;
        public Slider sliderFactionCowboys;
        public Slider sliderFactionBandits;
        public Slider sliderFactionMexicans;

        [Header("Buttons")]
        public Button buttonShowTownInfo;
        public Button buttonShowResources;
        public Button buttonShowInstruments;
        public Button buttonShowAnimals;
        public Button buttonShowWeapon;
        public Button buttonShowArmy;
        public Button buttonShowManage;

        [Header("Sort")]
        public Button buttonSortByName;
        public Button buttonSortByAmount;

        [Header("Storage")]
        public GameObject panelStorage;
        public Transform contentStorage;
        public UIResourceTownSlot prefab;

        [Header("Manage")]
        public Transform contentManage;
        public Button buttonUpgrade;

        [Header("Rename Town")]
        public GameObject panelTownRename;
        public InputField inputFieldtownRename;
        public Button buttonTownRename;
        public Button buttonConfirmTownRename;

        [Header("Send Workers")]
        public Text textFreeWorkers;
        public Transform sendWorkersContent;
        public GameObject sendWorkersPrefab;

        [Header("Error")]
        public GameObject panelError;
        public Text textError;

        public static UITown singleton;
        public UITown()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            if (singleton == null) singleton = this;
        }

        private void Start()
        {
            buttonShowTownInfo.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                showItemsType = ShowItemsType.TownInfo;
            });
            buttonShowResources.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                showItemsType = ShowItemsType.Resources;
            });
            buttonShowInstruments.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                showItemsType = ShowItemsType.Instruments;
            });
            buttonShowAnimals.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                showItemsType = ShowItemsType.Animals;
            });
            buttonShowWeapon.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                showItemsType = ShowItemsType.Weapon;
            });
            buttonShowArmy.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                showItemsType = ShowItemsType.Army;
            });
            buttonShowManage.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                showItemsType = ShowItemsType.Manage;
            });

            buttonClose.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                if (panelError.activeSelf) panelError.SetActive(false);
                else if (panelTownRename.activeSelf) panelTownRename.SetActive(false);
                else
                {
                    showItemsType = ShowItemsType.TownInfo;
                    panel.SetActive(false);
                }
            });

            //sort
            buttonSortByName.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                sortBy = SortBy.name;
            });
            buttonSortByAmount.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                if (sortBy != SortBy.amountAscending) sortBy = SortBy.amountAscending;
                else sortBy = SortBy.amountDescending;
            });
        }

        private void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null && player.TownMenuAvailable())
                {
                    //buttons
                    buttonShowTownInfo.interactable = showItemsType != ShowItemsType.TownInfo;
                    buttonShowResources.interactable = showItemsType != ShowItemsType.Resources;
                    buttonShowInstruments.interactable = showItemsType != ShowItemsType.Instruments;
                    buttonShowAnimals.interactable = showItemsType != ShowItemsType.Animals;
                    buttonShowWeapon.interactable = showItemsType != ShowItemsType.Weapon;
                    buttonShowArmy.interactable = showItemsType != ShowItemsType.Army;
                    buttonShowManage.interactable = showItemsType != ShowItemsType.Manage;

                    if (showItemsType == ShowItemsType.TownInfo)
                    {
                        panelTownInfo.SetActive(true);
                        panelStorage.gameObject.SetActive(false);
                        contentManage.gameObject.SetActive(false);

                        //show town name
                        if (string.IsNullOrEmpty(player.townName) == false) textTownName.text = player.townName;
                        else textTownName.text = Localization.Translate("City without a name");

                        //inhabitants
                        textInhabitantsValue.text = player.inhabitants.InhabitantsFree() + "/" + player.inhabitants.GetCurrent();
                        if (player.inhabitants.GetReserve() == 0) textInhabitantsMaxValue.text = player.inhabitants.InhabitantsMax().ToString();
                        else textInhabitantsMaxValue.text = player.inhabitants.InhabitantsMax() + "/ +" + player.inhabitants.GetReserve();

                        textInhabitantsInArmyValue.text = player.army.InhabitantsInArmy().ToString();
                        textPopulationGrowthValue.text = player.inhabitants.PopulationGrowth();

                        //storages
                        textTownDollarsValue.text = UIUtils.LongToString(player.items.TotalItemsByStorageType(StorageType.bank)) + "/" + UIUtils.LongToString(player.buildings.StorageSizeByType(StorageType.bank));
                        textIncomeFromTaxesValue.text = player.items.DollarsGrowth();
                        textItemsAmountMaxValue.text = UIUtils.LongToString(player.items.TotalItemsByStorageType(StorageType.warehouse)) + "/" + UIUtils.LongToString(player.items.GetStartingStorageCapacityByType(StorageType.warehouse) + player.buildings.StorageSizeByType(StorageType.warehouse));
                        textWaterAmountMaxValue.text = UIUtils.LongToString(player.items.TotalItemsByStorageType(StorageType.waterTower)) + "/" + UIUtils.LongToString(player.items.GetStartingStorageCapacityByType(StorageType.waterTower) + player.buildings.StorageSizeByType(StorageType.waterTower));

                        //relations with other factions
                        sliderFactionIndians.value = player.camps.factionIndians;
                        sliderFactionCowboys.value = player.camps.factionCowboys;
                        sliderFactionBandits.value = player.camps.factionBandits;
                        sliderFactionMexicans.value = player.camps.factionMexicans;
                    }
                    else if (showItemsType != ShowItemsType.TownInfo && showItemsType != ShowItemsType.Manage)
                    {
                        textType.text = Localization.Translate(showItemsType.ToString());

                        panelTownInfo.SetActive(false);
                        panelStorage.gameObject.SetActive(true);
                        contentManage.gameObject.SetActive(false);

                        List<ItemSlot> list = new List<ItemSlot>();

                        switch (showItemsType)
                        {
                            case ShowItemsType.Resources:
                                list = player.items.GetItemsListByType(new ScriptableResource());
                                break;
                            case ShowItemsType.Instruments:
                                list = player.items.GetItemsListByType(new ScriptableInstrument());
                                break;
                            case ShowItemsType.Animals:
                                list = player.items.GetItemsListByType(new ScriptableAnimal());
                                break;
                            case ShowItemsType.Weapon:
                                list = player.items.GetItemsListByType(new ScriptableWeapon());
                                break;
                            case ShowItemsType.Army:
                                list = player.army.AvailebleArmy();
                                break;
                        }

                        // instantiate/destroy enough slots
                        UIUtils.BalancePrefabs(prefab.gameObject, list.Count, contentStorage);

                        //sort
                        if (sortBy == SortBy.name)
                        {
                            list.Sort(delegate (ItemSlot x, ItemSlot y)
                            {
                                return x.item.name.CompareTo(y.item.name);
                            });
                        }
                        else if (sortBy == SortBy.amountAscending)
                        {
                            list.Sort(delegate (ItemSlot x, ItemSlot y)
                            {
                                return x.amount.CompareTo(y.amount);
                            });
                        }
                        else if (sortBy == SortBy.amountDescending)
                        {
                            list.Sort(delegate (ItemSlot x, ItemSlot y)
                            {
                                return y.amount.CompareTo(x.amount);
                            });
                        }

                        // refresh all slots
                        for (int i = 0; i < list.Count; i++)
                        {
                            UIResourceTownSlot slot = contentStorage.transform.GetChild(i).GetComponent<UIResourceTownSlot>();
                            ItemSlot itemslot = list[i];

                            //image
                            slot.image.sprite = itemslot.item.image;

                            //name by language
                            slot.textName.text = Localization.Translate(itemslot.item.name);

                            //amount
                            if (showItemsType != ShowItemsType.Army)
                            {
                                uint amountInShopStorage = player.items.GetItemShopAmount(itemslot.item.data);
                                if (amountInShopStorage == 0) slot.textAmount.text = UIUtils.LongToString(list[i].amount);
                                else slot.textAmount.text = UIUtils.LongToString(itemslot.amount) + "  +" + UIUtils.LongToString(amountInShopStorage);
                            }
                            else
                            {
                                slot.textAmount.text = UIUtils.LongToString(itemslot.amount);
                            }

                            //toggle
                            slot.toggle.gameObject.SetActive(true);
                            slot.toggle.onValueChanged.RemoveAllListeners();
                            slot.toggle.isOn = player.items.IsItemIsTracked(itemslot.item.data);
                            slot.toggle.onValueChanged.SetListener(delegate
                            {
                                _audio.PlaySoundButtonClick();

                                if (slot.toggle.isOn) player.items.itemsTracked.Add(itemslot.item.data);
                                else player.items.itemsTracked.Remove(itemslot.item.data);
                            });

                            slot.buttonDescription.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                            //UIDescriptionPanel.singleton.ShowScriptableItem(player, itemslot.item.data);
                        });
                        }
                    }
                    else
                    {
                        panelTownInfo.SetActive(false);
                        panelStorage.gameObject.SetActive(false);
                        contentManage.gameObject.SetActive(true);

                        textFreeWorkers.text = player.inhabitants.InhabitantsFree().ToString();

                        int index = player.buildings.FindIndex(player.accessToTheTown.item);
                        Building building = player.buildings.buildings[index];

                        // instantiate/destroy enough slots
                        UIUtils.BalancePrefabs(sendWorkersPrefab.gameObject, building.sendWorkers.Length, sendWorkersContent);

                        // refresh all slots
                        for (int i = 0; i < building.sendWorkers.Length; i++)
                        {
                            UISendWorkersSlot slot = sendWorkersContent.transform.GetChild(i).GetComponent<UISendWorkersSlot>();
                            slot.textName.text = building.sendWorkers[i].type.ToString();
                            slot.textInhabitantsAmount.text = building.GetSendWorkersAmountByType(building.sendWorkers[i].type).ToString();

                            int icopy = i;
                            slot.buttonMinus.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                if (building.GetSendWorkersAmountByType(building.sendWorkers[icopy].type) > 0)
                                    building.CmdDecreaseSendWorkesAmount(building.sendWorkers[icopy].type, 1);
                            });
                            slot.buttonPlus.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                if (player.inhabitants.InhabitantsFree() > 0)
                                    building.CmdIncreaseSendWorkesAmount(building.sendWorkers[icopy].type, 1);
                            });
                        }
                    }

                    buttonConfirmTownRename.onClick.SetListener(() =>
                    {
                        if (inputFieldtownRename.text.Length > 0)
                        {
                            _audio.PlaySoundButtonClick();
                            player.TownRename(inputFieldtownRename.text);
                        }
                        else
                        {
                            panelError.SetActive(true);
                            textError.text = Localization.Translate("TownNameEmpty");
                        }
                    });
                    buttonUpgrade.onClick.SetListener(() =>
                    {
                        _audio.PlaySoundButtonClick();
                        UIBuildingSelect.selectedBuilding = player.accessToTheTown.item;
                        _build.panel.SetActive(true);
                    });

                    //if (string.IsNullOrEmpty(player.townName)) panelTownRename.SetActive(true);

                    if (panelTownRename.activeSelf && !panelError.activeSelf)
                        buttonClose.gameObject.SetActive(false);
                    else buttonClose.gameObject.SetActive(true);
                }
                else panel.SetActive(false);
            }
        }

        public void TownRenameReply()
        {
            panelTownRename.SetActive(false);
            inputFieldtownRename.text = "";
        }

        public void ShowTownOptions()
        {
            showItemsType = ShowItemsType.Manage;
            panel.SetActive(true);
        }
    }
}


