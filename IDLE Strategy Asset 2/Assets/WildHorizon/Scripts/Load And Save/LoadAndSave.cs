using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;


namespace IdleStrategyKit
{
    [Serializable]
    class SaveSettingsData
    {
        //language
        public SystemLanguage language;

        //audio
        public float soundsVolume = 0.5f;
        public float musicVolume = 0.5f;
        public bool volumeState = true;

        //setings
        public bool disableBetaPanel;
        public bool showIdleAway;
        public bool showInfoPanel;
        public bool showPushMessage;
        public bool showQuestOnStart;
    }

    [Serializable]
    class SaveData
    {
        public DateTime lastSession;
        public float availableIdleAwaySeconds;

        //ads
        public bool adsDisabled;
        public DateTime timeLastShowAdsForFortune;

        //account info
        public GameType gameType;
        public string townName = "Hill Valley";
        public PreyType prayType;

        //
        public uint inhabitants;
        public uint inhabitantsStock;

        //relations with other factions
        public float factionIndians;
        public float factionCowboys;
        public float factionBandits;
        public float factionMexicans;

        //workers on default resources
        public SendWorkers[] sendWorkers;

        //lists
        public List<Quest> quests;
        public List<ItemSlot> items;
        public List<Building> buildings;
        public List<Research> researches;
        public List<Hero> heroes;
        public List<Boost> boosts;
        public List<Boost> boostsActive;
        public List<Army> armies;
    }

    public class LoadAndSave : MonoBehaviour
    {
        [Header("Components")]
        public Player player;
        public PlayerInhabitants inhabitants;
        public PlayerEnemyCamps enemyCamps;
        public PlayerItems items;
        public PlayerQuests quests;
        public PlayerBuildings buildings;
        public PlayerResearches researches;
        public PlayerHeroes heroes;
        public PlayerArmy army;

        [Header("Options")]
        [SerializeField] private string saveName = "MySaveData";
        [SerializeField] private string saveExtension = "dat";
        [SerializeField] private float saveInterval = 30;

        private void OnEnable()
        {
            if (LoadGame() == false)
            {
                player.StartNewStoryGame();
            }
            else
            {
                player.LoadLastSession();
            }
        }

        private void Start()
        {
            InvokeRepeating(nameof(SaveGame), saveInterval, saveInterval);
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) SaveGame();
        }

        //public static bool LoadSettings()
        //{
        //    if (File.Exists(Application.persistentDataPath + "/SettingsData.dat"))
        //    {
        //        BinaryFormatter bf = new BinaryFormatter();
        //        FileStream file = File.Open(Application.persistentDataPath + "/SettingsData.dat", FileMode.Open);
        //        SaveSettingsData data = (SaveSettingsData)bf.Deserialize(file);
        //        file.Close();

        //        //language
        //        Localization.languageCurrent = data.language;

        //        //audio
        //        SettingsLoader.volumeState = data.volumeState;
        //        SettingsLoader.musicVolume = data.musicVolume;
        //        SettingsLoader.soundsVolume = data.soundsVolume;

        //        //settings
        //        SettingsLoader.disableBetaPanel = data.disableBetaPanel;
        //        SettingsLoader.showIdleAway = data.showIdleAway;
        //        SettingsLoader.showInfoPanel = data.showInfoPanel;
        //        SettingsLoader.showQuestOnStart = data.showQuestOnStart;

        //        Debug.Log("Settings loaded!");
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static void SaveSettings()
        //{
        //    BinaryFormatter bf = new BinaryFormatter();
        //    FileStream file = File.Create(Application.persistentDataPath + "/SettingsData.dat");
        //    SaveSettingsData data = new SaveSettingsData();

        //    //languages
        //    data.language = Localization.languageCurrent;

        //    //audio
        //    data.volumeState = SettingsLoader.volumeState;
        //    data.musicVolume = SettingsLoader.v;
        //    data.soundsVolume = SettingsLoader.soundsVolume;

        //    //settings
        //    data.disableBetaPanel = SettingsLoader.disableBetaPanel;
        //    data.showIdleAway = SettingsLoader.showIdleAway;
        //    data.showInfoPanel = SettingsLoader.showInfoPanel;
        //    data.showQuestOnStart = SettingsLoader.showQuestOnStart;

        //    bf.Serialize(file, data);
        //    file.Close();
        //    Debug.Log("Settings saved!");
        //}

        public bool LoadGame()
        {
            if (File.Exists(Application.persistentDataPath + "/MySaveData.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/MySaveData.dat", FileMode.Open);
                SaveData data = (SaveData)bf.Deserialize(file);
                file.Close();

                player.lastSession = data.lastSession;
                player.availableIdleAwaySeconds = data.availableIdleAwaySeconds;

                //ads
                player.adsDisabled = data.adsDisabled;
                player.timeLastShowAdsForFortune = data.timeLastShowAdsForFortune;

                //account info
                player.townName = data.townName;
                inhabitants.SetCurrent = data.inhabitants;
                inhabitants.SetReserve = data.inhabitantsStock;
                player.prayType = data.prayType;

                //relations with other factions
                enemyCamps.factionIndians = data.factionIndians;
                enemyCamps.factionCowboys = data.factionCowboys;
                enemyCamps.factionBandits = data.factionBandits;
                enemyCamps.factionMexicans = data.factionMexicans;

                //quests
                quests.quests = data.quests;

                //items
                items.items = data.items;

                //buildings
                buildings.buildings = data.buildings;

                //researches
                researches.researches = data.researches;

                //heroes
                heroes.heroes = data.heroes;

                //armies
                army.armies = data.armies;

                Debug.Log("Game data loaded!");
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SaveGame()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/MySaveData.dat");
            SaveData data = new SaveData();

            data.lastSession = DateTime.UtcNow;
            data.availableIdleAwaySeconds = player.availableIdleAwaySeconds;

            //ads
            data.adsDisabled = player.adsDisabled;
            data.timeLastShowAdsForFortune = player.timeLastShowAdsForFortune;

            //account info
            data.gameType = player.gameType;
            data.townName = player.townName;
            data.inhabitants = inhabitants.GetCurrent();
            data.inhabitantsStock = inhabitants.GetReserve();
            data.prayType = player.prayType;

            //relations with other factions
            data.factionIndians = enemyCamps.factionIndians;
            data.factionCowboys = enemyCamps.factionCowboys;
            data.factionBandits = enemyCamps.factionBandits;
            data.factionMexicans = enemyCamps.factionMexicans;

            //items
            data.quests = quests.quests;
            data.items = items.items;
            data.buildings = buildings.buildings;
            data.researches = researches.researches;
            data.heroes = heroes.heroes;
            data.armies = army.armies;


            bf.Serialize(file, data);
            file.Close();
            Debug.Log("Game data saved!");
        }

        public void ResetData()
        {
            string path = Application.persistentDataPath + "/" + saveName + "." + saveExtension;
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.LogError("Data delete complete!");
            }
            else Debug.LogError("No save data to delete.");
        }

        public void OpenSavePath()
        {
#if UNITY_EDITOR
            EditorUtility.OpenFilePanel("MySaveData", Application.persistentDataPath, saveExtension);
#endif
        }
    }
}