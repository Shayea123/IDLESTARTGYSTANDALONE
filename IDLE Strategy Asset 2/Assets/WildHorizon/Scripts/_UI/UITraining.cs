using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UITraining : MonoBehaviour
    {
        public GameObject panelTraining;
        public TextMeshProUGUI text;
        public Button button;

        [Header("Components")]
        public UIAudio _audio;

        public void Show()
        {
            panelTraining.SetActive(true);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            button.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();

                panelTraining.SetActive(false);
            });
        }
    }
}


