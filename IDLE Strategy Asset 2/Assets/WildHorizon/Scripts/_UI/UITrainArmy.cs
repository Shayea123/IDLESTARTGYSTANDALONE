using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UITrainArmy : MonoBehaviour
    {
        public GameObject panel;
        public Text textInhabitantsFree;
        public Text textInhabitantsFreeValue;
        public Transform content;
        public GameObject craftPrefab;
        public GameObject craftIngredientPrefab;
        public Color colorEnabled;
        public Color colorDisabled;

        [Header("Components")]
        public UIAudio _audio;

        // Update is called once per frame
        /*void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    //find this building index in Global list
                    int index = player.buildings.FindIndex(UIBuildingSelect.selectedBuilding);
                    Building building = player.buildings.buildings[index];

                    //translate texts
                    textInhabitantsFree.font = Localization.fontClassic;

                    //show texts info (workers amount)
                    long inhabitants = player.inhabitants.InhabitantsCurrent();
                    textInhabitantsFreeValue.text = inhabitants.ToString();

                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(craftPrefab, building.data.trainArmy.Length, content);

                    //refresh all slots
                    for (int i = 0; i < building.data.trainArmy.Length; i++)
                    {
                        UIProcessingSlot slot = content.transform.GetChild(i).GetComponent<UIProcessingSlot>();
                        ScriptableTrainArmyRecipe recipe = building.data.trainArmy[i];

                        //image
                        slot.image.sprite = recipe.result.image;

                        //result name
                        slot.textName.text = Localization.Translate(recipe.result.name);
                        slot.textName.font = Localization.fontTMP;

                        //amount
                        slot.textAmount.text = player.army.GetArmyAmount(recipe.result).ToString();

                        // instantiate/destroy enough slots for ingredients
                        UIUtils.BalancePrefabs(craftIngredientPrefab, recipe.ingredients.Count, slot.ingredientsContent);

                        //refresh all slots
                        for (int x = 0; x < recipe.ingredients.Count; x++)
                        {
                            UIProcessingIngredientSlot ingredientSlot = slot.ingredientsContent.transform.GetChild(x).GetComponent<UIProcessingIngredientSlot>();

                            //image
                            ingredientSlot.image.sprite = recipe.ingredients[x].item.image;

                            //amount
                            ingredientSlot.textAmount.text = recipe.ingredients[x].amount.ToString();

                            //chage color if not enough ingredient 
                            if (player.items.EnoughItemAmount(recipe.ingredients[x])) ingredientSlot.image.color = Color.white;
                            else ingredientSlot.image.color = Color.red;
                        }

                        int icopy = i;
                        int recipeIndex = building.data.GetTrainArmyIndex(recipe);
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();

                            //if enabled
                            if (building.trainArmyEnable[recipeIndex])
                            {
                                Building temp = building;
                                temp.trainArmyEnable[recipeIndex] = false;
                                building = temp;
                            }
                            else
                            {
                                if (!player.researches.CheckRequiredResearcheslist(recipe.researches))
                                {
                                    //show error panel
                                    if (Localization.languageCurrent == SystemLanguage.English)
                                    {
                                        if (recipe.researches.Count == 1)
                                            UIError.singleton.ShowListScriptableResearchAndLevel("Need to get knowledge", recipe.researches, "");
                                        else
                                            UIError.singleton.ShowListScriptableResearchAndLevel("Need to get knowledges", recipe.researches, "");
                                    }
                                    else if (Localization.languageCurrent == SystemLanguage.Russian)
                                    {
                                        if (recipe.researches.Count == 1)
                                            UIError.singleton.ShowListScriptableResearchAndLevel("Необходимо получить знание", recipe.researches, "");
                                        else
                                            UIError.singleton.ShowListScriptableResearchAndLevel("Необходимо получить знания", recipe.researches, "");
                                    }
                                }
                                else
                                {
                                    //check free inhabitants
                                    if (inhabitants >= recipe.ingredients[0].amount)
                                    {
                                        //check ingredients amount
                                        if (player.items.EnoughItems(recipe.ingredients))
                                        {
                                            Building temp = building;
                                            temp.trainArmyEnable[recipeIndex] = true;

                                            float bonusTimeForManager = recipe.time * player.heroes.BonusForManager(temp.data, HeroBonusType.craftingSpeed);
                                            temp.lastTimeTrainArmy[recipeIndex] = recipe.time - bonusTimeForManager;

                                            building = temp;
                                        }
                                        else
                                        {
                                            //show error panel
                                            if (Localization.languageCurrent == SystemLanguage.English)
                                                UIError.singleton.ShowListScriptableItemAndAmount(player, "Not enough Ingredients", recipe.ingredients, "");
                                            else if (Localization.languageCurrent == SystemLanguage.Russian)
                                                UIError.singleton.ShowListScriptableItemAndAmount(player, "Не достаточно ресурсов", recipe.ingredients, "");
                                        }
                                    }
                                    else
                                    {
                                        //show error panel
                                        if (Localization.languageCurrent == SystemLanguage.English)
                                            UIError.singleton.ShowScriptableItemAndAmount(player, "Not enough workers", recipe.ingredients[0], "");
                                        else if (Localization.languageCurrent == SystemLanguage.Russian)
                                            UIError.singleton.ShowScriptableItemAndAmount(player, "Не достаточно рабочих", recipe.ingredients[0], "");
                                    }
                                }
                            }
                        });

                        //show panel lock if required researches is not done
                        slot.panelLock.SetActive(!player.researches.CheckRequiredResearcheslist(recipe.researches));

                        slot.panelTime.SetActive(building.trainArmyEnable[recipeIndex]);
                        slot.textTime.text = building.trainArmyEnable[recipeIndex] ? Utils.PrettySeconds((int)building.lastTimeTrainArmy[recipeIndex]) : "";

                        slot.imageState.color = building.trainArmyEnable[recipeIndex] ? colorEnabled : colorDisabled;
                    }
                }

            }
        }*/
    }
}


