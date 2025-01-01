using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIInfo : MonoBehaviour
    {
        public static UIInfo singleton;
        public UIInfo()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            if (singleton == null) singleton = this;
        }

        public GameObject panel;
        public Text text;

        public void Show(string message)
        {
            if (SettingsLoader.showInfoPanel)
            {
                text.text = message;
                panel.SetActive(true);
            }
        }
    }
}


