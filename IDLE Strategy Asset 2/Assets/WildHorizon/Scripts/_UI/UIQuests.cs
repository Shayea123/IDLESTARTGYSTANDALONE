using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIQuests : MonoBehaviour
    {
        [Header("Components")]
        public UIAudio _audio;
        public UIResearchDescription _researches;

        [Space(15)]
        public Button buttonOpenQuest;
        public GameObject imageInfoQuests;

        public GameObject panel;
        public Transform content;
        public UIQuestSlot prefab;

        public GameObject panelDescription;
        public Text textName;
        public Text textInfo;

        [Header("tasks")]
        public Transform tasksContent;
        public UIQuestTaskSlot taskPrefab;

        [Header("rewards")]
        public Transform rewardContent;
        public UIQuestRewardSlot rewardPrefab;

        [Header("Buttons")]
        public Button buttonClose;
        public Button buttonGetReward;
        public Button buttonContinue;

        [Header("Colors")]
        public Color selected;
        public Color nonSelected;
        public Color colorStory;
        public Color colorAchievement;
        public Color colorGuild;

        private static ScriptableQuest selectedQuest;

        private void Start()
        {
            UISettings.StartNewStoryGame.AddListener(OpenQuestDescriptionPanel);
            Invoke("OpenQuestDescriptionPanel", 1);

            buttonOpenQuest.onClick.SetListener(() =>
            {
                panel.SetActive(true);
                _audio.PlaySoundButtonClick();
            });
        }

        private void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                //info image for quests
                imageInfoQuests.SetActive(player.quests.HaveCompletedQuest());

                if (panel.activeSelf)
                {
                    //show not completed and those that we can fulfill
                    List<Quest> listAllQuests = player.quests.AllQuestsNotCompleted();

                    // instantiate/destroy enough slots for RequiredIngredients
                    UIUtils.BalancePrefabs(prefab.gameObject, listAllQuests.Count, content);

                    for (int i = 0; i < listAllQuests.Count; i++)
                    {
                        UIQuestSlot slot = content.transform.GetChild(i).GetComponent<UIQuestSlot>();

                        if (listAllQuests[i].data.questType == QuestType.story) slot.imageType.color = colorStory;
                        else if (listAllQuests[i].data.questType == QuestType.achievement) slot.imageType.color = colorAchievement;
                        else slot.imageType.color = colorGuild;

                        //name
                        slot.textName.text = Localization.Translate(listAllQuests[i].data.name);

                        if (selectedQuest != null && selectedQuest.Equals(listAllQuests[i].data)) slot.textName.color = selected;
                        else slot.textName.color = nonSelected;

                        float completed = player.quests.QuestCompletedPersent(listAllQuests[i].data);
                        if (listAllQuests[i].data.showCompletionPercentage && completed != 100) slot.textDone.text = completed + " %";
                        else slot.textDone.text = null;
                        slot.completed.SetActive(completed == 100);

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            selectedQuest = listAllQuests[icopy].data;
                            panelDescription.SetActive(true);
                        });
                    }
                }

                if (panelDescription.activeSelf)
                {
                    if (selectedQuest != null)
                    {
                        //continue
                        buttonClose.gameObject.SetActive(selectedQuest.nextPart == null);
                        buttonContinue.gameObject.SetActive(selectedQuest.nextPart != null);
                        buttonContinue.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();

                            player.quests.SetCompleted(selectedQuest.name);
                            selectedQuest = selectedQuest.nextPart;
                        });

                        //get reward
                        buttonGetReward.gameObject.SetActive(selectedQuest.nextPart == null && player.quests.QuestCompletedPersent(selectedQuest) == 100);
                        buttonGetReward.onClick.SetListener(() =>
                        {
                            _audio.PlaySoundButtonClick();
                            panel.SetActive(false);

                            player.quests.SetCompleted(selectedQuest.name);
                            selectedQuest = null;
                        });

                        //name
                        textName.text = Localization.Translate(selectedQuest.name);

                        //description
                        textInfo.text = selectedQuest.GetDescriptionByLanguage(Localization.languageCurrent);

                        //tasks
                        List<QuestTask> tasks = player.quests.GetTaskForQuest(selectedQuest);
                        tasksContent.gameObject.SetActive(tasks.Count > 0);

                        // instantiate/destroy enough slots
                        UIUtils.BalancePrefabs(taskPrefab.gameObject, tasks.Count + 1, tasksContent);

                        //refresh all slots
                        for (int i = 0; i < tasks.Count; i++)
                        {
                            UIQuestTaskSlot slot = tasksContent.transform.GetChild(i + 1).GetComponent<UIQuestTaskSlot>();

                            if (tasks[i].sprite != null) slot.image.sprite = tasks[i].sprite;

                            //name
                            slot.textName.text = tasks[i].name;

                            slot.toggle.isOn = tasks[i].completed;

                            int icopy = i;
                            slot.button.interactable = !tasks[i].completed;
                            slot.button.onClick.SetListener(() =>
                            {
                                if (tasks[icopy].type == QuestTaskType.buildings)
                                {
                                    panel.SetActive(false);

                                    UIBuildingSelect.selectedBuilding = (ScriptableBuilding)tasks[icopy].scObject;
                                    UIBuild.singleton.ShowBuildingFromOtherPanels();
                                }
                                else if (tasks[icopy].type == QuestTaskType.reserches)
                                {
                                    panel.SetActive(false);

                                    UIResearches.selectedResearch = (ScriptableResearch)tasks[icopy].scObject;
                                    _researches.panel.SetActive(true);
                                }
                                else if (tasks[icopy].type == QuestTaskType.townRename)
                                {
                                    UITown.singleton.panel.SetActive(true);
                                    UITown.singleton.panelTownRename.SetActive(true);
                                }
                                else if (tasks[icopy].type == QuestTaskType.sendInhabitans)
                                {
                                    UITown.singleton.panel.SetActive(true);
                                    UITown.singleton.contentManage.gameObject.SetActive(true);
                                }
                            });
                        }

                        //reward
                        if (selectedQuest.rewardItems.Count > 0)
                        {
                            rewardContent.gameObject.SetActive(true);

                            // instantiate/destroy enough slots
                            UIUtils.BalancePrefabs(rewardPrefab.gameObject, selectedQuest.rewardItems.Count + 1, rewardContent);

                            //refresh all slots
                            for (int i = 0; i < selectedQuest.rewardItems.Count; i++)
                            {
                                UIQuestRewardSlot slot = rewardContent.transform.GetChild(i + 1).GetComponent<UIQuestRewardSlot>();

                                slot.image.sprite = selectedQuest.rewardItems[i].item.image;

                                //name
                                slot.textName.text = Localization.Translate(selectedQuest.rewardItems[i].item.name);

                                //amount
                                slot.textAmount.text = selectedQuest.rewardItems[i].amount.ToString();

                                int icopy = i;
                                slot.buttonDescription.onClick.SetListener(() =>
                                {
                                    _audio.PlaySoundButtonClick();
                                    UIDescriptionPanel.singleton.ShowScriptableItem(player, selectedQuest.rewardItems[icopy].item);
                                });
                            }
                        }
                        else rewardContent.gameObject.SetActive(false);
                    }
                    else
                    {
                        panelDescription.SetActive(false);
                    }
                }
            }
        }

        private void OpenQuestDescriptionPanel()
        {
            Player player = Player.localPlayer;
            if (player != null && SettingsLoader.showQuestOnStart)
            {
                selectedQuest = player.quests.FindCurrentStoryQuest();
                if (selectedQuest == null) selectedQuest = player.quests.FindNextStoryQuest();
                panelDescription.SetActive(selectedQuest != null);
            }
        }
    }
}