using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum CraftInfoMessageType { researches, ingredients, workers, storage }

    public class PlayerCraft : MonoBehaviour
    {
        [Range(0, 1)] public float advertisingBonus = 0.2f;

        [Header("Components")]
        public Player player;
        public PlayerItems _items;
        public PlayerBuildings _buildings;
        public PlayerResearches _researches;
        public PlayerHeroes _heroes;

        public ScriptableBuilding requeredBuilding;

        public List<ScriptableRecipe> RecipesByType(ScriptableRecipe[] recipes, RecipeType type)
        {
            List<ScriptableRecipe> temp = new List<ScriptableRecipe>();
            foreach (ScriptableRecipe v in recipes)
            {
                if (v.recipeType == type)
                    temp.Add(v);
            }

            return temp;
        }

        public void CmdChangeStateForCraftRecipe(string buildingname, string recipename)
        {
            if (ScriptableBuilding.dict.TryGetValue(buildingname.GetStableHashCode(), out ScriptableBuilding bdata))
            {
                if (ScriptableRecipe.dict.TryGetValue(recipename.GetStableHashCode(), out ScriptableRecipe data))
                {
                    int index = _buildings.FindIndex(bdata);
                    Building building = _buildings.buildings[index];

                    if (building.data is ScriptableProductionBuilding productionBuilding)
                    {
                        int recipeIndex = productionBuilding.GetCraftRecipeIndex(data);

                        //if enabled - set to disable
                        if (building.craftEnable[recipeIndex])
                        {
                            building.craftEnable = new bool[productionBuilding.craftingRecipes.Length];
                            for (int i = 0; i < building.craftEnable.Length; i++)
                            {
                                building.craftEnable[i] = _buildings.buildings[index].craftEnable[i];
                            }

                            building.craftEnable[recipeIndex] = false;
                            _buildings.buildings[index] = building;
                        }
                        else
                        {
                            //check required researches
                            if (player.researches.CheckRequiredResearcheslist(data.researches))
                            {
                                //check free workers
                                if (player.inhabitants.InhabitantsFree() >= data.inhabitants)
                                {
                                    //check ingredients amount
                                    if (_items.EnoughItems(data.ingredients))
                                    {
                                        //free space in storage
                                        if (data.resultItem.storageType == StorageType.none || _items.FreeSpaceInStorage(data.resultItem.storageType) >= data.resultAmount)
                                        {
                                            building.craftEnable = new bool[productionBuilding.craftingRecipes.Length];
                                            building.lastTimeCraft = new float[productionBuilding.craftingRecipes.Length];

                                            for (int i = 0; i < building.craftEnable.Length; i++)
                                            {
                                                building.craftEnable[i] = _buildings.buildings[index].craftEnable[i];
                                                building.lastTimeCraft[i] = _buildings.buildings[index].lastTimeCraft[i];
                                            }

                                            building.craftEnable[recipeIndex] = true;

                                            //set time
                                            float bonus = player.researches.CraftSpeedBonus();
                                            if (player.adsDisabled) bonus += advertisingBonus;

                                            float bonusTimeForManager = data.time * _heroes.BonusForManager(building.data.name, HeroBonusType.craftingSpeed);
                                            building.lastTimeCraft[recipeIndex] = (data.time - bonusTimeForManager) * (1 - bonus);

                                            _buildings.buildings[index] = building;
                                        }
                                        else RpcShowInfoMessage(CraftInfoMessageType.storage, data.name);
                                    }
                                    else RpcShowInfoMessage(CraftInfoMessageType.ingredients, data.name);
                                }
                                else RpcShowInfoMessage(CraftInfoMessageType.workers, data.name);
                            }
                            else RpcShowInfoMessage(CraftInfoMessageType.researches, data.name);
                        }
                    }
                }
            }
        }

        private void RpcShowInfoMessage(CraftInfoMessageType type, string recipename)
        {
            if (ScriptableRecipe.dict.TryGetValue(recipename.GetStableHashCode(), out ScriptableRecipe data))
            {
                //show error panel
                if (type == CraftInfoMessageType.researches)
                {
                    if (Localization.languageCurrent == SystemLanguage.English)
                    {
                        if (data.researches.Length == 1)
                            UIError.singleton.ShowListScriptableResearchAndLevel("Need to get knowledge", data.researches, "");
                        else
                            UIError.singleton.ShowListScriptableResearchAndLevel("Need to get knowledges", data.researches, "");
                    }
                    else if (Localization.languageCurrent == SystemLanguage.Russian)
                    {
                        if (data.researches.Length == 1)
                            UIError.singleton.ShowListScriptableResearchAndLevel("Необходимо получить знание", data.researches, "");
                        else
                            UIError.singleton.ShowListScriptableResearchAndLevel("Необходимо получить знания", data.researches, "");
                    }
                }
                else if (type == CraftInfoMessageType.ingredients)
                {
                    if (Localization.languageCurrent == SystemLanguage.English)
                    {
                        //UIError.singleton.ShowListScriptableItemAndAmount(player, "Not enough Ingredients", data.ingredients, "");
                    }
                    else if (Localization.languageCurrent == SystemLanguage.Russian)
                    {
                        //UIError.singleton.ShowListScriptableItemAndAmount(player, "Не достаточно ресурсов", data.ingredients, "");
                    }
                }
                else if (type == CraftInfoMessageType.workers)
                {
                    ScriptableItemAndAmount temp = new ScriptableItemAndAmount();
                    temp.item = player.inhabitants.scriptableItem;
                    temp.amount = data.inhabitants;

                    if (Localization.languageCurrent == SystemLanguage.English)
                    {
                        //UIError.singleton.ShowScriptableItemAndAmount(player, "Not enough workers", temp, "");

                    }
                    else if (Localization.languageCurrent == SystemLanguage.Russian)
                    {
                        //UIError.singleton.ShowScriptableItemAndAmount(player, "Не достаточно рабочих", temp, "");

                    }
                }
                else if (type == CraftInfoMessageType.storage)
                {
                    if (Localization.languageCurrent == SystemLanguage.English)
                        UIError.singleton.ShowTextError("The storage for this resource is full");
                    else if (Localization.languageCurrent == SystemLanguage.Russian)
                        UIError.singleton.ShowTextError("Хранилище для этого ресурса заполнено");
                }
            }
        }
    }
}