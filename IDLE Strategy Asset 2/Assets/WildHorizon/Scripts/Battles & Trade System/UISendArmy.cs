using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UISendArmy : MonoBehaviour
    {
        public GameObject panel;
        public Transform content;
        public GameObject prefab;
        public Button buttonSend;

        [Header("Components")]
        public GameObject panelBattleIsOver;
        public GameObject panelBattles;
        public UIAudio _audio;

        // Update is called once per frame
        void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                if (panel.activeSelf)
                {
                    List<ItemSlot> list = player.items.GetItemsListByType(new ScriptableWeapon());

                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(prefab, list.Count, content);

                    for (int i = 0; i < list.Count; i++)
                    {
                        UIArmySlot slot = content.transform.GetChild(i).GetComponent<UIArmySlot>();

                        slot.image.sprite = list[i].item.data.image;

                        //name
                        slot.textName.text = Localization.Translate(list[i].item.name);

                        //amount
                        //slot.textAmount.text = slot.amount + "/" + list[i].amount;

                        //item
                        slot.item = (ScriptableWeapon)list[i].item.data;

                        int icopy = i;
                        //slot.buttonPlus.interactable = slot.amount < list[i].amount && Global.InhabitantsFree() > slot.amount;
                        slot.buttonPlus.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            slot.amount++;
                        });

                        slot.buttonMinus.interactable = slot.amount > 0;
                        slot.buttonMinus.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            slot.amount--;
                        });
                    }

                    buttonSend.interactable = CanSend();
                    buttonSend.onClick.SetListener(() =>
                    {
                        _audio.PlaySoundButtonClick();
                    //SendArmy(player);
                    panel.SetActive(false);
                        panelBattles.SetActive(true);
                    });

                    //if this camp is disappeared
                    if (player.camps.GetCampIndexByHash(player.camps.selectedEnemyCamp._hash) != -1)
                    {
                        panelBattleIsOver.SetActive(true);
                        panel.SetActive(false);
                    }
                }
            }

        }

        void SendArmy(Player player)
        {
            int index = player.camps.GetCampIndexByHash(player.camps.selectedEnemyCamp._hash);
            if (index != -1)
            {
                EnemyCamp camp = player.camps.enemyCamps[index];
                camp.state = CampState.War;
                //camp.sendArmy = GetSendArmy();
                camp.actionEndTime = 60;

                player.camps.enemyCamps[index] = camp;
            }
        }

        List<ScriptableItemAndAmount> GetSendArmy()
        {
            List<ScriptableItemAndAmount> list = new List<ScriptableItemAndAmount>();

            for (int i = 0; i < content.transform.childCount; i++)
            {
                UIArmySlot slot = content.transform.GetChild(i).GetComponent<UIArmySlot>();
                if (slot.amount > 0)
                {
                    ScriptableItemAndAmount temp = new ScriptableItemAndAmount();
                    temp.item = slot.item;
                    temp.amount = slot.amount;
                    list.Add(temp);
                }
            }

            return list;
        }

        bool CanSend()
        {
            for (int i = 0; i < content.transform.childCount; i++)
            {
                UIArmySlot slot = content.transform.GetChild(i).GetComponent<UIArmySlot>();
                if (slot.amount > 0) return true;
            }

            return false;
        }
    }
}


