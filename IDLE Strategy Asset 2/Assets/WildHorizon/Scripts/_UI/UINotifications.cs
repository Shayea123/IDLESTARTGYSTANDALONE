using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UINotifications : MonoBehaviour
    {
        public GameObject panel;
        public Transform content;
        public UINotificationsSlot prefab;
        public Sprite spriteNull;

        public Button buttonOpen;
        public Text textAmount;
        public Button buttonCloseAll;
        public Button buttonClose;

        [Header("Components")]
        public UIAudio _audio;

        private void Start()
        {
            buttonOpen.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                panel.SetActive(true);
            });

            buttonClose.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                panel.SetActive(false);
            });
        }

        private void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                //show or hide button
                if (player.notifications.notifications.Count > 0)
                {
                    buttonOpen.gameObject.SetActive(true);
                    textAmount.text = player.notifications.notifications.Count.ToString();
                }
                else buttonOpen.gameObject.SetActive(false);

                //only if panel is open
                if (panel.activeSelf)
                {
                    if (player.notifications.notifications.Count == 0) panel.SetActive(false);

                    buttonCloseAll.onClick.SetListener(() =>
                    {
                        _audio.PlaySoundButtonClick();
                        player.notifications.notifications = new List<Notifications>();
                        panel.SetActive(false);
                    });

                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(prefab.gameObject, player.notifications.notifications.Count, content);

                    //refresh all slots
                    for (int i = 0; i < player.notifications.notifications.Count; i++)
                    {
                        UINotificationsSlot slot = content.transform.GetChild(i).GetComponent<UINotificationsSlot>();

                        slot.image.sprite = player.notifications.notifications[i].image != null ? player.notifications.notifications[i].image : spriteNull;
                        slot.text.text = player.notifications.notifications[i].name;

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            panel.SetActive(false);
                            player.notifications.ShowInfo(player.notifications.notifications[icopy].type);
                        });

                        slot.buttonClose.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            player.notifications.notifications.Remove(player.notifications.notifications[icopy]);
                        });
                    }
                }
            }
        }
    }
}


