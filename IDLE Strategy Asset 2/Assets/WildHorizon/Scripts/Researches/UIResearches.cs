using UnityEngine;

namespace IdleStrategyKit
{
    public class UIResearches : MonoBehaviour
    {
        [Header("Components")]
        public UIAudio _audio;

        public GameObject panel;
        public Transform content;
        public GameObject researchTypeContentPrefab;
        public GameObject researchPrefab;

        public GameObject panelDescription;

        [Header("Images By level")]
        public Sprite imageInProgress_lv1;
        public Sprite[] imageInProgress_lv2;
        public Sprite[] imageInProgress_lv3;
        public Sprite[] imageInProgress_lv4;
        public Sprite[] imageInProgress_lv5;

        [Header("Colors")]
        public Color colorNotReserched;

        public static UIResearches singleton;
        public UIResearches()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            if (singleton == null) singleton = this;
        }

        public static ScriptableResearch selectedResearch;

        private void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    // instantiate/destroy enough slots for 
                    UIUtils.BalancePrefabs(researchTypeContentPrefab.gameObject, player.researches.templates.Length, content);

                    // instantiate/destroy enough slots for researches by types
                    for (int i = 0; i < player.researches.templates.Length; i++)
                        UIUtils.BalancePrefabs(researchPrefab.gameObject, player.researches.templates[i].researches.Length, content.GetChild(i).transform);

                    // refresh all slots
                    for (int i = 0; i < player.researches.templates.Length; i++)
                    {
                        for (int x = 0; x < player.researches.templates[i].researches.Length; x++)
                        {
                            ScriptableResearch data = player.researches.templates[i].researches[x];

                            UIResearchButtonSlot slot = content.GetChild(i).transform.GetChild(x).GetComponent<UIResearchButtonSlot>();
                            slot.imageLogo.sprite = data.sprite;

                            int index = player.researches.FindIndex(data);
                            if (index != -1)
                            {
                                Research research = player.researches.researches[index];
                                slot.imageProgress.sprite = GetCurrentSpriteByLevel(research);
                                slot.imageProgress.color = research.level > 0 ? Color.white : Color.clear;

                                slot.slider.gameObject.SetActive(research.underStudy);
                                if (research.level < data.levels.Length)
                                    slot.slider.maxValue = data.levels[research.level].researchTime;
                                else slot.slider.maxValue = data.levels[research.level - 1].researchTime;
                                slot.slider.value = slot.slider.maxValue - research.time;

                                slot.panelLevel.SetActive(research.level > 0);
                                slot.textLevel.text = research.level.ToString();
                            }
                            else
                            {
                                slot.imageProgress.color = Color.clear;
                                slot.slider.gameObject.SetActive(false);
                                slot.panelLevel.SetActive(false);
                            }

                            int icopy = i;
                            int xcopy = x;
                            slot.button.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                selectedResearch = player.researches.templates[icopy].researches[xcopy];
                                panelDescription.SetActive(true);
                            });
                        }
                    }
                }
                else panel.SetActive(false);
            }
        }

        private Sprite GetCurrentSpriteByLevel(Research research)
        {
            if (research.data.levels.Length == 1)
            {
                return imageInProgress_lv1;
            }
            else if (research.data.levels.Length == 2)
            {
                return research.level == 0 ? imageInProgress_lv2[0] : imageInProgress_lv2[research.level - 1];
            }
            else if (research.data.levels.Length == 3)
            {
                return research.level == 0 ? imageInProgress_lv3[0] : imageInProgress_lv3[research.level - 1];
            }
            else if (research.data.levels.Length == 4)
            {
                return research.level == 0 ? imageInProgress_lv4[0] : imageInProgress_lv4[research.level - 1];
            }

            return null;
        }
    }
}


