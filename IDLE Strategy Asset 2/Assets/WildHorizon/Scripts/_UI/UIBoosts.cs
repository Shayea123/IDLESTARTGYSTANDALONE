using UnityEngine;

namespace IdleStrategyKit
{
    public class UIBoosts : MonoBehaviour
    {
        public GameObject panel;
        public Transform content;
        public UIBoostSlot prefab;

        public static bool showBoosts = false;

        // Update is called once per frame
        void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                if (showBoosts)
                {
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(prefab.gameObject, player.boosts.boosts.Count, content);

                    // refresh all slots
                    for (int i = 0; i < player.boosts.boosts.Count; i++)
                    {
                        UIBoostSlot slot = content.transform.GetChild(i).GetComponent<UIBoostSlot>();
                        Boost boost = player.boosts.boosts[i];

                        slot.image.sprite = boost.data.image;
                        slot.textName.text = boost.data.name;
                        string time = "";
                        //if (boosts[i].addHours > 0) time = boosts[i].addHours + "h";
                        //if (boosts[i].addMinutes > 0) time = time + " : " + boosts[i].addMinutes + "min";
                        if (boost.data.addSeconds > 0) time = time + " : " + boost.data.addSeconds + "sec";
                        slot.textInfo.text = boost.data.GetDescriptionByLanguage(Localization.languageCurrent) + " " + Utils.PrettySeconds(boost.data.addSeconds);

                        int icopy = i;
                        uint amount = player.boosts.GetAmount(boost.data);
                        if (amount > 0)
                        {
                            slot.textButtonFirst.text = "[" + amount + "]";

                            slot.buttonFirst.onClick.SetListener(() =>
                            {
                                player.boosts.CmdUseBoost(icopy);
                            });

                            slot.buttonSecond.gameObject.SetActive(boost.data.watchAd);
                            slot.buttonSecond.onClick.SetListener(() =>
                            {

                            });
                        }
                        else
                        {
                            slot.buttonFirst.gameObject.SetActive(player.boosts.boosts[icopy].data.buy);
                            slot.textButtonFirst.text = player.boosts.boosts[icopy].data.shopPrice.ToString();
                            slot.buttonFirst.onClick.SetListener(() =>
                            {
                                player.boosts.CmdBuyBoost(icopy);
                            });

                            slot.buttonSecond.gameObject.SetActive(player.boosts.boosts[icopy].data.watchAd);
                            slot.buttonSecond.onClick.SetListener(() =>
                            {

                            });
                        }
                    }
                }
            }
        }
    }
}


