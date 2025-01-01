using TMPro;
using UnityEngine;
using System;

namespace IdleStrategyKit
{
    public class BuildingOnMap : Entity
    {
        public ScriptableBuilding building;
        public SpriteRenderer spriteRenderer;
        public ScriptableBuilding target;
        public int indexInGlobalList = -1;
        public bool showBuildPanelthenCleaningTheTerritory = true;
        public UIConstructionTime panelTime;

        [Header("Workers")]
        public GameObject[] workers;
        public int levelIndex = 5;

        private void Start()
        {
            //SetBuilding();
        }

        private void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                SetBuilding(player);
            }
        }

        private void SetBuilding(Player player)
        {
            if (building != null)
            {
                indexInGlobalList = player.buildings.FindIndex(building);

                if (indexInGlobalList != -1)
                {
                    //is the building already built?
                    if (player.buildings.buildings[indexInGlobalList].level > 0)
                    {
                        spriteRenderer.sprite = building.spritesByLevel[player.buildings.buildings[indexInGlobalList].level - 1];
                    }
                    else
                    {
                        if (!player.buildings.buildings[indexInGlobalList].underConstruction)
                        {
                            //если сделана подготова для строительства
                            int index = player.buildings.FindIndex(player.clearTerritoryBuilding);
                            if (index != -1 && player.buildings.buildings[index].level > 0) spriteRenderer.sprite = building.spriteConstruction;
                            else spriteRenderer.sprite = building.spriteBurn != null ? building.spriteBurn : null;
                        }
                        else
                        {
                            if (building.spritesByLevel[0]) spriteRenderer.sprite = building.spritesByLevel[0];
                            //else spriteRenderer.sprite = building.spriteConstruction;
                        }
                    }

                    //panel time and level
                    if (panelTime != null)
                    {
                        if (player.buildings.buildings[indexInGlobalList].underConstruction)
                        {
                            panelTime.panel.SetActive(true);
                            panelTime.textTime.text = Utils.PrettySeconds((int)player.buildings.buildings[indexInGlobalList].time);
                            panelTime.textLevel.text = Localization.Translate("Level") + " " + (player.buildings.buildings[player.buildings.FindIndex(player.clearTerritoryBuilding)].level + 1).ToString();
                        }
                        else if (showBuildPanelthenCleaningTheTerritory && player.buildings.buildings[player.buildings.FindIndex(player.clearTerritoryBuilding)].underConstruction && building.spriteBurn)
                        {
                            panelTime.panel.SetActive(true);
                            panelTime.textTime.text = Utils.PrettySeconds((int)player.buildings.buildings[player.buildings.FindIndex(player.clearTerritoryBuilding)].time);
                        }
                        else panelTime.panel.SetActive(false);
                    }

                    //show workers
                    if (player.buildings.buildings[indexInGlobalList].level > 0)
                    {
                        if (workers.Length > 0)
                        {
                            for (int i = 0; i < workers.Length; i++)
                            {
                                workers[i].SetActive(player.buildings.buildings[indexInGlobalList].level >= levelIndex * i);
                            }
                        }
                    }
                    else
                    {
                        if (workers.Length > 0)
                        {
                            for (int i = 0; i < workers.Length; i++)
                            {
                                workers[i].SetActive(false);
                            }
                        }
                    }
                }
            }
            else
            {
                if (panelTime != null) panelTime.panel.SetActive(false);
            }
        }
    }
}