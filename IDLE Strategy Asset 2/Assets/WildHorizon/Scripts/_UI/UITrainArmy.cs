using System.Collections;
using System.Collections.Generic;
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
        void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    if (UIBuildingSelect.selectedBuilding != null && UIBuildingSelect.selectedBuilding is ScriptableBuilding selectedBuilding)
                    {
                        // Find this building index in global list
                        int index = player.buildings.FindIndex(UIBuildingSelect.selectedBuilding);
                        Building building = player.buildings.buildings[index];

                        // Show texts info (workers amount)
                        long inhabitants = player.inhabitants.GetCurrent();
                        textInhabitantsFreeValue.text = inhabitants.ToString();

                        // Instantiate/destroy enough slots
                        UIUtils.BalancePrefabs(craftPrefab, building.data.trainArmy.Length, content);

                        // Refresh all slots
                        for (int i = 0; i < building.data.trainArmy.Length; i++)
                        {
                            UIProcessingSlot slot = content.transform.GetChild(i).GetComponent<UIProcessingSlot>();
                            ScriptableTrainArmyRecipe recipe = building.data.trainArmy[i];

                            // Set image
                            slot.image.sprite = recipe.result.image;

                            // Set result name
                            slot.textName.text = Localization.Translate(recipe.result.name);

                            // Set amount
                            slot.textAmount.text = player.army.GetArmyAmount(recipe.result).ToString();

                            // Instantiate/destroy enough slots for ingredients
                            UIUtils.BalancePrefabs(craftIngredientPrefab, recipe.ingredients.Length, slot.ingredientsContent);

                            // Refresh all slots
                            for (int x = 0; x < recipe.ingredients.Length; x++)
                            {
                                UIProcessingIngredientSlot ingredientSlot = slot.ingredientsContent.transform.GetChild(x).GetComponent<UIProcessingIngredientSlot>();

                                // Set image
                                ingredientSlot.image.sprite = recipe.ingredients[x].item.image;

                                // Set amount
                                ingredientSlot.textAmount.text = recipe.ingredients[x].amount.ToString();

                                // Change color if not enough ingredients
                                if (player.items.EnoughItems(new ScriptableItemAndAmount[] { recipe.ingredients[x] }))
                                    ingredientSlot.image.color = Color.white;
                                else
                                    ingredientSlot.image.color = Color.red;
                            }

                            // Button onClick behavior
                            int recipeIndex = building.data.GetTrainArmyIndex(recipe);
                            slot.button.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();

                                // Check if enabled
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
                                        // Show error panel if researches are not done
                                        string errorMessage = Localization.languageCurrent == SystemLanguage.English
                                            ? "Need to get knowledge"
                                            : "Íåîáõîäèìî ïîëó÷èòü çíàíèå";
                                        UIError.singleton.ShowListScriptableResearchAndLevel(errorMessage, recipe.researches, "");
                                    }
                                    else if (inhabitants >= recipe.ingredients[0].amount &&
                                             player.items.EnoughItems(recipe.ingredients))
                                    {
                                        // Start training
                                        Building temp = building;
                                        temp.trainArmyEnable[recipeIndex] = true;

                                        float bonusTimeForManager = recipe.time * player.heroes.BonusForManager(temp.data.name, HeroBonusType.craftingSpeed);
                                        temp.lastTimeTrainArmy[recipeIndex] = recipe.time - bonusTimeForManager;

                                        building = temp;

                                        // Stop training after one cycle
                                        StartCoroutine(StopTrainingAfterCycle(recipeIndex, recipe.time - bonusTimeForManager, temp));
                                    }
                                    else
                                    {
                                        // Show error if not enough inhabitants or ingredients
                                        string errorMessage = Localization.languageCurrent == SystemLanguage.English
                                            ? "Not enough resources"
                                            : "Íåõâàòêà ðåñóðñîâ";
                                        UIError.singleton.ShowListScriptableItemAndAmount(errorMessage, recipe.ingredients, "");
                                    }
                                }
                            });


                            // Show panel lock if required researches are not done
                            slot.panelLock.SetActive(!player.researches.CheckRequiredResearcheslist(recipe.researches));

                            // Show panel time if training is enabled
                            slot.panelTime.SetActive(building.trainArmyEnable[recipeIndex]);
                            slot.textTime.text = building.trainArmyEnable[recipeIndex]
                                ? Utils.PrettySeconds((int)building.lastTimeTrainArmy[recipeIndex])
                                : "";

                            // Update state image color
                            slot.imageState.color = building.trainArmyEnable[recipeIndex] ? colorEnabled : colorDisabled;
                        }
                    }
                    else panel.SetActive(false);
                }
                else panel.SetActive(false);
            }
        }
        private IEnumerator StopTrainingAfterCycle(int recipeIndex, float cooldownTime, Building building)
        {
            yield return new WaitForSeconds(cooldownTime);

            Building temp = building;
            temp.trainArmyEnable[recipeIndex] = false; // Disable training
            building = temp;
        }

    }
}
