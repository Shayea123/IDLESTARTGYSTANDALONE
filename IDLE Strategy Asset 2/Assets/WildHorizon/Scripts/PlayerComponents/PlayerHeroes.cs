using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public class PlayerHeroes : MonoBehaviour
    {
        [Header("Components")]
        public Player player; //for notification

        [Space(10)]
        public ScriptableHero[] templates = new ScriptableHero[] { };
        public ScriptableHero[] defaultHeroes = new ScriptableHero[] { };

        [HideInInspector] public List<Hero> heroes = new List<Hero>();

        public void StartNewGame()
        {
            heroes.Clear();

            //add default Heroes
            for (int i = 0; i < defaultHeroes.Length; i++)
            {
                Hero hero = new Hero();
                hero.parts = 1;
                hero.level = 1;
                hero.hash = defaultHeroes[i].name.GetStableHashCode();
                heroes.Add(hero);
            }
        }

        public int GetCharacterIndex(string name)
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].name == name) return i;
            }

            return -1;
        }
        public int GetCharacterIndex(ScriptableHero hero)
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].data.Equals(hero)) return i;
            }

            return -1;
        }
        public int FindHeroIndexByPlace(string place)
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].placeId == place) return i;
            }

            return -1;
        }

        public List<Hero> GetStandbyCharacters()
        {
            List<Hero> list = new List<Hero>();

            for (int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].level > 0) list.Add(heroes[i]);
            }

            return list;
        }
        public List<ScriptableHero> GetUnobtainedCharacters()
        {
            List<ScriptableHero> list = new List<ScriptableHero>();

            for (int i = 0; i < templates.Length; i++)
            {
                int index = GetCharacterIndex(templates[i]);
                if (index == -1 || heroes[index].level == 0)
                {
                    list.Add(templates[i]);
                }
            }

            return list;
        }

        //upgrade
        public bool CanUpgradeHero(int index)
        {
            return heroes[index].level < heroes[index].data.needParts.Length &&
                            heroes[index].parts >= heroes[index].data.needParts[heroes[index].level];
        }
        public void UpgradeHero(int index)
        {
            if (CanUpgradeHero(index))
            {
                Hero hero = heroes[index];
                hero.parts -= hero.data.needParts[hero.level];
                hero.level += 1;
                heroes[index] = hero;
            }
        }

        public ScriptableHero RandomHeroPart(ItemRarity rarity)
        {
            List<ScriptableHero> list = new List<ScriptableHero>();

            //generate list by rarity
            for (int i = 0; i < templates.Length; i++)
            {
                if (templates[i].rarity == rarity) list.Add(templates[i]);
            }

            if (list.Count > 0)
            {
                return list[Random.Range(0, list.Count)];
            }

            return null;
        }

        public void AddHeroPart(ScriptableHero heroPart, ushort amount)
        {
            int index = GetCharacterIndex(heroPart);
            if (index != -1)
            {
                Hero hero = heroes[index];
                hero.parts += amount;
                heroes[index] = hero;

                ShowNotification(ref hero, amount);
            }
            else
            {
                Hero hero = new Hero();
                hero.parts = amount;
                hero.hash = heroPart.name.GetStableHashCode();
                heroes.Add(hero);

                ShowNotification(ref hero, amount);
            }
        }

        private void ShowNotification(ref Hero hero, int amount)
        {
            if (hero.parts >= hero.data.needParts[hero.level])
                player.notifications.RpcAddNotification("You have collected the required number of parts " + hero.name, NotificationsType.none);

            //show info panel
            player.notifications.RpcAddNotification("You get : " + hero.name + " / " + amount, NotificationsType.heroes);
        }

        public string HeroBonuses(ScriptableHero hero)
        {
            if (hero.bonuses.Count > 0)
            {
                string bonuses = "";
                for (int i = 0; i < hero.bonuses.Count; i++)
                {
                    if (hero.bonuses[i].bonusType == HeroBonusType.dollarsIncrease) bonuses += "Tax increase by " + (hero.bonuses[i].value * 100) + "% \n";
                    else if (hero.bonuses[i].bonusType == HeroBonusType.buildingSpeed) bonuses += "Build speed +" + (hero.bonuses[i].value * 100) + "% \n";
                    else if (hero.bonuses[i].bonusType == HeroBonusType.researchSpeed) bonuses += "Research speed +" + (hero.bonuses[i].value * 100) + "% \n";
                    else if (hero.bonuses[i].bonusType == HeroBonusType.miningSpeed) bonuses += "Resources mining speed +" + (hero.bonuses[i].value * 100) + "% \n";
                    else if (hero.bonuses[i].bonusType == HeroBonusType.miningAmount) bonuses += "Resources mining amount +" + (hero.bonuses[i].value * 100) + "% \n";
                    else if (hero.bonuses[i].bonusType == HeroBonusType.transportationSpeed) bonuses += "Resources transport speed +" + (hero.bonuses[i].value * 100) + "% \n";
                    else if (hero.bonuses[i].bonusType == HeroBonusType.transportationWeight) bonuses += "Resources transport weight +" + (hero.bonuses[i].value * 100) + "% \n";
                    else if (hero.bonuses[i].bonusType == HeroBonusType.processingSpeed) bonuses += "Processing Resources speed +" + (hero.bonuses[i].value * 100) + "% \n";
                }
                return bonuses;
            }
            else return null;
        }
        public float BonusForManager(string building, HeroBonusType bonusType)
        {
            int heroIndex = FindHeroIndexByPlace(building);
            if (heroIndex != -1)
            {
                ScriptableHero hero = heroes[heroIndex].data;
                if (hero.bonuses.Count > 0)
                {
                    for (int i = 0; i < hero.bonuses.Count; i++)
                    {
                        if (hero.bonuses[i].bonusType == bonusType)
                            return hero.bonuses[i].value;
                    }
                }
            }

            return 0;
        }

        public void SetPlace(int index, string place)
        {
            //whether a hero has already been assigned to this place
            int oldHeroIndex = FindHeroIndexByPlace(place);
            if (oldHeroIndex != -1)
            {
                Hero old = heroes[oldHeroIndex];
                old.placeId = "";
                heroes[oldHeroIndex] = old;
            }

            Hero temp = heroes[index];
            temp.placeId = place;
            heroes[index] = temp;
        }

        public bool IsHeroWaiting()
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                if (CanUpgradeHero(i)) return true;
            }

            return false;
        }
    }
}