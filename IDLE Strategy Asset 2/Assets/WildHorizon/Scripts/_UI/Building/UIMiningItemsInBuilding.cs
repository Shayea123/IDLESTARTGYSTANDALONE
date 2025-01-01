using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIMiningItemsInBuilding : MonoBehaviour
    {
        public GameObject panel;

        [Header("Mining")]
        public Transform content;
        public GameObject pefab;

        [Header("Workers")]
        public Text textWorkersFree;
        public Text textWorkersMax;

        public Button buttonMinningPlus;
        public Button buttonMinningMinus;
        public Button buttonTransportationPlus;
        public Button buttonTransportationMinus;

        public Text textMineValue;
        public Text textTransportationValue;

        public TMP_Dropdown dropdownMiningTool;
        public TMP_Dropdown dropdownTransportationTool;

        public Text textMineBonus;
        public Text textTransportationBonus;

        [Header("Panel Error")]
        public GameObject panelError;
        public Text textError;

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
                    if (UIBuildingSelect.selectedBuilding is ScriptableMiningBuilding miningBuilding)
                    {
                        //find this building index in Global list
                        int index = player.buildings.FindIndex(UIBuildingSelect.selectedBuilding);
                        Building building = player.buildings.buildings[index];

                        // instantiate/destroy enough slots
                        UIUtils.BalancePrefabs(pefab, miningBuilding.minedResources.Length, content);

                        //refresh all slots
                        for (int i = 0; i < miningBuilding.minedResources.Length; i++)
                        {
                            UIPrefabResourceSlot slot = content.transform.GetChild(i).GetComponent<UIPrefabResourceSlot>();
                            slot.image.sprite = miningBuilding.minedResources[i].item.image;

                            //name
                            slot.textName.text = Localization.Translate(miningBuilding.minedResources[i].item.name);
                            slot.textYield.text = miningBuilding.minedResources[i].yield + " %";

                            float bonusTimeForTool = 0;
                            if (building.mineTool != 0)
                                bonusTimeForTool = miningBuilding.miningRate.Get(building.level) * (((ScriptableInstrument)miningBuilding.miningTools[building.mineTool - 1]).mineSpeedBonus / 100);

                            float bonusTimeForManager = miningBuilding.miningRate.Get(building.level) * player.heroes.BonusForManager(building.data.name, HeroBonusType.miningSpeed);
                            slot.textRate.text = (miningBuilding.miningRate.Get(building.level) - bonusTimeForManager - bonusTimeForTool).ToString("F2") + "/s";
                            slot.textAmount.text = UIUtils.LongToString(building.resources[i]);
                        }

                        //show texts info (workers amount)
                        uint workersFree = player.inhabitants.InhabitantsFree();
                        textWorkersFree.text = workersFree.ToString();
                        textWorkersMax.text = miningBuilding.workersMax.Get(building.level).ToString();

                        //mine
                        textMineValue.text = player.buildings.buildings[index].workersMining.ToString();
                        buttonMinningPlus.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            if (workersFree > 0)
                            {
                                if (player.buildings.buildings[index].workersMining < miningBuilding.workersMax.Get(building.level))
                                {
                                    player.buildings.CmdIncreaseWorkerForMining(index, 1);
                                }
                                else
                                {
                                    textError.text = "Maximum number of workers \n Need to upgrade the building";
                                    panelError.SetActive(true);
                                }
                            }
                            else
                            {
                                textError.text = "No more free workers";
                                panelError.SetActive(true);
                            }
                        });
                        buttonMinningMinus.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            player.buildings.CmdDecreaseWorkerForMining(index, 1);
                        });

                        //transportation
                        textTransportationValue.text = building.workersTransportation.ToString();
                        buttonTransportationPlus.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            if (workersFree > 0)
                            {
                                if (building.workersTransportation < miningBuilding.workersMax.Get(building.level))
                                {
                                    player.buildings.CmdIncreaseWorkerForTransportation(index, 1);
                                }
                                else
                                {
                                    textError.text = "Maximum number of workers \n Need to upgrade the building";
                                    panelError.SetActive(true);
                                }
                            }
                            else
                            {
                                textError.text = "No more free workers";
                                panelError.SetActive(true);
                            }
                        });
                        buttonTransportationMinus.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            player.buildings.CmdDecreaseWorkerForTransportation(index, 1);
                        });

                        //dropdown
                        ShowDropdowns(player, miningBuilding);

                        dropdownMiningTool.value = building.mineTool;
                        dropdownMiningTool.onValueChanged.SetListener(delegate
                        {
                            player.buildings.CmdSetToolForMining(index, dropdownMiningTool.value);
                        });

                        dropdownTransportationTool.value = building.transportationTool;
                        dropdownTransportationTool.onValueChanged.SetListener(delegate
                        {
                            player.buildings.CmdSetToolForTransportation(index, dropdownTransportationTool.value);
                        });

                        //bonuses
                        if (building.mineTool != 0)
                        {
                            textMineBonus.text = "Speed Bonus : " + ((ScriptableInstrument)miningBuilding.miningTools[building.mineTool - 1]).mineSpeedBonus + "%";
                        }
                        else
                        {
                            if (Localization.languageCurrent == SystemLanguage.English)
                                textMineBonus.text = "Bonus : none";
                            else if (Localization.languageCurrent == SystemLanguage.Russian)
                                textMineBonus.text = "Бонус : нет";
                        }

                        if (building.transportationTool != 0)
                        {
                            ScriptableInstrument go = (ScriptableInstrument)miningBuilding.transportationTools[building.transportationTool - 1];
                            string bonus = "";
                            if (go.transportationSpeedBonus > 0) bonus += "Speed Bonus : " + go.transportationSpeedBonus + "%";
                            if (go.transportationWeightBonus > 0) bonus += "Weight Bonus : " + go.transportationWeightBonus + "%";

                            textTransportationBonus.text = bonus;
                        }
                        else
                        {
                            if (Localization.languageCurrent == SystemLanguage.English)
                                textTransportationBonus.text = "Bonus : none";
                            else if (Localization.languageCurrent == SystemLanguage.Russian)
                                textTransportationBonus.text = "Бонус : нет";
                        }
                    }
                }
            }
        }

        //mine dropdowns
        public void ShowDropdowns(Player player, ScriptableMiningBuilding building)
        {
            dropdownMiningTool.ClearOptions();
            dropdownTransportationTool.ClearOptions();

            //mining
            List<string> values = new List<string>();
            values.Add(Localization.Translate("Instruments"));

            for (int i = 0; i < building.miningTools.Length; i++)
            {
                values.Add(building.miningTools[i].name + " : " + player.items.GetItemAmount(building.miningTools[i]));
            }
            dropdownMiningTool.AddOptions(values);

            //transportation
            values = new List<string>();
            values.Add(Localization.Translate("Instruments"));

            for (int i = 0; i < building.transportationTools.Length; i++)
            {
                values.Add(building.transportationTools[i].name + " : " + player.items.GetItemAmount(building.transportationTools[i]));
            }
            dropdownTransportationTool.AddOptions(values);
        }
    }
}


