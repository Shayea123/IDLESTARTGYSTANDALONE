using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable] public class Researches
    {
        public string name;
        public ScriptableResearch[] researches;
    }

    public class PlayerResearches : MonoBehaviour
    {
        [Header("Components")]
        public Player player;
        public PlayerItems items;
        public PlayerBuildings buildings;

        [HideInInspector] public List<Research> researches = new List<Research>();
        public Researches[] templates;

        public ScriptableBuilding requeredBuilding;

        private void Update()
        {
            for (int i = 0; i < researches.Count; i++)
            {
                if (researches[i].underStudy)
                {
                    if (researches[i].time > 0)
                    {
                        Research temp = researches[i];

                        float bonus = player.prayType == PreyType.researchSpeed ? buildings.PrayBonusBuildingSpeed() : 0;
                        temp.time -= (Time.deltaTime + (Time.deltaTime * bonus));
                        researches[i] = temp;
                    }
                    else
                    {
                        Research temp = researches[i];
                        temp.level += 1;
                        temp.time = 0;
                        temp.underStudy = false;
                        temp.workers = 0;
                        researches[i] = temp;

                        //show info panel
                        player.notifications.RpcAddNotification(Localization.Translate("StudyCompleted") + temp.name, NotificationsType.research);
                    }
                }
            }
        }

        public void CreateNewlist()
        {
            researches.Clear();

            for (int i = 0; i < templates.Length; i++)
            {
                for (int x = 0; x < templates[i].researches.Length; x++)
                {
                    researches.Add(new Research(templates[i].researches[x], 0, false, 0, 0, 0));
                }
            }
        }

        public int FindIndex(ScriptableResearch item)
        {
            for (int i = 0; i < researches.Count; i++)
            {
                if (researches[i].data.Equals(item)) return i;
            }

            return -1;
        }

        public void CmdStartResearching(string name, bool forCoins)
        {
            ScriptableResearch data;
            if (ScriptableResearch.dict.TryGetValue(name.GetStableHashCode(), out data))
            {
                int index = FindIndex(data);
                Research research = researches[index];
                research.underStudy = true;

                if (forCoins)
                {
                    if (research.data.levels[research.level].coins <= items.GetItemAmount(player.coinsItem))
                    {
                        //decrease coins
                        items.DecreaseItemAmount(player.coinsItem, data.levels[research.level].coins);

                        research.time = 0;
                        research.workers = 0;
                    }
                }
                else
                {
                    //decrease items
                    for (int i = 0; i < data.levels[research.level].ingredients.Length; i++)
                    {
                        items.DecreaseItemAmount(data.levels[research.level].ingredients[i].item, data.levels[research.level].ingredients[i].amount);
                    }

                    //set time
                    research.time = data.levels[research.level].researchTime;
                    if (player.adsDisabled) research.time = research.time * 0.7f;

                    research.workers = data.levels[research.level].workersNeed;
                    research.adsAmount = 2;
                }

                researches[index] = research;
            }
        }

        public float ConstructionSpeedBonus()
        {
            float value = 0;
            foreach (Research v in researches)
            {
                if (v.level > 0)
                    value += v.data.levels[v.level - 1].buildingSpeedIncrease;
            }

            return value;
        }

        public float ResearchSpeedIncreaseBonus()
        {
            float value = 0;
            foreach (Research v in researches)
            {
                if (v.level > 0)
                    value += v.data.levels[v.level - 1].increaseResearchSpeed;
            }

            return value;
        }

        public float CraftSpeedBonus()
        {
            float value = 0;
            foreach (Research v in researches)
            {
                if (v.level > 0)
                    value += v.data.levels[v.level - 1].increaseCraftSpeed;
            }

            return value;
        }

        public float IncreasesMaximumWeight()
        {
            float value = 0;
            foreach (Research v in researches)
            {
                if (v.level > 0)
                    value += v.data.levels[v.level - 1].increaseMaxWeight;
            }

            return value;
        }

        public bool CheckRequiredResearcheslist(ScriptableResearchAndLevel[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                int index = FindIndex(list[i].item);

                if (researches[index].level < list[i].researachLevel) return false;
            }
            return true;
        }
        public bool CheckRequiredResearches(ScriptableResearchAndLevel[] requiredResearches, int level)
        {
            for (int i = 0; i < requiredResearches.Length; i++)
            {
                if (requiredResearches[i].forLevel == level)
                {
                    int index = FindIndex(requiredResearches[i].item);
                    if (researches[index].level == 0) return false;
                }
            }
            return true;
        }
        public bool CheckRequiredResearches(ResearchAndLevel[] requiredResearches)
        {
            for (int i = 0; i < requiredResearches.Length; i++)
            {
                int index = FindIndex(requiredResearches[i].item);
                if (researches[index].level == 0) return false;
            }
            return true;
        }
        public List<ScriptableResearchAndLevel> requiredResearches(ScriptableResearchAndLevel[] requiredResearches, int level)
        {
            List<ScriptableResearchAndLevel> list = new List<ScriptableResearchAndLevel>();

            for (int i = 0; i < requiredResearches.Length; i++)
            {
                if (level == requiredResearches[i].forLevel)
                {
                    list.Add(requiredResearches[i]);
                }
            }

            return list;
        }

        public void DecreaseResearchTime(float seconds)
        {
            for (int i = 0; i < researches.Count; i++)
            {
                Research research = researches[i];

                if (research.underStudy)
                {
                    research.time = Mathf.Clamp(research.time - seconds, 0, research.time);
                    researches[i] = research;
                }
            }
        }
    }
}