using UnityEngine;

namespace IdleStrategyKit
{
    public class UIQuestsTracked : MonoBehaviour
    {
        public Transform content;
        public UIQuestsTrackedSlot prefab;

        // Update is called once per frame
        void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                // instantiate/destroy enough slots
                UIUtils.BalancePrefabs(prefab.gameObject, player.quests.questsTracked.Count, content);

                for (int i = 0; i < player.quests.questsTracked.Count; i++)
                {
                    UIQuestsTrackedSlot slot = content.transform.GetChild(i).GetComponent<UIQuestsTrackedSlot>();

                    //slot.image.sprite = player.quests.questsTracked[i].image;
                    slot.textDone.text = player.quests.QuestCompletedPersent(player.quests.questsTracked[i]) + " %";
                }
            }
        }
    }
}


