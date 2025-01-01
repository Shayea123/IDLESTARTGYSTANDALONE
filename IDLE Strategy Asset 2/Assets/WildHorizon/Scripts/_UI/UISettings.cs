using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UISettings : MonoBehaviour
    {
        [Header("Components")]
        public UIAudio _audio;

        public static UnityEvent StartNewStoryGame = new UnityEvent();

        [Space(20)]
        public Button buttonOpen;
        public GameObject panel;
        public GameObject panelNewGame;
        public Button buttonClose;
        public Button buttonRateUs;
        public Button buttonNewGamePanel;
        public Button buttonStartNewGame;
        public Text textVersion;

        public Toggle toggleShowIdleAway;
        public Toggle toggleShowInfoPanel;
        public Toggle toggleShowPushMessage;
        public Toggle toggleShowQuestOnStart;

        private void Start()
        {
            buttonOpen.onClick.SetListener(() => {
                _audio.PlaySoundButtonClick();
                panel.SetActive(true);
            });

            buttonClose.onClick.SetListener(() => {
                _audio.PlaySoundButtonClick();
                panel.SetActive(false);
            });

            buttonRateUs.onClick.SetListener(() => {
                _audio.PlaySoundButtonClick();
                panel.SetActive(false);
                UIRateUs.ShowRateUs?.Invoke();
            });

            buttonNewGamePanel.onClick.SetListener(() => {
                _audio.PlaySoundButtonClick();
                panelNewGame.SetActive(true);
            });

            buttonStartNewGame.onClick.SetListener(() => {
                _audio.PlaySoundButtonClick();
                panel.SetActive(false);
                panelNewGame.SetActive(false);

                SettingsLoader.showQuestOnStart = true;
                PlayerPrefs.SetInt("Notifications_QuestOnStart", SettingsLoader.showQuestOnStart.ToInt());
                StartNewStoryGame?.Invoke();
            });

            toggleShowIdleAway.isOn = SettingsLoader.showIdleAway;
            toggleShowInfoPanel.isOn = SettingsLoader.showInfoPanel;
            toggleShowQuestOnStart.isOn = SettingsLoader.showQuestOnStart;

            //settings
            toggleShowIdleAway.onValueChanged.AddListener(delegate
            {
                _audio.PlaySoundButtonClick();
                SettingsLoader.showIdleAway = toggleShowIdleAway.isOn;
                PlayerPrefs.SetInt("Notifications_IdleAway", UIUtils.ToInt(SettingsLoader.showIdleAway));
            });
            toggleShowInfoPanel.onValueChanged.AddListener(delegate
            {
                _audio.PlaySoundButtonClick();
                SettingsLoader.showInfoPanel = toggleShowInfoPanel.isOn;
                PlayerPrefs.SetInt("Notifications_InfoPanel", UIUtils.ToInt(SettingsLoader.showInfoPanel));
            });
            toggleShowQuestOnStart.onValueChanged.AddListener(delegate
            {
                _audio.PlaySoundButtonClick();
                SettingsLoader.showQuestOnStart = toggleShowQuestOnStart.isOn;
                PlayerPrefs.SetInt("Notifications_QuestOnStart", UIUtils.ToInt(SettingsLoader.showQuestOnStart));
            });

            textVersion.text = "Ver " + Application.version.ToString();
        }
    }
}