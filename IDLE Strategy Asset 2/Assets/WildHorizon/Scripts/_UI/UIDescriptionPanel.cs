using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIDescriptionPanel : MonoBehaviour
    {
        public GameObject panel;
        public Image image;
        public Text textItemName;
        public Text textDescription;
        public TextMeshProUGUI textAmount;
        public GameObject panelAmount;
        public Button buttonClose;

        [Header("Components")]
        public UIAudio _audio;

        public static UIDescriptionPanel singleton;
        public UIDescriptionPanel()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            //if (singleton == null) 
            singleton = this;
        }

        private void Start()
        {
            buttonClose.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                panel.SetActive(false);
            });
        }

        public void ShowScriptableItem(Player player, ScriptableItem item)
        {
            image.sprite = item.image;

            //amount
            textAmount.text = UIUtils.LongToString(player.items.GetItemAmount(item)).ToString();
            textItemName.text = Localization.Translate(item.name);

            //description
            textDescription.text = item.GetDescriptionByLanguage(Localization.languageCurrent);

            panel.SetActive(true);
        }
        public void ShowScriptableArmy(Player player, ScriptableArmy army)
        {
            image.sprite = army.image;

            //amount
            textAmount.text = UIUtils.LongToString(player.army.GetArmyAmount(army)).ToString();

            textItemName.text = army.name;

            //description
            textDescription.text = army.GetDescriptionByLanguage(Localization.languageCurrent);

            panel.SetActive(true);
        }
        public void ShowScriptableBuilding(ScriptableBuilding building)
        {
            image.sprite = building.spriteForPreview[0];

            //amount
            textAmount.text = "";
            panelAmount.SetActive(false);

            //name
            textItemName.text = Localization.Translate(building.name);

            //Description
            textDescription.text = building.GetDescriptionByLanguage(Localization.languageCurrent);

            panel.SetActive(true);
        }
        public void ShowScriptableResearch(ScriptableResearch research)
        {
            image.sprite = research.sprite;

            //amount
            textAmount.text = "";
            textItemName.text = Localization.Translate(research.name);
            textDescription.text = research.GetDescriptionByLanguage(Localization.languageCurrent);

            panel.SetActive(true);
        }
    }
}


