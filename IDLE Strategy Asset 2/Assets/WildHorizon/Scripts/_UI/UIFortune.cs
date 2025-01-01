using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public enum FortuneType { normal, rare, legendary }

    public class UIFortune : MonoBehaviour
    {
        FortuneType type = FortuneType.normal;
        public Color colorNormal;

        private int numberOfTurns;
        private int whatWeWin;

        public float startSpeed = 0.2f;
        private float speed;
        private bool canWeTurn = true;

        [Header("UI Elements")]
        public GameObject panel;
        public Transform content;

        public Button buttonTurn;
        public Button buttonTurnAd;

        public Button buttonNormal;
        public Button buttonRare;
        public Button buttonLegendary;

        public GameObject panelTurn;
        public GameObject whill;

        [Header("Win info")]
        public GameObject panelWhatWin;
        public Text textWinning;
        public Text textAmount;
        public Image imageLogo;

        [Header("Error")]
        public GameObject panelError;

        [Header("Components")]
        public UIAudio _audio;

        public static UIFortune singleton;
        public UIFortune()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            if (singleton == null) singleton = this;
        }

        private void Start()
        {
            buttonNormal.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                type = FortuneType.normal;
            });
            buttonRare.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                type = FortuneType.rare;
            });
            buttonLegendary.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                type = FortuneType.legendary;
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    //turn for coins
                    buttonTurn.interactable = canWeTurn;
                    buttonTurn.GetComponentInChildren<Text>().text = player.fortune.rewards.rewards[(int)type].itemForTurn.amount.ToString();
                    buttonTurn.onClick.SetListener(() =>
                    {
                        if (player.items.GetItemAmount(player.fortune.rewards.rewards[(int)type].itemForTurn.item) >= player.fortune.rewards.rewards[(int)type].itemForTurn.amount)
                        {
                            textWinning.text = "";
                            panelTurn.SetActive(true);
                            _audio.PlaySoundButtonClick();
                            canWeTurn = false;
                            player.fortune.CmdTryTurnForItem((int)type);
                        }
                        else
                        {
                            panelError.SetActive(true);
                        }
                    });

                    //turn for ad
                    TimeSpan ts = player.timeLastShowAdsForFortune.AddMinutes(player.fortune.timeAdRefresh) - DateTime.UtcNow;
                    if (ts.TotalSeconds > 0)
                    {
                        buttonTurnAd.GetComponentInChildren<Text>().text = ts.Minutes + ":" + ts.Seconds;
                        buttonTurnAd.interactable = false;
                    }
                    else
                    {
                        buttonTurnAd.GetComponentInChildren<Text>().text = Localization.Translate("Watch Ad");
                        buttonTurnAd.onClick.SetListener(() =>
                        {
                            player.timeLastShowAdsForFortune = DateTime.UtcNow.AddMinutes(player.fortune.timeAdRefresh);
                            //GameADS.singleton.ShowRewardedAd(AdsType.fortune);
                        });

                        //buttonTurnAd.interactable = Advertisement.IsReady(GameADS.singleton.rewardedVideo) && canWeTurn && player.fortune.rewards[(int)type].turnForAd;
                    }

                    ScriptableItemAndAmount[] list = player.fortune.rewards.rewards[(int)type].items;

                    // refresh all slots
                    for (int i = 0; i < list.Length; i++)
                    {
                        UIFortuneSlot slot = content.GetChild(i).GetComponent<UIFortuneSlot>();

                        slot.textName.text = list[i].item.name;

                        if (list[i].item is ScriptableHero hero)
                        {
                            slot.image.sprite = hero.imagePart;
                            slot.imageRarity.color = player.rarity.GetColor(hero);
                            slot.textAmount.text = list[i].amount.ToString();
                        }
                        else
                        {
                            slot.image.sprite = list[i].item.image;
                            slot.imageRarity.color = colorNormal;
                            slot.textAmount.text = UIUtils.LongToString(list[i].amount);
                        }

                        int icopy = i;
                        slot.buttonDescription.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            UIDescriptionPanel.singleton.ShowScriptableItem(player, list[icopy].item);
                        });
                    }
                }
            }
        }

        public void TurnForAds()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                textWinning.text = "";
                panelTurn.SetActive(true);
                _audio.PlaySoundButtonClick();
                canWeTurn = false;
                StartCoroutine(TurnTheWheel());
                player.fortune.CmdSetTimeAds();
            }
        }

        private IEnumerator TurnTheWheel()
        {
            yield return new WaitForSeconds(0.5f);
            numberOfTurns = UnityEngine.Random.Range(60, 120);

            speed = startSpeed;
            for (int i = 0; i < numberOfTurns; i++)
            {
                whill.transform.Rotate(0, 0, 20f);

                if (i > Mathf.RoundToInt(numberOfTurns * 0.4f))
                {
                    speed = 0.04f;
                }
                if (i > Mathf.RoundToInt(numberOfTurns * 0.6f))
                {
                    speed = 0.06f;
                }
                if (i > Mathf.RoundToInt(numberOfTurns * 0.8f))
                {
                    speed = 0.08f;
                }
                if (i > Mathf.RoundToInt(numberOfTurns * 0.9f))
                {
                    speed = 0.1f;
                }

                yield return new WaitForSeconds(speed);
            }

            if (Mathf.RoundToInt(whill.transform.eulerAngles.z) % 40 != 0)
            {
                whill.transform.Rotate(0, 0, 20f);
            }

            whatWeWin = Mathf.RoundToInt(whill.transform.eulerAngles.z);

            Player player = Player.localPlayer;

            switch (whatWeWin)
            {
                case 0:
                    yield return new WaitForSeconds(1);
                    player.fortune.CmdAddReward((int)type, 0);
                    break;

                case 40:
                    yield return new WaitForSeconds(1);
                    player.fortune.CmdAddReward((int)type, 8);
                    break;

                case 80:
                    yield return new WaitForSeconds(1);
                    player.fortune.CmdAddReward((int)type, 7);
                    break;

                case 120:
                    yield return new WaitForSeconds(1);
                    player.fortune.CmdAddReward((int)type, 6);
                    break;

                case 160:
                    yield return new WaitForSeconds(1);
                    player.fortune.CmdAddReward((int)type, 5);
                    break;

                case 200:
                    yield return new WaitForSeconds(1);
                    player.fortune.CmdAddReward((int)type, 4);
                    break;

                case 240:
                    yield return new WaitForSeconds(1);
                    player.fortune.CmdAddReward((int)type, 3);
                    break;

                case 280:
                    yield return new WaitForSeconds(1);
                    player.fortune.CmdAddReward((int)type, 2);
                    break;

                case 320:
                    yield return new WaitForSeconds(1);
                    player.fortune.CmdAddReward((int)type, 1);
                    break;
            }

            StartCoroutine(CloseTheWheel());
        }

        private IEnumerator CloseTheWheel()
        {
            yield return new WaitForSeconds(1.0f);
            panelTurn.SetActive(false);
            canWeTurn = true;
        }

        public void OpenPanelWhatBuying(string itemName, int amount, Sprite logo)
        {
            panelTurn.SetActive(false);
            textWinning.text = itemName;
            textAmount.text = Localization.Translate("Amount") + " : " + amount;
            imageLogo.sprite = logo;
            panelWhatWin.SetActive(true);
        }

        public void TurnForItem()
        {
            StartCoroutine(TurnTheWheel());
        }
    }
}


