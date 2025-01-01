using System;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public partial class UILocalizationText : MonoBehaviour
    {
        public string key;
        public string specialSymbol;
        public Text text;

        private void OnEnable()
        {
            if (text == null) text = GetComponent<Text>();
        }

        public void Start()
        {
            //if key is null or empty
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) key = text.text;

            Translate();
            Localization.LocalizationChanged += Translate;
        }

        private void OnDestroy()
        {
            Localization.LocalizationChanged -= Translate;
        }

        private void Translate()
        {
            string temp = Localization.Translate(key);
            temp = temp.Replace("\\n", Environment.NewLine);
            text.text = temp + specialSymbol;
        }

        private void OnValidate()
        {
            if (text == null) text = GetComponent<Text>();
        }
    }
}


