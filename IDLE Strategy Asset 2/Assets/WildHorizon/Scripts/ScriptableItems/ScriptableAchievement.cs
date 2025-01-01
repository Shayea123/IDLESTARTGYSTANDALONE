using UnityEngine;

namespace IdleStrategyKit
{
    [CreateAssetMenu(menuName = "Idle clicker strategy Game/Achievements", order = 999)]
    public class ScriptableAchievement : ScriptableObject
    {
        [Header("Requirements")]
        public ScriptableQuest predecessor; // this quest has to be completed first
        public ScriptableItem[] items;


        [Header("Description by Language")]
        [SerializeField] private LocalizeText[] descriptionLocalize;
        public string GetDescriptionByLanguage(SystemLanguage lang)
        {
            for (int i = 0; i < descriptionLocalize.Length; i++)
            {
                if (descriptionLocalize[i].language == lang) return descriptionLocalize[i].description;
            }
            return null;
        }
    }
}


