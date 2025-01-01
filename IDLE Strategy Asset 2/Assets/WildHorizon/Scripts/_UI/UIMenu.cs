using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIMenu : MonoBehaviour
    {
        [Header("Buttons")]
        public Button buttonTown;
        public Button buttonBuild;
        public Button buttonResearch;
        public Button buttonHeroes;
        public Button buttonBoosts;
        public Button buttonBattles;
        public Button buttonShop;
        public Button buttonFortune;
        public Button buttonTrade;

        [Header("Panels")]
        public GameObject panelTown;
        public GameObject panelBuild;
        public GameObject panelResearch;
        public GameObject panelHeroes;
        public GameObject panelBoosts;
        public GameObject panelBattles;
        public GameObject panelShop;

        [Header("Panels info - red image")]
        public GameObject panelInfoTown;
        public GameObject panelInfoBuild;
        public GameObject panelInfoResearch;
        public GameObject panelInfoHeroes;
        public GameObject panelInfoBoosts;
        public GameObject panelInfoShop;

        [Header("Components")]
        public UIAudio _audio;

        private void Start()
        {
            buttonTown.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();
                panelTown.SetActive(true);
            });

            buttonBuild.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();
                panelBuild.SetActive(!panelBuild.activeSelf);
            });

            buttonHeroes.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();
                panelHeroes.SetActive(true);
            });

            buttonBoosts.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();
            });

            buttonBattles.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();
            });

            buttonShop.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();
                panelShop.SetActive(true);
            });
        }

        private void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                buttonFortune.gameObject.SetActive(player.TownMenuAvailable());
                buttonTown.interactable = player.TownMenuAvailable();

                buttonHeroes.interactable = player.accessToTheHeroes.item == null || player.buildings.buildings[player.buildings.FindIndex(player.accessToTheHeroes.item)].level >= player.accessToTheHeroes.requiredBuildingLevel;
                buttonBoosts.interactable = player.accessToTheBoosts.item == null || player.buildings.buildings[player.buildings.FindIndex(player.accessToTheBoosts.item)].level > 0;
                buttonBattles.interactable = player.accessToTheBattles.item == null || player.buildings.buildings[player.buildings.FindIndex(player.accessToTheBattles.item)].level >= player.accessToTheBattles.requiredBuildingLevel;
                buttonShop.interactable = player.TownMenuAvailable();

                //panelInfoTown.SetActive(buttonTown.interactable && player.quests.HaveCompletedQuest() == true);
                panelInfoBuild.SetActive(!panelBuild.activeSelf && player.quests.CheckCurrentQuestForBuild());
                panelInfoHeroes.SetActive(player.heroes.IsHeroWaiting());

                buttonResearch.onClick.SetListener(() =>
                {
                    _audio.PlaySoundButtonClick();
                    if (player.researches.requeredBuilding == null || player.buildings.GetBuildingLevel(player.researches.requeredBuilding) > 0)
                        panelResearch.SetActive(true);
                    else
                    {
                        UIShowRequiredBuilding.singleton.Show(player.researches.requeredBuilding, "To start learning new technologies you need to build an institute.");

                    }
                });
            }
        }
    }
}