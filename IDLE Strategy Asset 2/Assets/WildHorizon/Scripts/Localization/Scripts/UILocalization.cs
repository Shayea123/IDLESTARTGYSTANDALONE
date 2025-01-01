using TMPro;
using UnityEngine;

namespace IdleStrategyKit
{
    public class UILocalization : MonoBehaviour
    {
        [SerializeField] private Lang[] availableLanguages = new Lang[] { };
        public bool changeFont = false;

        public GameObject panel;
        public Transform content;
        public UILocalizationLanguageButtonSlot buttonPrefab;

        public Color colorLangSelect;
        public Color colorLangNonSelect;

        [Header("Components")]
        public UIAudio _audio;

        private void Start()
        {
            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(buttonPrefab.gameObject, availableLanguages.Length, content);

            for (int i = 0; i < availableLanguages.Length; i++)
            {
                UILocalizationLanguageButtonSlot slot = content.transform.GetChild(i).GetComponent<UILocalizationLanguageButtonSlot>();
                slot.text.text = availableLanguages[i].language.ToString();

                int icopy = i;
                slot.button.onClick.AddListener(() =>
                {
                    if (_audio)_audio.PlaySoundButtonClick();
                    Localization.Language = availableLanguages[icopy].language;
                });
            }
        }

        private void Update()
        {
            if (panel.activeSelf)
            {
                for (int i = 0; i < availableLanguages.Length; i++)
                {
                    UILocalizationLanguageButtonSlot slot = content.transform.GetChild(i).GetComponent<UILocalizationLanguageButtonSlot>();

                    if (Localization.languageCurrent == availableLanguages[i].language)
                        slot.image.color = colorLangSelect;
                    else slot.image.color = colorLangNonSelect;
                }
            }
        }

        //change Font
        private Font GetFont()
        {
            for (int i = 0; i < availableLanguages.Length; i++)
            {
                if (availableLanguages[i].language == Localization.languageCurrent) return availableLanguages[i].font;
            }

            return availableLanguages[0].font;
        }

        private TMP_FontAsset GetFontTMP()
        {
            for (int i = 0; i < availableLanguages.Length; i++)
            {
                if (availableLanguages[i].language == Localization.languageCurrent) return availableLanguages[i].fontTMP;
            }

            return availableLanguages[0].fontTMP;
        }

        private void OnValidate()
        {
            for (int i = 0; i < availableLanguages.Length; i++)
            {
                availableLanguages[i].name = availableLanguages[i].language.ToString();
            }
        }
    }
}