using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIEnemyCamps : MonoBehaviour
    {
        public int pendingBattlesMaxAmount = 8;

        [Header("Instantiate on map")]
        public bool addToMap;
        public Transform[] locations;
        public GameObject battlefieldPrefab;

        [Header("")]
        public GameObject panel;
        public Transform content;
        public GameObject prefab;
        public Button buttonTrade;
        public Button buttonAttack;

        [Header("Colors")]
        public Gradient colorGradient;
        public Color colorSelected;
        public Color colorDefault;

        [Header("Image Slot")]
        public Image imageRarity;
        public Image imageLogo;

        [Header("State")]
        public Transform stateContent;
        public GameObject statePrefab;
        public Text textState;

        [Header("Army")]
        public Transform enemyArmyContent;
        public GameObject enemyArmyPrefab;

        [Header("Trade Items")]
        public Transform tradeItemsContent;
        public GameObject tradeItemsPrefab;

        [Header("Required Items")]
        public Transform requiredItemsContent;
        public GameObject requiredItemsPrefab;

        [Header("Panel Error")]
        public GameObject panelError;
        public Text textErrorInfo;
        public Button buttonBuildingBuild;

        [Header("Components")]
        public UIAudio _audio;
        public GameObject panelTrade;

        public void Show()
        {
            _audio.PlaySoundButtonClick();
            panel.SetActive(true);
        }

        public static UIEnemyCamps singleton;
        public UIEnemyCamps()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            if (singleton == null) singleton = this;
        }

        // Update is called once per frame
        void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                //показываем на карте лагеря соперников
                if (player.accessToTheBattles.item == null || player.buildings.buildings[player.buildings.FindIndex(player.accessToTheBattles.item)].level >= player.accessToTheBattles.requiredBuildingLevel)
                {
                    for (int i = 0; i < player.camps.enemyCamps.Count; i++)
                    {
                        if (player.camps.enemyCamps[i].location != -1)
                        {
                            UIUtils.BalancePrefabs(battlefieldPrefab, 1, locations[player.camps.enemyCamps[i].location]);
                            Battlefield slot = locations[player.camps.enemyCamps[i].location].GetChild(0).GetComponent<Battlefield>();

                            slot.spriteLogo.sprite = player.camps.enemyCamps[i].data.image;
                            slot.spriteColor.color = colorGradient.Evaluate(player.camps.FindFactionRelationshipLevel(player.camps.enemyCamps[i].data.factionType));
                            slot.index = i;
                        }
                    }
                }

                if (panel.activeSelf)
                {
                    // instantiate/destroy enough slots - all active camps
                    UIUtils.BalancePrefabs(prefab, player.camps.enemyCamps.Count, content);
                    for (int i = 0; i < player.camps.enemyCamps.Count; i++)
                    {
                        UIEnemyCampSlot slot = content.transform.GetChild(i).GetComponent<UIEnemyCampSlot>();

                        //image
                        slot.imageRelations.color = colorGradient.Evaluate(player.camps.FindFactionRelationshipLevel(player.camps.enemyCamps[i].data.factionType));
                        slot.image.sprite = player.camps.enemyCamps[i].data.image;

                        //name
                        slot.textName.text = Localization.Translate(player.camps.enemyCamps[i].data.name);

                        slot.textName.color = player.camps.enemyCamps[i]._hash == player.camps.selectedEnemyCamp._hash ? colorSelected : colorDefault;

                        //state
                        if (player.camps.enemyCamps[i].state != CampState.none)
                        {
                            slot.textType.text = Localization.Translate(player.camps.enemyCamps[i].state.ToString());
                            slot.textTime.text = Utils.TimeBySeconds(player.camps.enemyCamps[i].actionEndTime);
                        }
                        else
                        {
                            slot.textType.text = "";
                            slot.textTime.text = "";
                        }

                        //button
                        int icopy = i;
                        slot.button.interactable = player.TownMenuAvailable();
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();

                            player.camps.selectedEnemyCamp = player.camps.enemyCamps[icopy];
                        });
                    }

                    //show camp description
                    if (player.camps.GetCampIndexByHash(player.camps.selectedEnemyCamp._hash) != -1)
                    {
                        //buttonAttack.gameObject.SetActive(false);
                        buttonAttack.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();

                        });

                        buttonTrade.onClick.SetListener(() =>
                        {
                            if (player.accessToTheCampTrade.item == null || player.buildings.buildings[player.buildings.FindIndex(player.accessToTheCampTrade.item)].level > 0)
                            {
                                player.camps.CmdCreateNullList();
                                _audio.PlaySoundButtonClick();
                                panelTrade.SetActive(true);
                                panel.SetActive(false);
                            }
                            else
                            {
                                textErrorInfo.text = Localization.Translate("TradeRequired") + "\n" + Localization.Translate(player.accessToTheCampTrade.item.name);
                                panelError.SetActive(true);
                            }
                        });

                        buttonBuildingBuild.onClick.SetListener(() =>
                        {
                            panelError.SetActive(false);
                            panel.SetActive(false);

                        //UIBuildingSelect.selectedBuilding = player.accessToTheCampTrade.item;
                        //UIBuild.singleton.ShowBuildingFromOtherPanels();
                    });

                        //image
                        imageLogo.sprite = player.camps.selectedEnemyCamp.data.image;
                        imageRarity.color = player.rarity.GetColorForEmptySlot();

                        //state
                        if (player.camps.selectedEnemyCamp.state != CampState.none)
                        {
                            stateContent.gameObject.SetActive(true);
                            textState.text = Localization.Translate(player.camps.selectedEnemyCamp.state.ToString());

                            // instantiate/destroy enough slots : Trade Progress Items
                            UIUtils.BalancePrefabs(statePrefab, player.camps.selectedEnemyCamp.barterItems.Length + 1, stateContent);
                            for (int i = 0; i < player.camps.selectedEnemyCamp.barterItems.Length; i++)
                            {
                                UITradeCampItemSlot slot = stateContent.transform.GetChild(i + 1).GetComponent<UITradeCampItemSlot>();

                                //image
                                slot.image_1.sprite = player.camps.selectedEnemyCamp.barterItems[i].slot1.item.image;
                                slot.image_2.sprite = player.camps.selectedEnemyCamp.barterItems[i].slot2.item.image;

                                //name
                                slot.text_1_name.text = Localization.Translate(player.camps.selectedEnemyCamp.barterItems[i].slot1.item.name);
                                slot.text_2_name.text = Localization.Translate(player.camps.selectedEnemyCamp.barterItems[i].slot2.item.name);

                                //amount
                                slot.text_1_amount.text = player.camps.selectedEnemyCamp.barterItems[i].slot1.amount.ToString();
                                slot.text_2_amount.text = player.camps.selectedEnemyCamp.barterItems[i].slot2.amount.ToString();
                            }
                        }
                        else stateContent.gameObject.SetActive(false);

                        // instantiate/destroy enough slots : Army
                        int amount = 1 + player.camps.selectedEnemyCamp.data.army.Length;
                        if (player.camps.selectedEnemyCamp.data.hero != null)
                        {
                            amount += 1;
                            // instantiate/destroy enough slots
                            UIUtils.BalancePrefabs(enemyArmyPrefab, amount, enemyArmyContent);

                            UIQuestRewardSlot slot = enemyArmyContent.transform.GetChild(1).GetComponent<UIQuestRewardSlot>();

                            slot.image.sprite = player.camps.selectedEnemyCamp.data.hero.image;

                            //name
                            slot.textName.text = Localization.Translate(player.camps.selectedEnemyCamp.data.hero.name);

                            //amount
                            slot.textAmount.text = "1";

                            slot.buttonDescription.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                            });
                        }
                        else
                        {
                            UIUtils.BalancePrefabs(enemyArmyPrefab, amount, enemyArmyContent);
                        }

                        for (int i = 0; i < player.camps.selectedEnemyCamp.data.army.Length; i++)
                        {
                            UIQuestRewardSlot slot = enemyArmyContent.transform.GetChild((amount - player.camps.selectedEnemyCamp.data.army.Length) + i).GetComponent<UIQuestRewardSlot>();

                            slot.image.sprite = player.camps.selectedEnemyCamp.data.army[i].item.image;

                            //name
                            slot.textName.text = Localization.Translate(player.camps.selectedEnemyCamp.data.army[i].item.name);

                            //amount
                            slot.textAmount.text = player.camps.selectedEnemyCamp.data.army[i].amount.ToString();

                            int icopy = i;
                            slot.buttonDescription.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                            });
                        }

                        // instantiate/destroy enough slots : Trade Items
                        UIUtils.BalancePrefabs(tradeItemsPrefab, player.camps.selectedEnemyCamp.tradeItems.Length + 1, tradeItemsContent);
                        for (int i = 0; i < player.camps.selectedEnemyCamp.tradeItems.Length; i++)
                        {
                            UIQuestRewardSlot slot = tradeItemsContent.transform.GetChild(i + 1).GetComponent<UIQuestRewardSlot>();

                            slot.image.sprite = player.camps.selectedEnemyCamp.tradeItems[i].item.image;

                            //name
                            slot.textName.text = Localization.Translate(player.camps.selectedEnemyCamp.tradeItems[i].item.name);

                            //amount
                            slot.textAmount.text = player.camps.selectedEnemyCamp.tradeItems[i].amount.ToString();

                            int icopy = i;
                            slot.buttonDescription.onClick.SetListener(() =>
                            {
                                _audio.PlaySoundButtonClick();
                                UIDescriptionPanel.singleton.ShowScriptableItem(player, player.camps.selectedEnemyCamp.tradeItems[icopy].item.data);
                            });
                        }

                        // instantiate/destroy enough slots : Required Items
                        /*UIUtils.BalancePrefabs(requiredItemsPrefab, player.camps.selectedEnemyCamp.requiredItems.Length + 1, requiredItemsContent);
                        for (int i = 0; i < player.camps.selectedEnemyCamp.requiredItems.Length; i++)
                        {
                            UIQuestRewardSlot slot = requiredItemsContent.transform.GetChild(i+1).GetComponent<UIQuestRewardSlot>();

                            //image
                            slot.image.sprite = player.camps.selectedEnemyCamp.requiredItems[i].image;

                            //name
                            slot.textName.text = Localization.Translate(player.camps.selectedEnemyCamp.requiredItems[i].name);
                            slot.textName.font = Localization.fontClassic;

                            //amount
                            uint itemamount = player.items.GetItemAmount(player.camps.selectedEnemyCamp.requiredItems[i].data);
                            slot.textAmount.text = itemamount.ToString();
                            if (itemamount < 1) slot.textAmount.color = Color.red;
                            else slot.textAmount.color = Color.black;
                        }*/
                    }
                    else
                    {
                        if (player.camps.enemyCamps.Count > 0) player.camps.selectedEnemyCamp = player.camps.enemyCamps[0];
                    }
                }
            }
            else panel.SetActive(false);
        }
    }
}


