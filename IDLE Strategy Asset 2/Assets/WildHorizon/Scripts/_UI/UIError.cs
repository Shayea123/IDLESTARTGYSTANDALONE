using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIError : MonoBehaviour
    {
        public GameObject panel;
        public Text textErrorCause;
        public Transform content;
        public GameObject prefab;
        public Text textDescription;

        [Header("Components")]
        public UIAudio _audio;

        public static UIError singleton;
        public UIError()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            //if (singleton == null) 
            singleton = this;
        }

        public void ShowTextError(string errorCause)
        {
            //textErrorCause.text = errorCause;

            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(prefab, 0, content);

            textDescription.text = errorCause;

            panel.SetActive(true);
        }

        public void ShowScriptableItemAndAmount(string errorCause, ScriptableItemAndAmount item, string description)
        {
            textErrorCause.text = errorCause;

            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(prefab, 1, content);

            //refresh slot
            UISlot slot = content.transform.GetChild(0).GetComponent<UISlot>();

            //image
            slot.image.sprite = item.item.image;

            //amount
            //slot.textAmount.text = Global.GetItemAmount(item.item).ToString();
            slot.textAmount.text = "0";
            slot.panelAmount.SetActive(true);

            slot.button.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();
                //UIDescriptionPanel.singleton.ShowScriptableItem(item.item);
            });

            textDescription.text = description;

            panel.SetActive(true);
        }
        public void ShowListScriptableItemAndAmount(string errorCause, ScriptableItemAndAmount[] list, string description)
        {
            textErrorCause.text = errorCause;

            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(prefab, list.Length, content);

            //refresh all slots
            for (int i = 0; i < list.Length; i++)
            {
                UISlot slot = content.transform.GetChild(i).GetComponent<UISlot>();

                //image
                slot.image.sprite = list[i].item.image;

                //amount
                //uint amount = Global.GetItemAmount(list[i].item);
                uint amount = 0;
                //if ()
                slot.textAmount.text = UIUtils.LongToString(amount) + " / " + list[i].amount;
                slot.panelAmount.SetActive(true);
                if (amount < list[i].amount) slot.textAmount.color = Color.red;
                else slot.textAmount.color = Color.white;

                int icopy = i;
                slot.button.onClick.SetListener(() =>
                {
                    _audio.PlaySoundButtonClick();
                    //UIDescriptionPanel.singleton.ShowScriptableItem(list[icopy].item);
                });
            }

            textDescription.text = description;

            panel.SetActive(true);
        }

        public void ShowScriptableBuildingAndAmountOrLevel(string errorCause, ScriptableBuildingAndAmountOrLevel item, string description)
        {
            textErrorCause.text = errorCause;

            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(prefab, 1, content);

            UISlot slot = content.transform.GetChild(0).GetComponent<UISlot>();

            //image
            slot.image.sprite = item.item.spriteForPreview[0];

            //amount
            slot.textAmount.text = "";
            slot.panelAmount.SetActive(false);

            slot.button.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();
                UIDescriptionPanel.singleton.ShowScriptableBuilding(item.item);
            });


            textDescription.text = description;

            panel.SetActive(true);
        }
        public void ShowListScriptableBuildingAndAmountOrLevel(string errorCause, ScriptableBuildingAndAmountOrLevel[] list, string description)
        {
            textErrorCause.text = errorCause;

            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(prefab, list.Length, content);

            //refresh all slots
            for (int i = 0; i < list.Length; i++)
            {
                UISlot slot = content.transform.GetChild(i).GetComponent<UISlot>();

                //image
                slot.image.sprite = list[i].item.spriteForPreview[0];

                //amount
                slot.textAmount.text = "";
                slot.panelAmount.SetActive(false);

                int icopy = i;
                slot.button.onClick.SetListener(() =>
                {
                    _audio.PlaySoundButtonClick();
                    UIDescriptionPanel.singleton.ShowScriptableBuilding(list[icopy].item);
                });
            }

            textDescription.text = description;

            panel.SetActive(true);
        }

        public void ShowListScriptableResearchAndLevel(string errorCause, ScriptableResearchAndLevel[] list, string description)
        {
            textErrorCause.text = errorCause;

            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(prefab, list.Length, content);

            //refresh all slots
            for (int i = 0; i < list.Length; i++)
            {
                UISlot slot = content.transform.GetChild(i).GetComponent<UISlot>();

                //image
                slot.image.sprite = list[i].item.sprite;

                //amount
                slot.textAmount.text = "";
                slot.panelAmount.SetActive(false);

                int icopy = i;
                slot.button.onClick.SetListener(() =>
                {
                    _audio.PlaySoundButtonClick();
                    UIDescriptionPanel.singleton.ShowScriptableResearch(list[icopy].item);
                });
            }

            textDescription.text = description;

            panel.SetActive(true);
        }
    }
}


