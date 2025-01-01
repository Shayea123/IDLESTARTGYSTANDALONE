using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIHeroes : MonoBehaviour
    {
        public GameObject panel;
        public Transform content;
        public UIHeroSlot prefab;
        public Button buttonClose;
        public Transform transformStandbyCharacters;
        public Transform transformUnobtainedCharacters;
        public GameObject panelStandbyCharacters;
        public GameObject panelUnobtainedCharacters;

        [Header("Hero description")]
        public GameObject panelDescription;
        public Image rarityInfo;
        public Image imageInfo;
        public Text textName;
        public TextMeshProUGUI textParts;
        public TextMeshProUGUI textBonuses;
        public TMP_Dropdown dropdownPlace;
        public Text textInfo;
        public Button buttonUpgrade;

        [Header("Components")]
        public UIAudio _audio;

        private int indexSelectedHero = -1;

        private void Start()
        {
            buttonClose.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                panel.SetActive(false);
            });
        }

        private void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    List<Hero> standbyCharacters = player.heroes.GetStandbyCharacters();
                    List<ScriptableHero> unobtainedCharacters = player.heroes.GetUnobtainedCharacters();

                    //sort by rarity
                    //List<ScriptableHero> heroes = new List<ScriptableHero>();
                    //for (int i = 0; i < player.heroes.heroes.Count; i++){heroes.Add(player.heroes.heroes[i].data);}
                    //heroes.Sort(delegate (ScriptableHero x, ScriptableHero y){return x.rarity.CompareTo(y.rarity);});

                    // instantiate/destroy enough slots
                    panelStandbyCharacters.SetActive(standbyCharacters.Count > 0);
                    UIUtils.BalancePrefabs(prefab.gameObject, standbyCharacters.Count, transformStandbyCharacters);
                    for (int i = 0; i < standbyCharacters.Count; i++)
                    {
                        UIHeroSlot slot = transformStandbyCharacters.transform.GetChild(i).GetComponent<UIHeroSlot>();
                        Hero hero = standbyCharacters[i];

                        slot.imageLevel.SetActive(true);
                        slot.textLevel.text = hero.level.ToString();
                        slot.image.color = Color.white;
                        slot.textName.text = hero.name;

                        //image and info
                        slot.image.sprite = hero.image;
                        slot.textPartsAmount.text = hero.parts + "/" + hero.data.needParts[hero.level];

                        //rarity color
                        slot.rarityImage.color = player.rarity.GetColor(hero.data);

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();

                            if (hero.parts >= hero.data.needParts[hero.level] || hero.level > 0)
                            {
                                indexSelectedHero = player.heroes.GetCharacterIndex(hero.data);
                                panelDescription.SetActive(true);

                                dropdownPlace.ClearOptions();
                                List<string> places = new List<string>() { "None" };

                                for (int i = 0; i < player.buildings.buildings.Count; i++)
                                {
                                    if (player.buildings.buildings[i].level > 0 && player.buildings.buildings[i].data.managerAvailable)
                                        places.Add(player.buildings.buildings[i].data.name);
                                }
                                dropdownPlace.AddOptions(places);
                            }
                        });
                    }

                    // instantiate/destroy enough slots
                    panelUnobtainedCharacters.SetActive(unobtainedCharacters.Count > 0);
                    UIUtils.BalancePrefabs(prefab.gameObject, unobtainedCharacters.Count, transformUnobtainedCharacters);
                    for (int i = 0; i < unobtainedCharacters.Count; i++)
                    {
                        UIHeroSlot slot = transformUnobtainedCharacters.transform.GetChild(i).GetComponent<UIHeroSlot>();

                        slot.imageLevel.SetActive(false);
                        slot.image.color = Color.black;
                        slot.textName.text = "";

                        //image and info
                        slot.image.sprite = unobtainedCharacters[i].image;
                        int heroIndex = player.heroes.GetCharacterIndex(unobtainedCharacters[i]);
                        if (heroIndex != -1)
                        {
                            Hero hero = player.heroes.heroes[heroIndex];

                            slot.textPartsAmount.text = hero.parts + "/" + hero.data.needParts[hero.level];

                            int icopy = i;
                            slot.button.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();

                                if (hero.parts >= hero.data.needParts[hero.level] || hero.level > 0)
                                {
                                    indexSelectedHero = heroIndex;
                                    panelDescription.SetActive(true);

                                    dropdownPlace.ClearOptions();
                                    List<string> places = new List<string>() { "None" };

                                    for (int i = 0; i < player.buildings.buildings.Count; i++)
                                    {
                                        if (player.buildings.buildings[i].level > 0 && player.buildings.buildings[i].data.managerAvailable)
                                            places.Add(player.buildings.buildings[i].data.name);
                                    }
                                    dropdownPlace.AddOptions(places);
                                }
                            });
                        }
                        else slot.textPartsAmount.text = "";
                    }

                    if (panelDescription.activeSelf && indexSelectedHero != -1)
                    {
                        Hero hero = player.heroes.heroes[indexSelectedHero];
                        textName.text = hero.data.name;
                        imageInfo.sprite = hero.data.image;
                        imageInfo.color = hero.level > 0 ? Color.white : Color.black;

                        textParts.text = hero.parts + "/" + hero.data.needParts[hero.level];

                        //rarity color
                        rarityInfo.gameObject.SetActive(true);
                        rarityInfo.color = player.rarity.GetColor(hero.data);

                        if (hero.level > 0)
                        {
                            dropdownPlace.interactable = true;
                            textBonuses.text = player.heroes.HeroBonuses(hero.data);
                        }
                        else
                        {
                            dropdownPlace.interactable = false;
                            textBonuses.text = "";
                        }

                        int ind = dropdownPlace.options.FindIndex(x => x.text == hero.placeId);
                        dropdownPlace.value = ind == -1 ? 0 : ind;
                        dropdownPlace.onValueChanged.SetListener(delegate
                        {
                            player.heroes.SetPlace(indexSelectedHero, dropdownPlace.options[dropdownPlace.value].text);
                        });

                        buttonUpgrade.interactable = player.heroes.CanUpgradeHero(indexSelectedHero);
                        buttonUpgrade.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            player.heroes.UpgradeHero(indexSelectedHero);
                        });
                    }
                }
                else panel.SetActive(false);
            }
        }
    }
}