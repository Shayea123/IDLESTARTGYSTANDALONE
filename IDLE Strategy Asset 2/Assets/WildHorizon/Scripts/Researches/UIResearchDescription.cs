using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIResearchDescription : MonoBehaviour
    {
        public GameObject panel;
        public Image imageInfo;
        public Text textResearchName;
        public Text textDescription;
        public GameObject panelWorkers;
        public Text textWorkersValue;
        public GameObject panelTime;
        public Text textBuildTimeValue;

        [Header("Required Ingredients")]
        public GameObject panelIngredients;
        public Transform contentIngredients;
        public UIIngredientResourceSlot pefabIngredient;

        [Header("Required Buildings")]
        public GameObject panelBuildings;
        public Transform contentBuildings;
        public GameObject pefabBuilding;

        [Header("Required Researches")]
        public GameObject panelResearches;
        public Transform contentResearches;
        public GameObject pefabResearches;

        [Header("Buttons")]
        public Button buttonResearchShop;
        public Button buttonResearch;
        public Button buttonADS;

        [Header("Colors")]
        public Color colorNormal;
        public Color colorEmpty;

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
                    ScriptableResearch selectedResearch = UIResearches.selectedResearch;

                    if (selectedResearch != null)
                    {
                        int index = player.researches.FindIndex(selectedResearch);
                        Research research = player.researches.researches[index];

                        //sprite
                        imageInfo.sprite = selectedResearch.sprite;

                        //name
                        textResearchName.text = Localization.Translate(selectedResearch.name);

                        //description
                        textDescription.text = selectedResearch.GetDescriptionByLanguage(Localization.languageCurrent);

                        //time
                        //textTimeValue.text = Utils.PrettySeconds((int)selectedResearch.researchTime.Get(player.researches.researches[index].level));
                        if (research.level < research.data.levels.Length)
                        {
                            if (research.underStudy)
                            {
                                textBuildTimeValue.text = Mathf.Round(research.time) > 0 ? Utils.PrettySeconds((int)research.time) : "";

                                //workers
                                panelWorkers.SetActive(true);
                                textWorkersValue.text = selectedResearch.levels[research.level].workersNeed.ToString();
                                textWorkersValue.color = Color.black;
                            }
                            else
                            {
                                panelTime.SetActive(true);
                                textBuildTimeValue.text = Utils.PrettySeconds((int)research.data.levels[research.level].researchTime);

                                //workers
                                panelWorkers.SetActive(true);
                                textWorkersValue.text = selectedResearch.levels[research.level].workersNeed.ToString();
                                textWorkersValue.color = player.inhabitants.InhabitantsFree() < selectedResearch.levels[research.level].workersNeed ? Color.red : Color.black;
                            }

                            //ingredients
                            panelIngredients.SetActive(selectedResearch.levels[research.level].ingredients.Length > 0);
                            // instantiate/destroy enough slots for RequiredIngredients
                            UIUtils.BalancePrefabs(pefabIngredient.gameObject, selectedResearch.levels[research.level].ingredients.Length, contentIngredients);
                            for (int i = 0; i < selectedResearch.levels[research.level].ingredients.Length; i++)
                            {
                                UIIngredientResourceSlot slot = contentIngredients.transform.GetChild(i).GetComponent<UIIngredientResourceSlot>();

                                slot.image.sprite = selectedResearch.levels[research.level].ingredients[i].item.image;

                                //name
                                slot.textName.text = Localization.Translate(selectedResearch.levels[research.level].ingredients[i].item.name);

                                uint amount = player.items.GetItemAmount(selectedResearch.levels[research.level].ingredients[i].item);
                                uint shopAmount = player.items.GetItemShopAmount(selectedResearch.levels[research.level].ingredients[i].item);

                                slot.textAmount.text = UIUtils.LongToString(selectedResearch.levels[research.level].ingredients[i].amount) + " / " + UIUtils.LongToString(amount + shopAmount);

                                if (amount + shopAmount < selectedResearch.levels[research.level].ingredients[i].amount) slot.textAmount.color = colorEmpty;
                                else slot.textAmount.color = colorNormal;
                            }

                            //researches
                            panelResearches.SetActive(selectedResearch.levels[research.level].requiredResearches.Length > 0);
                            // instantiate/destroy enough slots for RequiredResearches
                            UIUtils.BalancePrefabs(pefabResearches.gameObject, selectedResearch.levels[research.level].requiredResearches.Length, contentResearches);
                            for (int i = 0; i < selectedResearch.levels[research.level].requiredResearches.Length; i++)
                            {
                                UIIngredientResearchSlot slot = contentResearches.transform.GetChild(i).GetComponent<UIIngredientResearchSlot>();

                                slot.image.sprite = selectedResearch.levels[research.level].requiredResearches[i].item.sprite;

                                //name
                                slot.textName.text = Localization.Translate(selectedResearch.levels[research.level].requiredResearches[i].item.name);
                                slot.textAmount.text = Localization.Translate("Level ") + " " + selectedResearch.levels[research.level].requiredResearches[i].requiredLevel;

                                //check this building in global list
                                int ind = player.researches.researches.FindIndex(x => x.name == selectedResearch.levels[research.level].requiredResearches[i].item.name);
                                if (ind == -1 || player.researches.researches[ind].level < selectedResearch.levels[research.level].requiredResearches[i].requiredLevel) slot.textAmount.color = colorEmpty;
                                else slot.textAmount.color = colorNormal;

                                int icopy = i;
                                slot.button.onClick.SetListener(() =>
                                {
                                    _audio.PlaySoundButtonClick();
                                    UIResearches.selectedResearch = selectedResearch.levels[research.level].requiredResearches[icopy].item;
                                });
                            }

                            //buildings
                            panelBuildings.SetActive(selectedResearch.levels[research.level].requiredBuildings.Length > 0);
                            // instantiate/destroy enough slots for RequiredBuildings
                            UIUtils.BalancePrefabs(pefabBuilding.gameObject, selectedResearch.levels[research.level].requiredBuildings.Length, contentBuildings);
                            for (int i = 0; i < selectedResearch.levels[research.level].requiredBuildings.Length; i++)
                            {
                                UIIngredientBuildingSlot slot = contentBuildings.transform.GetChild(i).GetComponent<UIIngredientBuildingSlot>();

                                slot.image.sprite = selectedResearch.levels[research.level].requiredBuildings[i].item.spriteForPreview[0];
                                slot.textName.text = Localization.Translate(selectedResearch.levels[research.level].requiredBuildings[i].item.name);
                                slot.textAmount.text = Localization.Translate("Level") + " " + selectedResearch.levels[research.level].requiredBuildings[i].requiredLevel.ToString();

                                //check this building in global list
                                int required = player.buildings.FindIndex(selectedResearch.levels[research.level].requiredBuildings[i].item);
                                if (player.buildings.buildings[required].level < selectedResearch.levels[research.level].requiredBuildings[i].requiredLevel)
                                {
                                    slot.textAmount.color = colorEmpty;
                                    slot.button.interactable = true;
                                }
                                else
                                {
                                    slot.textAmount.color = colorNormal;
                                    slot.button.interactable = false;
                                }

                                int icopy = i;
                                slot.button.onClick.SetListener(() =>
                                {
                                    _audio.PlaySoundButtonClick();
                                    UIBuildingSelect.selectedBuilding = selectedResearch.levels[research.level].requiredBuildings[icopy].item;
                                });
                            }

                            buttonResearch.gameObject.SetActive(selectedResearch.levels[research.level].coins > 0 && research.level < research.data.levels.Length);
                            buttonResearch.onClick.SetListener(() =>
                            {
                            //is study now ?
                            if (!research.underStudy)
                                {
                                //check inhabitants
                                if (player.inhabitants.InhabitantsFree() >= selectedResearch.levels[research.level].workersNeed)
                                    {
                                    //check ingredients
                                    if (player.items.EnoughItems(selectedResearch.levels[research.level].ingredients))
                                        {
                                        //check buildings
                                        if (player.buildings.CheckRequiredBuildings(selectedResearch.levels[research.level].requiredBuildings))
                                            {
                                            //check research
                                            if (player.researches.CheckRequiredResearches(selectedResearch.levels[research.level].requiredResearches))
                                                {
                                                    _audio.PlaySoundButtonClick();
                                                    player.researches.CmdStartResearching(selectedResearch.name, false);
                                                    panel.SetActive(false);
                                                    panelResearches.SetActive(true);
                                                }
                                                else
                                                {

                                                }
                                            }
                                            else
                                            {

                                            }
                                        }
                                        else
                                        {
                                            UIError.singleton.ShowTextError("Not enough Ingredients");
                                        }
                                    }
                                    else
                                    {
                                        UIError.singleton.ShowTextError("Not enough Inhabitants");
                                    }
                                }
                                else
                                {
                                    UIError.singleton.ShowTextError("You must first complete the current study");
                                }
                            });

                            buttonResearchShop.gameObject.SetActive(selectedResearch.levels[research.level].coins > 0 && research.level < research.data.levels.Length);
                            buttonResearchShop.GetComponentInChildren<Text>().text = selectedResearch.levels[research.level].coins.ToString();
                            buttonResearchShop.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();

                            //is study now ?
                            if (!research.underStudy)
                                {
                                    if (selectedResearch.levels[research.level].coins <= player.items.GetItemAmount(player.coinsItem))
                                    {
                                        player.researches.CmdStartResearching(selectedResearch.name, true);

                                    //add new research or upgarde level
                                    panel.SetActive(false);
                                        panelResearches.SetActive(true);
                                    }
                                    else
                                    {
                                        ScriptableItemAndAmount temp = new ScriptableItemAndAmount();
                                        temp.item = player.coinsItem;
                                        temp.amount = selectedResearch.levels[research.level].coins;
                                        UIError.singleton.ShowScriptableItemAndAmount("Not enough Coins", temp, "");
                                    }
                                }
                                else
                                {
                                    UIError.singleton.ShowTextError("You must first complete the current study");
                                }
                            });

                            buttonADS.gameObject.SetActive(
                                //Advertisement.IsReady(GameADS.singleton.rewardedVideo) &&
                                research.time > 0 &&
                                selectedResearch.levels[research.level].decreaseTimeForAds &&
                                research.adsAmount > 0);
                            buttonADS.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();

                            });
                        }
                        else
                        {
                            panelWorkers.SetActive(false);
                            panelTime.SetActive(false);

                            buttonResearch.gameObject.SetActive(false);
                            buttonResearchShop.gameObject.SetActive(false);
                            buttonADS.gameObject.SetActive(false);

                            UIUtils.BalancePrefabs(pefabBuilding.gameObject, 0, contentIngredients);
                            UIUtils.BalancePrefabs(pefabBuilding.gameObject, 0, contentBuildings);
                            UIUtils.BalancePrefabs(pefabBuilding.gameObject, 0, contentResearches);
                        }
                    }
                    else panel.SetActive(false);
                }
            }
        }
    }
}


