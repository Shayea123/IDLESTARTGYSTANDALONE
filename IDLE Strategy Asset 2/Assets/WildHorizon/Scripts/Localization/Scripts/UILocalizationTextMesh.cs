using System;
using TMPro;
using UnityEngine;

namespace IdleStrategyKit
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public partial class UILocalizationTextMesh : MonoBehaviour
    {
        public string key;
        public string specialSymbol;
        public TextMeshProUGUI text;

        private void OnEnable()
        {
            if (text == null) text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
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
    }
}


