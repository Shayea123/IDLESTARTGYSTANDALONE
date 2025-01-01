using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UICraft : MonoBehaviour
    {
        public GameObject panel;
        public GameObject buttonTypePrefab;
        public Transform contentForButtons;

        public Text textWorkersFree;
        public Text textWorkersUsed;
        public Text textWorkersFreeValue;
        public Text textWorkersUsedValue;
        public Transform content;
        public GameObject craftPrefab;
        public GameObject craftIngredientPrefab;
        public Color colorEnabled;
        public Color colorDisabled;
        RecipeType recipeType;

        [Header("Components")]
        public UIAudio _audio;

        public static UICraft singleton;
        public UICraft()
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
                    if (UIBuildingSelect.selectedBuilding != null && UIBuildingSelect.selectedBuilding is ScriptableProductionBuilding productionBuilding)
                    {
                        //find this building index in Global list
                        int index = player.buildings.FindIndex(UIBuildingSelect.selectedBuilding);
                        Building building = player.buildings.buildings[index];

                        // instantiate/destroy enough buttons slots
                        if (productionBuilding.recipeTypes.Length > 1)
                        {
                            UIUtils.BalancePrefabs(buttonTypePrefab, productionBuilding.recipeTypes.Length, contentForButtons);

                            //refresh all slots
                            for (int i = 0; i < productionBuilding.recipeTypes.Length; i++)
                            {
                                int icopy = i;
                                Button bt = contentForButtons.transform.GetChild(i).GetComponent<Button>();
                                bt.gameObject.GetComponentInChildren<Text>().text = Localization.Translate(productionBuilding.recipeTypes[i].ToString());

                                bt.onClick.SetListener(() =>
                                {
                                    _audio.PlaySoundButtonClick();
                                    recipeType = productionBuilding.recipeTypes[icopy];
                                });
                            }

                            if (!productionBuilding.CheckCraftRecipeTypes(recipeType)) recipeType = productionBuilding.recipeTypes[0];
                        }
                        else
                        {
                            UIUtils.BalancePrefabs(buttonTypePrefab, 0, contentForButtons);
                            recipeType = productionBuilding.recipeTypes[0];
                        }

                        //show texts info (workers amount)
                        textWorkersFreeValue.text = player.inhabitants.InhabitantsFree().ToString();
                        textWorkersUsedValue.text = building.InhabitantsInBuildingCraft().ToString();

                        List<ScriptableRecipe> list = player.craft.RecipesByType(productionBuilding.craftingRecipes, recipeType);

                        // instantiate/destroy enough slots
                        UIUtils.BalancePrefabs(craftPrefab, list.Count, content);

                        //refresh all slots
                        for (int i = 0; i < list.Count; i++)
                        {
                            UIProcessingSlot slot = content.transform.GetChild(i).GetComponent<UIProcessingSlot>();

                            //image
                            slot.image.sprite = list[i].resultItem.image;

                            //result Item name                
                            slot.textName.text = Localization.Translate(list[i].resultItem.name);

                            //amount
                            slot.textAmount.text = player.items.GetItemAmount(list[i].resultItem).ToString();

                            // instantiate/destroy enough slots for ingredients
                            UIUtils.BalancePrefabs(craftIngredientPrefab, list[i].ingredients.Length + 1, slot.ingredientsContent);

                            //inhabitants
                            UIProcessingIngredientSlot inhabitantsSlot = slot.ingredientsContent.transform.GetChild(0).GetComponent<UIProcessingIngredientSlot>();
                            inhabitantsSlot.image.sprite = player.inhabitants.scriptableItem.image;
                            inhabitantsSlot.textAmount.text = list[i].inhabitants.ToString();

                            //refresh all slots
                            for (int x = 0; x < list[i].ingredients.Length; x++)
                            {
                                UIProcessingIngredientSlot ingredientSlot = slot.ingredientsContent.transform.GetChild(x + 1).GetComponent<UIProcessingIngredientSlot>();

                                //image
                                ingredientSlot.image.sprite = list[i].ingredients[x].item.image;

                                //chage color if not enough ingredient 
                                if ((player.items.GetItemAmount(list[i].ingredients[x].item) + player.items.GetItemShopAmount(list[i].ingredients[x].item) > list[i].ingredients[x].amount)) ingredientSlot.image.color = Color.white;
                                else ingredientSlot.image.color = Color.red;

                                //amount
                                ingredientSlot.textAmount.text = list[i].ingredients[x].amount.ToString();
                            }

                            //show panel lock if required researches is not done
                            slot.panelLock.SetActive(!player.researches.CheckRequiredResearcheslist(list[i].researches));

                            int icopy = i;
                            slot.button.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                player.craft.CmdChangeStateForCraftRecipe(building.data.name, list[icopy].name);
                            });

                            int recipeIndex = productionBuilding.GetCraftRecipeIndex(list[i]);
                            slot.panelTime.SetActive(building.craftEnable[recipeIndex]);
                            if (building.craftEnable[recipeIndex]) Debug.Log(recipeIndex + " / " + list[i].name + " / " + building.lastTimeCraft[recipeIndex]);
                            slot.textTime.text = building.craftEnable[recipeIndex] ? Utils.PrettySeconds((int)building.lastTimeCraft[recipeIndex]) : "";

                            //slot.imageState.color = building.craftEnable[recipeIndex] ? colorEnabled : colorDisabled;
                        }
                    }
                    else panel.SetActive(false);
                }
                else panel.SetActive(false);
            }
        }
    }
}


