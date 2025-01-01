using UnityEngine;

namespace IdleStrategyKit
{
    public class UIResourceTracked : MonoBehaviour
    {
        public Transform defaultItems;
        public Transform addedByPlayer;
        public UIResourceTrackedSlot prefab;

        [Header("Components")]
        public UIAudio _audio;

        // Update is called once per frame
        void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                if (player.TownMenuAvailable())
                {
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(prefab.gameObject, player.items.DefaultTrackedItems.Length, defaultItems);
                    for (int i = 0; i < player.items.DefaultTrackedItems.Length; i++)
                    {
                        UIResourceTrackedSlot slot = defaultItems.transform.GetChild(i).GetComponent<UIResourceTrackedSlot>();
                        slot.image.sprite = player.items.DefaultTrackedItems[i].image;

                        //if item is inhabitants = show free inhabitants
                        if (player.items.DefaultTrackedItems[i].Equals(player.inhabitants.scriptableItem)) slot.textAmount.text = player.inhabitants.InhabitantsFree().ToString();
                        else
                        {
                            slot.textAmount.text = UIUtils.LongToString(player.items.GetItemAmount(player.items.DefaultTrackedItems[i]) + player.items.GetItemShopAmount(player.items.DefaultTrackedItems[i]));
                        }

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            UIDescriptionPanel.singleton.ShowScriptableItem(player, player.items.DefaultTrackedItems[icopy]);
                        });
                    }

                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(prefab.gameObject, player.items.itemsTracked.Count, addedByPlayer);
                    for (int i = 0; i < player.items.itemsTracked.Count; i++)
                    {
                        UIResourceTrackedSlot slot = addedByPlayer.transform.GetChild(i).GetComponent<UIResourceTrackedSlot>();
                        slot.image.sprite = player.items.itemsTracked[i].image;

                        slot.textAmount.text = UIUtils.LongToString(player.items.GetItemAmount(player.items.itemsTracked[i]) + player.items.GetItemShopAmount(player.items.itemsTracked[i]));

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            UIDescriptionPanel.singleton.ShowScriptableItem(player, player.items.itemsTracked[icopy]);
                        });
                    }
                }
                else
                {
                    UIUtils.BalancePrefabs(prefab.gameObject, 0, addedByPlayer);
                }
            }
        }
    }
}

