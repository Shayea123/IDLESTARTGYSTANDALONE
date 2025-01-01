using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIBuildingManagement : MonoBehaviour
    {
        public GameObject panel;
        public GameObject panelBuilding;
        public Button buttonBuild;
        public Text textBuildingName;

        [Header("ADS")]
        public Button buttonManageADS;

        [Header("Manager")]
        public GameObject panelManager;
        public Image imageManagerRarity;
        public Image imageManagerLogo;
        public GameObject managerLevel;
        public TextMeshProUGUI textManagerLevel;
        public TextMeshProUGUI textManagerName;
        public Text textManagerBonuses;
        public Button buttonAddManager;

        [Header("Mining")]
        public GameObject panelMining;
        public GameObject panelWorkers;

        [Header("Trade")]
        public GameObject panelSellBuy;

        [Header("UI Elements : Craft")]
        public GameObject panelCraft;

        [Header("UI Elements : Train Army")]
        public GameObject panelTrainArmy;

        [Header("UI Elements : Prayer")]
        public GameObject panelPrayer;
        public Button buttonPrayForBuildSpeed;
        public Button buttonPrayForResearchSpeed;
        public Image imagePrayBuildSpeed;
        public Image imagePrayResearchSpeed;

        [Header("Components")]
        public UIAudio _audio;
        public UIHeroes heroes;

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
                        //button "Upgrade"
                        buttonBuild.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            panel.SetActive(false);
                            panelBuilding.SetActive(true);
                        });

                        //find this building index in Global list
                        int index = player.buildings.FindIndex(UIBuildingSelect.selectedBuilding);

                        //name
                        textBuildingName.text = Localization.Translate(UIBuildingSelect.selectedBuilding.name) + " : " + Localization.Translate("Level") + " " + player.buildings.buildings[index].level;

                        //ads
                        /*if (building.time > 0 && building.adsAmount > 0)
                        {
                            panelManageAds.SetActive(true);
                            buttonManageADS.interactable = Advertisement.IsReady(GameADS.singleton.rewardedVideo);
                        }
                        else panelManageAds.SetActive(false);*/

                        //manager
                        if (UIBuildingSelect.selectedBuilding.managerAvailable)
                        {
                            panelManager.SetActive(true);

                            int ind = player.heroes.FindHeroIndexByPlace(player.buildings.buildings[index].data.name);
                            if (ind != -1)
                            {
                                Hero hero = player.heroes.heroes[ind];

                                imageManagerRarity.color = player.rarity.GetColor(hero.data);
                                imageManagerLogo.sprite = hero.image;
                                imageManagerLogo.color = Color.white;
                                textManagerName.text = hero.name;

                                managerLevel.SetActive(true);
                                textManagerLevel.text = hero.level.ToString();
                                textManagerBonuses.text = player.heroes.HeroBonuses(hero.data);
                            }
                            else
                            {
                                imageManagerRarity.color = player.rarity.GetColorForEmptySlot();
                                imageManagerLogo.color = Color.black;
                                textManagerName.text = "";
                                textManagerBonuses.text = "";

                                managerLevel.SetActive(false);
                            }

                            buttonAddManager.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                panel.SetActive(false);

                                heroes.panel.SetActive(true);
                            });
                        }
                        else panelManager.SetActive(false);

                        //sell buy
                        panelSellBuy.SetActive(UIBuildingSelect.selectedBuilding.sellAndBuyResources);

                        //mine
                        panelMining.SetActive(UIBuildingSelect.selectedBuilding is ScriptableMiningBuilding);
                        panelWorkers.SetActive(UIBuildingSelect.selectedBuilding is ScriptableMiningBuilding);

                        //craft
                        panelCraft.SetActive(UIBuildingSelect.selectedBuilding is ScriptableProductionBuilding);

                        //train Army
                        panelTrainArmy.SetActive(UIBuildingSelect.selectedBuilding.trainArmy.Length > 0);

                        //Church
                        if (UIBuildingSelect.selectedBuilding.managedPrayer)
                        {
                            panelPrayer.SetActive(true);
                            imagePrayBuildSpeed.gameObject.SetActive(player.prayType == PreyType.buildSpeed);
                            imagePrayResearchSpeed.gameObject.SetActive(player.prayType == PreyType.researchSpeed);

                            buttonPrayForBuildSpeed.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                player.CmdSetPrayType((int)PreyType.buildSpeed);
                            });
                            buttonPrayForResearchSpeed.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                player.CmdSetPrayType((int)PreyType.researchSpeed);
                            });
                        }
                        else panelPrayer.SetActive(false);
                    }
                    else panel.SetActive(false);
                }
                else panel.SetActive(false);
            }
        }
    }
}


