using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIBuild : MonoBehaviour
    {
        public GameObject panel;
        public Text text_BuildUpgrade_NameValue;
        public Image image_BuildUpgrade;

        public Text text_LevelValue;
        public Text text_TimeValue;
        public Text text_Capacity;
        public Text text_CapacityValue;

        public TextMeshProUGUI textTimeValue;

        [Header("Required Ingredients")]
        public Transform contentIngredients;
        public UIIngredientResourceSlot prefabIngredient;

        [Header("Required Researches")]
        public Transform contentResearches;
        public UIIngredientResearchSlot prefabResearch;

        [Header("Required Buildings")]
        public Transform contentBuildings;
        public UIIngredientBuildingSlot prefabBuilding;

        [Header("Temporarily Inactive")]
        public GameObject panelTemporarilyInactive;
        public TextMeshProUGUI textTemporarilyInactive;
        [SerializeField] private LocalizeText[] descriptionTemporarilyInactiveLocalize;
        public string GetDescriptionByLanguage(SystemLanguage lang)
        {
            for (int i = 0; i < descriptionTemporarilyInactiveLocalize.Length; i++)
            {
                if (descriptionTemporarilyInactiveLocalize[i].language == lang) return descriptionTemporarilyInactiveLocalize[i].description;
            }
            return null;
        }

        [Header("Buttons")]
        public GameObject panelAds;
        public Button buttonADS;
        public Button buttonBuild;
        public Button buttonBuildForShop;
        public Button buttonManage;
        public Button buttonNext;

        [Header("UI Elements : Error")]
        public GameObject panelError;
        public Text textError;

        [Header("Colors")]
        public Color colorNormal;
        public Color colorMissing;

        [Header("Components")]
        public UIAudio _audio;
        public UIBuildingSelect panelSelect;
        public UIBuildingManagement _management;
        public UIMiningItemsInBuilding _mining;
        public UIResearchDescription _researches;

        public static event Action UpdateBuildingSprite = () => { };

        public static UIBuild singleton;
        public UIBuild()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            if (singleton == null) singleton = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    if (UIBuildingSelect.selectedBuilding != null)
                    {
                        ScriptableBuilding selectedBuilding = UIBuildingSelect.selectedBuilding;

                        //building name by language
                        text_BuildUpgrade_NameValue.text = Localization.Translate(selectedBuilding.name);

                        //find this building index in Global list
                        int index = player.buildings.FindIndex(selectedBuilding);
                        Building building = player.buildings.buildings[index];

                        //sprite
                        if (building.level == building.data.maxLevel) image_BuildUpgrade.sprite = selectedBuilding.spritesByLevel[building.level - 1];
                        else image_BuildUpgrade.sprite = selectedBuilding.spritesByLevel[building.level];

                        //current level
                        if (building.level > 0)
                        {
                            if (building.underConstruction == false)
                            {
                                if (building.underConstruction == false && building.level < building.data.maxLevel) text_LevelValue.text = building.level + " -> " + (building.level + 1);
                                else text_LevelValue.text = building.data.maxLevel.ToString();
                            }
                            else text_LevelValue.text = Localization.Translate("Under construction");
                        }
                        else text_LevelValue.text = "1";

                        if (building.level < building.data.maxLevel) text_TimeValue.text = Utils.PrettySeconds((int)player.buildings.ConstructionTime(building));
                        else text_TimeValue.text = building.level + ":" + Localization.Translate("Max Level");

                        textTimeValue.text = Mathf.Round(building.time) > 0 ? Utils.PrettySeconds((int)building.time) : "";

                        //capacity
                        if (selectedBuilding is ScriptableMiningBuilding miningBuilding)
                        {
                            text_Capacity.gameObject.SetActive(true);

                            if (building.level < selectedBuilding.spritesByLevel.Length - 1 && building.level > 0)
                                text_CapacityValue.text = UIUtils.LongToString(miningBuilding.internalStorage.Get(building.level - 1)) + " -> " + UIUtils.LongToString(miningBuilding.internalStorage.Get(building.level));
                            else
                                text_CapacityValue.text = UIUtils.LongToString(miningBuilding.internalStorage.Get(building.level));
                        }
                        else
                        {
                            text_Capacity.gameObject.SetActive(false);
                            text_CapacityValue.text = "";
                        }

                        //ingredients
                        List<ScriptableItemAndAmount> list = player.items.IngredientsList(selectedBuilding.ingredients, building.level);

                        if (selectedBuilding.workersNeed.Get(building.level + 1) > 0)
                        {
                            ScriptableItemAndAmount resource = new ScriptableItemAndAmount();
                            resource.item = player.inhabitants.scriptableItem;
                            resource.amount = selectedBuilding.workersNeed.Get(building.level + 1);

                            list.Add(resource);
                        }
                        contentIngredients.gameObject.SetActive(list.Count > 0);

                        // instantiate/destroy enough slots for RequiredIngredients
                        UIUtils.BalancePrefabs(prefabIngredient.gameObject, list.Count + 1, contentIngredients);
                        for (int i = 0; i < list.Count; i++)
                        {
                            UIIngredientResourceSlot slot = contentIngredients.transform.GetChild(i + 1).GetComponent<UIIngredientResourceSlot>();

                            slot.image.sprite = list[i].item.image;

                            //building name by language
                            slot.textName.text = Localization.Translate(list[i].item.name);

                            uint amount = 0;
                            uint shopAmount = 0;
                            if (list[i].item.Equals(player.inhabitants.scriptableItem))
                            {
                                amount = player.inhabitants.InhabitantsFree();
                                shopAmount = player.inhabitants.GetReserve();
                            }
                            else
                            {
                                amount = player.items.GetItemAmount(list[i].item);
                                shopAmount = player.items.GetItemShopAmount(list[i].item);
                            }

                            slot.textAmount.text = UIUtils.LongToString(list[i].amount) + " / " + UIUtils.LongToString(amount + shopAmount);

                            if (amount + shopAmount < list[i].amount) slot.textAmount.color = colorMissing;
                            else slot.textAmount.color = colorNormal;
                        }

                        //researches
                        List<ScriptableResearchAndLevel> researches = player.researches.requiredResearches(selectedBuilding.requiredResearches, building.level);
                        contentResearches.gameObject.SetActive(researches.Count > 0);
                        // instantiate/destroy enough slots for RequiredResearches
                        UIUtils.BalancePrefabs(prefabResearch.gameObject, researches.Count + 1, contentResearches);
                        for (int i = 0; i < researches.Count; i++)
                        {
                            UIIngredientResearchSlot slot = contentResearches.transform.GetChild(i + 1).GetComponent<UIIngredientResearchSlot>();

                            slot.image.sprite = researches[i].item.sprite;

                            //name
                            slot.textName.text = Localization.Translate(researches[i].item.name);
                            slot.textAmount.text = Localization.Translate("Level") + " " + researches[i].researachLevel;

                            //check this research in global list
                            int ind = player.researches.FindIndex(researches[i].item);
                            if (ind == -1 || player.researches.researches[ind].level < researches[i].researachLevel) slot.textAmount.color = colorMissing;
                            else slot.textAmount.color = colorNormal;

                            int icopy = i;
                            slot.button.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                UIResearches.selectedResearch = researches[icopy].item;
                                _researches.panel.SetActive(true);
                            });
                        }

                        //buildings
                        List<ScriptableBuildingAndAmountOrLevel> buildings = player.buildings.requiredBuildings(selectedBuilding.requiredBuildings, building.level);
                        contentBuildings.gameObject.SetActive(buildings.Count > 0);
                        // instantiate/destroy enough slots for RequiredBuildings
                        UIUtils.BalancePrefabs(prefabBuilding.gameObject, buildings.Count + 1, contentBuildings);
                        for (int i = 0; i < buildings.Count; i++)
                        {
                            UIIngredientBuildingSlot slot = contentBuildings.transform.GetChild(i + 1).GetComponent<UIIngredientBuildingSlot>();

                            slot.image.sprite = buildings[i].item.spriteForPreview[0];

                            //building name
                            slot.textName.text = Localization.Translate(buildings[i].item.name);
                            slot.textAmount.text = Localization.Translate("Level") + " " + buildings[i].requiredBuildingLevel;

                            //check this building in global list
                            int required = player.buildings.FindIndex(buildings[i].item);
                            if (player.buildings.buildings[required].level < buildings[i].requiredBuildingLevel)
                            {
                                slot.textAmount.color = colorMissing;
                                slot.button.interactable = true;
                            }
                            else
                            {
                                slot.button.interactable = false;
                                slot.textAmount.color = colorNormal;
                            }

                            int icopy = i;

                            slot.button.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                UIBuildingSelect.selectedBuilding = buildings[icopy].item;
                            });
                        }

                        //Temporarily Inactive
                        panelTemporarilyInactive.SetActive(building.data.temporarilyInactive);
                        //textTemporarilyInactive.text = GetDescriptionByLanguage(Localization.languageCurrent);

                        //if Building has already been build but could be improved
                        if (building.level > 0)
                        {
                            //if current
                            if (building.level < building.data.maxLevel)
                                buttonBuild.gameObject.GetComponentInChildren<Text>().text = Localization.Translate("Upgrade");

                            else if (building.level == building.data.maxLevel)
                                buttonBuild.gameObject.GetComponentInChildren<Text>().text = Localization.Translate("MaxLevel");
                        }
                        else buttonBuild.gameObject.GetComponentInChildren<Text>().text = Localization.Translate("Build");

                        //button build
                        buttonBuild.gameObject.SetActive(building.level < building.data.maxLevel);
                        buttonBuild.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();

                        //
                        if (building.underConstruction == false)
                            {
                            //check inhabitants
                            if (player.inhabitants.InhabitantsFree() >= selectedBuilding.workersNeed.Get(building.level))
                                {
                                //check ingredients
                                if (player.items.EnoughItems(selectedBuilding.ingredients, building.level))
                                    {
                                    //check researches
                                    if (player.researches.CheckRequiredResearches(selectedBuilding.requiredResearches, building.level))
                                        {
                                        //check buildings
                                        if (player.buildings.CheckRequiredBuildings(selectedBuilding.requiredBuildings, building.level))
                                            {
                                                player.buildings.CmdStartBuilding(UIBuildingSelect.selectedBuilding.name, false);

                                            //move camera
                                            if (building.level == 0) MoveCamera();
                                            }
                                            else
                                            {
                                                panelError.SetActive(true);
                                                textError.text = "not all necessary buildings have been built";
                                            }
                                        }
                                        else
                                        {
                                            panelError.SetActive(true);
                                            textError.text = "not all necessary studies have been studied";
                                        }
                                    }
                                    else
                                    {
                                        panelError.SetActive(true);
                                        textError.text = "Not Enought Ingredients";
                                    }
                                }
                                else
                                {
                                    panelError.SetActive(true);
                                    textError.text = "Not Enought Inhabitants";
                                }
                            }
                            else
                            {
                                panelError.SetActive(true);
                                textError.text = "First you need to complete the current improvement";
                            }
                        });

                        //button build for coins
                        buttonBuildForShop.gameObject.SetActive(selectedBuilding.coins.Get(building.level) > 0 && (building.level < building.data.maxLevel));
                        buttonBuildForShop.GetComponentInChildren<Text>().text = selectedBuilding.coins.Get(building.level).ToString();
                        buttonBuildForShop.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();

                            if (UIBuildingSelect.selectedBuilding.coins.Get(building.level) <= player.items.GetItemAmount(player.coinsItem))
                            {
                                if (player.researches.CheckRequiredResearches(selectedBuilding.requiredResearches, building.level))
                                {
                                    if (player.buildings.CheckRequiredBuildings(selectedBuilding.requiredBuildings, building.level))
                                    {
                                        player.buildings.CmdStartBuilding(UIBuildingSelect.selectedBuilding.name, true);

                                    //move camera
                                    if (building.level == 0) MoveCamera();
                                    }
                                    else
                                    {
                                        UIError.singleton.ShowListScriptableBuildingAndAmountOrLevel("Buildings to be built", selectedBuilding.requiredBuildings, "");
                                    }
                                }
                                else
                                {
                                    UIError.singleton.ShowListScriptableResearchAndLevel("Need to get knowledges", selectedBuilding.requiredResearches, "");
                                }
                            }
                            else
                            {
                                ScriptableItemAndAmount temp = new ScriptableItemAndAmount();
                                temp.item = player.coinsItem;
                                temp.amount = UIBuildingSelect.selectedBuilding.coins.Get(building.level);
                                UIError.singleton.ShowScriptableItemAndAmount("Not enough Coins", temp, "");
                            }
                        });

                        //button "Manage"
                        buttonManage.gameObject.SetActive(index != -1 && building.data.IsManaged() && building.level >= 1);
                        buttonManage.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            if (building.data.Equals(player.accessToTheTown.item) == false)
                            {
                                panel.SetActive(false);
                                _management.panel.SetActive(true);
                            }
                            else
                            {
                                panel.SetActive(false);
                                UITown.singleton.panel.SetActive(true);
                            }
                        });

                        //ads
                        /*panelAds.SetActive(
                            Advertisement.IsReady(GameADS.singleton.rewardedVideo) &&
                            building.underConstruction &&
                            building.time > 5 &&
                            building.adsAmount > 0);
                        buttonADS.onClick.SetListener(() =>
                        {
                            buttonADS.gameObject.SetActive(false);
                            _audio.PlaySoundButtonClick();
                        });*/
                    }
                    else panel.SetActive(false);
                }
                else panel.SetActive(false);
            }
        }

        public void OpenBuildingManager(ScriptableBuilding building)
        {
            UIBuildingSelect.selectedBuilding = building;
            _management.panel.SetActive(true);
        }

        private void MoveCamera()
        {
            panel.SetActive(false);

            int indexOnScene = panelSelect.BuildingIndexOnScene(UIBuildingSelect.selectedBuilding);
            Debug.Log("Index on scene for move camera " + indexOnScene);
            if (indexOnScene != -1)
            {
                Vector3 pos = new Vector3(panelSelect.buildingsOnScene[indexOnScene].gameObject.transform.position.x,
                    panelSelect.buildingsOnScene[indexOnScene].gameObject.transform.position.y,
                    Camera.main.transform.position.z);
                Camera.main.transform.position = pos;
            }
        }

        public void ShowBuildingFromOtherPanels()
        {
            panel.SetActive(true);
        }
    }
}


