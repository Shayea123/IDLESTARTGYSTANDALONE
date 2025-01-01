using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public class PlayerQuests : MonoBehaviour
    {
        [Header("Components")]
        public Player player;
        public PlayerItems items;
        public PlayerBuildings buildings;
        public PlayerResearches researches;

        [HideInInspector] public List<Quest> quests = new List<Quest>();
        [HideInInspector] public List<ScriptableQuest> questsTracked = new List<ScriptableQuest>();

        public void CreateNewlist()
        {
            quests.Clear();

            foreach (var item in ScriptableQuest.dict)
            {
                if (item.Value is ScriptableQuest quest)
                    quests.Add(new Quest(quest, 0, false));
            }
        }

        private int FindIndexByName(string quest)
        {
            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i].data.name.Equals(quest)) return i;
            }

            return -1;
        }

        public void SetCompleted(string questname)
        {
            int index = FindIndexByName(questname);
            if (index != -1)
            {
                Quest quest = quests[index];
                quest.fullfilled = 100;
                quest.completed = true;
                quests[index] = quest;

                for (int i = 0; i < quest.data.rewardItems.Count; i++)
                {
                    if (quest.data.rewardItems[i].item.Equals(player.inhabitants.scriptableItem))
                    {
                        player.inhabitants.AddCurrent(quest.data.rewardItems[i].amount);
                    }
                    else items.IncreaseItemAmount(quest.data.rewardItems[i].item, quest.data.rewardItems[i].amount);
                }

                CheckAllQuestEvent();
            }
        }

        public void CheckAllQuestEvent()
        {
            for (int i = 0; i < quests.Count; i++)
            {
                if (!quests[i].completed && CheckQuestPredecessor(quests[i].data))
                {
                    bool succes = true;

                    //check items in stock
                    for (int y = 0; y < quests[i].data.items.Length; y++)
                    {
                        if (items.GetItemAmount(quests[i].data.items[y].item) < quests[i].data.items[y].amount)
                        {
                            succes = false;
                            break;
                        }
                    }

                    if (succes)
                    {
                        //check sell

                        //check buy
                    }

                    //check buildings
                    if (succes)
                    {
                        for (int y = 0; y < quests[i].data.buildings.Length; y++)
                        {
                            int itemIndex = buildings.buildings.FindIndex(x => x.data.name == quests[i].data.buildings[y].item.name);
                            if (itemIndex == -1 || buildings.buildings[itemIndex].level == 0)
                            {
                                succes = false;
                                break;
                            }
                        }
                    }

                    //check research
                    if (succes)
                    {
                        for (int y = 0; y < quests[i].data.researches.Length; y++)
                        {
                            int itemIndex = researches.FindIndex(quests[i].data.researches[y]);
                            if (itemIndex == -1)
                            {
                                succes = false;
                                break;
                            }
                            else
                            {
                                if (researches.researches[itemIndex].level > 0)
                                {
                                    succes = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (succes && quests[i].data.renameTown && player.townName == "")
                    {
                        succes = false;
                    }

                    if (succes)
                    {
                        if (quests[i].data.nextPart == null)
                        {
                            player.notifications.RpcAddNotification(Localization.Translate("Quest is Complete") + ": " + Localization.Translate(quests[i].data.name), NotificationsType.quest);
                        }
                        else
                        {
                            //if this is story quest
                            if (quests[i].data.questType == QuestType.story)
                            {
                                //update quest
                                Quest temp = quests[i];
                                temp.completed = true;
                                quests[i] = temp;

                                //CheckQuestsOnCompleted();
                            }
                        }
                    }
                }
            }
        }

        private bool CheckQuestPredecessor(ScriptableQuest quest)
        {
            if (quest.predecessor == null) return true;
            else
            {
                for (int i = 0; i < quests.Count; i++)
                {
                    if (quests[i].data.Equals(quest.predecessor))
                    {
                        return quests[i].completed;
                    }
                }

                return false;
            }
        }

        public float QuestCompletedPersent(ScriptableQuest quest)
        {
            float percent = 0;
            int value = quest.items.Length + quest.buildings.Length + quest.researches.Length;

            //check workers in town
            if (quest.sendWorkers != null && quest.sendWorkers.Length > 0) value += quest.sendWorkers.Length;

            if (quest.renameTown) value += 1;

            float forOne = value == 0 ? 0 : ((float)100 / value);

            //check items in stock
            if (quest.items.Length > 0)
            {
                for (int i = 0; i < quest.items.Length; i++)
                {
                    uint amountInStock = items.GetItemAmount(quest.items[i].item);
                    if (quest.items[i].amount <= amountInStock) percent += forOne;
                    else percent += forOne / 100 * (amountInStock * 100 / quest.items[i].amount);
                }
            }

            //check buildings
            if (quest.buildings != null)
            {
                for (int y = 0; y < quest.buildings.Length; y++)
                {
                    int itemIndex = buildings.FindIndex(quest.buildings[y].item);
                    if (itemIndex != -1 && buildings.buildings[itemIndex].level > 0) percent += forOne;
                }
            }

            //check research
            if (quest.researches != null)
            {
                for (int y = 0; y < quest.researches.Length; y++)
                {
                    int itemIndex = researches.FindIndex(quest.researches[y]);
                    if (itemIndex == -1 && researches.researches[itemIndex].level > 0) percent += forOne;
                }
            }

            //check heroes

            //check town rename 
            if (quest.renameTown && player.townName != "") percent += forOne;

            //check workers in town
            for (int i = 0; i < quest.sendWorkers.Length; i++)
            {
                uint amount = player.inhabitants.GetSendWorkersAmountByType(quest.sendWorkers[i].type);
                if (amount >= quest.sendWorkers[i].inhabitants) percent += forOne;
            }

            if (forOne == 0) return 100;
            else return percent;
        }

        public ScriptableQuest FindCurrentStoryQuest()
        {
            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i].data.questType == QuestType.story && !quests[i].completed && CheckQuestPredecessor(quests[i].data))
                {
                    return quests[i].data;
                }
            }

            return null;
        }
        public ScriptableQuest FindNextStoryQuest()
        {
            //
            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i].data.questType == QuestType.story && CheckQuestPredecessor(quests[i].data) && !quests[i].completed)
                {
                    return quests[i].data;
                }
            }

            return null;
        }
        public List<Quest> AllQuestsNotCompleted()
        {
            List<Quest> list = new List<Quest>();
            for (int i = 0; i < quests.Count; i++)
            {
                if (!quests[i].completed && CheckQuestPredecessor(quests[i].data))
                {
                    list.Add(quests[i]);
                }
            }

            return list;
        }

        public List<QuestTask> GetTaskForQuest(ScriptableQuest quest)
        {
            List<QuestTask> list = new List<QuestTask>();
            bool completed = false;

            //items, buildings, researches
            for (int i = 0; i < quest.items.Length; i++)
            {
                completed = false;
                if (items.GetItemAmount(quest.items[i].item) > quest.items[i].amount) completed = true;

                list.Add(new QuestTask(quest.items[i].item.image, Localization.Translate(quest.items[i].item.name) + " : " + quest.items[i].amount, completed, QuestTaskType.resources, quest.items[i].item));
            }
            for (int i = 0; i < quest.buildings.Length; i++)
            {
                completed = false;
                int index = buildings.buildings.FindIndex(x => x.data.name == quest.buildings[i].item.name);
                if (index != -1 && buildings.buildings[index].level > 0) completed = true;

                list.Add(new QuestTask(quest.buildings[i].item.spriteForPreview[0], Localization.Translate("Build") + " : " + Localization.Translate(quest.buildings[i].item.name), completed, QuestTaskType.buildings, quest.buildings[i].item));
            }
            for (int i = 0; i < quest.researches.Length; i++)
            {
                completed = false;
                int index = player.researches.researches.FindIndex(x => x.name == quest.researches[i].name);
                if (index != -1 && player.researches.researches[index].level > 0) completed = true;

                list.Add(new QuestTask(quest.researches[i].sprite, Localization.Translate("Research") + " : " + Localization.Translate(quest.researches[i].name), completed, QuestTaskType.reserches, quest.researches[i]));
            }

            //check workers in town
            for (int i = 0; i < quest.sendWorkers.Length; i++)
            {
                uint inStock = player.inhabitants.GetSendWorkersAmountByType(quest.sendWorkers[i].type);

                list.Add(new QuestTask(player.inhabitants.scriptableItem.image,
                    Localization.Translate("add" + quest.sendWorkers[i].type) + " : " + inStock + "/" + quest.sendWorkers[i].inhabitants, (inStock >= quest.sendWorkers[i].inhabitants), QuestTaskType.sendInhabitans, null));

            }

            if (quest.renameTown)
                list.Add(new QuestTask(player.inhabitants.scriptableItem.image, Localization.Translate("renameTown"), player.townName != "", QuestTaskType.townRename, null));

            return list;
        }

        public bool HaveCompletedQuest()
        {
            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i].completed == false && CheckQuestPredecessor(quests[i].data) && QuestCompletedPersent(quests[i].data) == 100) return true;
            }
            return false;
        }

        public bool CheckCurrentQuestForBuild()
        {
            ScriptableQuest quest = player.quests.FindCurrentStoryQuest();
            if (quest != null)
            {
                for (int i = 0; i < quest.buildings.Length; i++)
                {
                    int index = buildings.FindIndex(quest.buildings[i].item);
                    if (index != -1 && (buildings.buildings[index].level < quest.buildings[i].requiredBuildingLevel || buildings.buildings[index].time > 0)) return true;
                }
            }

            return false;
        }
    }
}


