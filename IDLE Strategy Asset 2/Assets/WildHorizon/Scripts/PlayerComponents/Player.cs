using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    [Serializable]
    public struct ScriptableBuildingAndAmountOrLevel
    {
        public int forLevel;
        public ScriptableBuilding item;
        public int amount;
        public int requiredBuildingLevel;
    }
    [Serializable]
    public struct BuildingAndAmountOrLevel
    {
        public ScriptableBuilding item;
        public int amount;
        public int requiredLevel;
    }
    [Serializable]
    public struct ScriptableResearchAndLevel
    {
        public int forLevel;
        public ScriptableResearch item;
        public int researachLevel;
    }
    [Serializable]
    public struct ResearchAndLevel
    {
        public ScriptableResearch item;
        public uint requiredLevel;
    }

    [Serializable]
    public struct ScriptableItemAmountAndPrice
    {
        [HideInInspector]public string name;
        public ScriptableItem item;
        [Min (1)] public uint amount;
        [Min (1)] public uint price;
    }
    [Serializable]
    public struct AmountPriceAndSprite
    {
        public uint amount;
        public uint price;
        public Sprite sprite;
    }

    public enum StorageType { none, inhabitants, bank, warehouse, waterTower, armory, army }
    public enum MiningMethod { none, hunters, water }
    public enum GameType { story, confrontation, survival }
    public enum SortBy { none, name, amountAscending, amountDescending, priceAscending, priceDescending }
    public enum HeroBonusType
    {
        inhabitansIncrease,
        dollarsIncrease,
        buildingSpeed,
        researchSpeed,
        miningSpeed,
        miningAmount,
        transportationSpeed,
        transportationWeight,
        processingSpeed,
        craftingSpeed
    }
    public enum PreyType { none, buildSpeed, researchSpeed }

    public class Player : MonoBehaviour
    {
        [Header("Components")]
        public PlayerInhabitants inhabitants;
        public PlayerItems items;
        public PlayerBuildings buildings;
        public PlayerQuests quests;
        public PlayerResearches researches;
        public PlayerCraft craft;
        public PlayerHeroes heroes;
        public PlayerBoosts boosts;
        public PlayerShop shop;
        public PlayerArmy army;
        public PlayerFortune fortune;
        public PlayerEnemyCamps camps;
        public PlayerNotifications notifications;


        [Header("Gamedata")]
        public ScriptableItemRarity rarity;
        public ScriptableItem coinsItem;
        public ScriptableItem dollarsItem;
        public ScriptableBuilding clearTerritoryBuilding;
        public ScriptableItem townRenameItem;

        [Header("Settings")]
        public float saveInterval = 30;
        public ScriptableBuildingAndAmountOrLevel accessToTheTown;
        public ScriptableBuildingAndAmountOrLevel accessToTheResearches;
        public ScriptableBuildingAndAmountOrLevel accessToTheHeroes;
        public ScriptableBuildingAndAmountOrLevel accessToTheBoosts;
        public ScriptableBuildingAndAmountOrLevel accessToTheBattles;
        public ScriptableBuildingAndAmountOrLevel accessToTheCampTrade;

        public bool TownMenuAvailable() => accessToTheTown.item == null || buildings.buildings[buildings.FindIndex(accessToTheTown.item)].level > 0;

        // some meta info
        [HideInInspector] public string account = "";

        // localPlayer singleton for easier access from UI scripts etc.
        public static Player localPlayer;

        // cache players to save lots of computations
        // (otherwise we'd have to iterate NetworkServer.objects all the time)
        // => on server: all online players
        // => on client: all observed players
        public static Dictionary<string, Player> onlinePlayers = new Dictionary<string, Player>();

        [HideInInspector] public GameType newGameType = GameType.story;
        public GameType gameType = GameType.story;

        [Header("Town Info")]
        public string townName = "";

        public PreyType prayType = PreyType.none;

        [Header("Idle Away time")]
        public float availableIdleAwaySeconds = 0;
        public int idleAwaySecondsMin = 7200;
        public int idleAwaySecondsMax = 86400;
        public DateTime lastSession;

        [Header("Ads decrease time")]
        public float decreaseBuildingTimeByAdsShow = 15;
        public float decreaseResearchingTimeByAdsShow = 15;

        [Header("ADS")]
        public bool adsDisabled = false;
        [HideInInspector] public DateTime timeLastShowAdsForFortune;

        [Header("Panels Beta")]
        public GameObject panelBeta;
        public Toggle toggleBeta;

        [Header("Panel Chapter")]
        public GameObject panelChapter;

        [Header("Panel Idle Time")]
        public GameObject panelIdleTime;
        public Text textAwayTimeCurrent;
        public Text textAwayTimeMax;
        public GameObject panelResources;
        public Transform contentIdleTime;
        public GameObject resourcesPrefab;
        public Button buttonShowAd;

        private float availableSeconds = 0;

        //not saved lists
        public static List<ScriptableItemAndAmount> resourcesIdleTime = new List<ScriptableItemAndAmount>();

        public ScriptableBuilding requiredBuildingForIdlePanel;

        public static bool idleByAds = false;//
        public static float adsTime = 0;//

        private void Start()
        {
            // set singleton
            localPlayer = this;

            UISettings.StartNewStoryGame.AddListener(StartNewStoryGame);
            Application.runInBackground = true;
        }

        public void StartNewStoryGame()
        {
            notifications.notifications = new List<Notifications>();
            CmdStartNewStoryGame();

            //show panel beta if this option is not disabled
            if (toggleBeta.isOn)
            {
                panelBeta.SetActive(true);
                toggleBeta.onValueChanged.AddListener(delegate
                {
                    SettingsLoader.disableBetaPanel = toggleBeta.isOn;
                });
            }
        }

        public void CmdStartNewStoryGame()
        {
            //show Panel "Chapter"
            panelChapter.SetActive(true);

            townName = "";

            quests.CreateNewlist();
            buildings.CreateNewlist();
            researches.CreateNewlist();
            heroes.StartNewGame();
            boosts.CreateNewlist();

            //add default resources
            inhabitants.StartNewGame();
            items.StartNewGame();
            army.StartNewGame();

            prayType = PreyType.none;
        }

        public void TownRename(string newname)
        {
            if (!string.IsNullOrEmpty(newname) && newname != townName)
            {
                //decrease requered item
                items.DecreaseItemAmount(townRenameItem, 1);

                townName = newname;

                UITown.singleton.TownRenameReply();
            }
        }

        public void CmdSetPrayType(int type)
        {
            if (prayType == (PreyType)type) prayType = PreyType.none;
            else prayType = (PreyType)type;
        }

        //idle time
        public void LoadLastSession()
        {
            TimeSpan ts = DateTime.UtcNow - lastSession;
            availableSeconds = ts.TotalSeconds >= availableIdleAwaySeconds ? availableIdleAwaySeconds : (float)ts.TotalSeconds;

            //decrease availableIdleAwaySeconds
            if (availableIdleAwaySeconds > idleAwaySecondsMin)
            {
                availableIdleAwaySeconds -= (int)ts.TotalSeconds;

                if (availableIdleAwaySeconds < idleAwaySecondsMin) availableIdleAwaySeconds = idleAwaySecondsMin;
            }
            else if (availableIdleAwaySeconds < idleAwaySecondsMin)
            {
                availableIdleAwaySeconds = idleAwaySecondsMin;
            }

            buildings.UpdateBuildingsByIdleTime(availableSeconds);
            researches.DecreaseResearchTime(availableSeconds);


            if (SettingsLoader.showIdleAway && availableSeconds > 10 &&
                (requiredBuildingForIdlePanel == null || buildings.buildings[buildings.FindIndex(requiredBuildingForIdlePanel)].level > 0))
            {
                Debug.Log("load last 2");

                textAwayTimeCurrent.text = Utils.PrettySeconds((int)availableSeconds);
                textAwayTimeMax.text = "Max idle time is Now: " + Utils.PrettySeconds((int)availableIdleAwaySeconds);

                if (resourcesIdleTime.Count > 0)
                {
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(resourcesPrefab, resourcesIdleTime.Count, contentIdleTime);

                    //refresh all slots
                    for (int i = 0; i < resourcesIdleTime.Count; i++)
                    {
                        UIIngredientResourceSlot slot = contentIdleTime.transform.GetChild(i).GetComponent<UIIngredientResourceSlot>();
                        slot.image.sprite = resourcesIdleTime[i].item.image;
                        slot.textName.text = resourcesIdleTime[i].item.name + " : " + resourcesIdleTime[i].amount;
                    }
                    panelResources.SetActive(true);
                    buttonShowAd.gameObject.SetActive(true);
                }
                else
                {
                    panelResources.SetActive(false);
                    buttonShowAd.gameObject.SetActive(false);
                }

                panelIdleTime.SetActive(true);
            }
            else
            {
                Debug.Log("load last 3 " + availableSeconds);

            }
        }
        public void AddItemsForIdlePanel()
        {
            for (int i = 0; i < resourcesIdleTime.Count; i++)
            {
                items.IncreaseItemAmount(resourcesIdleTime[i].item, resourcesIdleTime[i].amount);
            }

            resourcesIdleTime.Clear();
        }
        public void ShowAdIdleTime()
        {
            //GameADS.singleton.ShowRewardedAd(AdsType.idleTime);
        }
        public void ClosePanelIdleTime()
        {
            for (int i = 0; i < resourcesIdleTime.Count; i++)
            {
                items.IncreaseItemAmount(resourcesIdleTime[i].item, resourcesIdleTime[i].amount);
            }

            resourcesIdleTime.Clear();
        }

        void Update()
        {
            //buttonShowAd.interactable = Advertisement.IsReady(GameADS.singleton.rewardedVideo);
        }

        public void ClosePanelBeta()
        {
            StartCoroutine(HidePanelChapters());
        }

        public IEnumerator HidePanelChapters()
        {
            yield return new WaitForSeconds(3);

            panelChapter.SetActive(false);
        }
    }
}


