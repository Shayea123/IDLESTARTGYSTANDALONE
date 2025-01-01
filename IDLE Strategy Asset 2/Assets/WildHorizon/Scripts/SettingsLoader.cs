using UnityEngine;
using UnityEngine.Audio;

namespace IdleStrategyKit
{
    public class SettingsLoader : MonoBehaviour
    {
        [SerializeField]private AudioMixer mixer;

        public static bool disableBetaPanel = false;
        public static bool showIdleAway = true;
        public static bool showInfoPanel = true;
        public static bool showQuestOnStart = true;

        private void Start()
        {
            //does not work correctly in the method OnEnable
            LoadAudioSettings();
            LoadMiscSettins();
        }

        private void LoadAudioSettings()
        {
            if (mixer)
            {
                if (PlayerPrefs.HasKey("VolumeMaster")) mixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("VolumeMaster"));
                if (PlayerPrefs.HasKey("VolumeMusic")) mixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("VolumeMusic"));
                if (PlayerPrefs.HasKey("VolumeSounds")) mixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("VolumeSounds"));
            }
        }

        private void LoadMiscSettins()
        {
            if (PlayerPrefs.HasKey("Notifications_Beta")) disableBetaPanel = UIUtils.ToBool(PlayerPrefs.GetInt("Notifications_Beta"));
            if (PlayerPrefs.HasKey("Notifications_IdleAway")) showIdleAway = UIUtils.ToBool(PlayerPrefs.GetInt("Notifications_IdleAway"));
            if (PlayerPrefs.HasKey("Notifications_InfoPanel")) showInfoPanel = UIUtils.ToBool(PlayerPrefs.GetInt("Notifications_InfoPanel"));
            if (PlayerPrefs.HasKey("Notifications_QuestOnStart")) showQuestOnStart = UIUtils.ToBool(PlayerPrefs.GetInt("Notifications_QuestOnStart"));
        }
    }
}