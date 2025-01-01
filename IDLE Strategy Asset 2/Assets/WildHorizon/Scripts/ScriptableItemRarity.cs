using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum ItemRarity { Normal, Rare, Elite, Epic, Legendary }

    [Serializable]
    public class ItemTypes
    {
        public ItemRarity rarity;
        public Color color;
    }

    [CreateAssetMenu(menuName = "Item Rarity", order = 999)]
    public class ScriptableItemRarity : ScriptableObject
    {
        [Header("Sprites")]
        [SerializeField] private bool useRarityTypeA;
        [SerializeField] private bool useRarityTypeB;
        [SerializeField] private Sprite raritySpriteA;
        [SerializeField] private Sprite raritySpriteB;

        [Header("Names and Colors")]
        [SerializeField] private List<ItemTypes> types = new List<ItemTypes>() { };

        public Sprite GetSprite
        {
            get
            {
                if (useRarityTypeA) return raritySpriteA;
                else if (useRarityTypeB) return raritySpriteB;
                else return null;
            }
        }

        public Color GetColor(ScriptableItem item)
        {
            return types[types.FindIndex(x => x.rarity == item.rarity)].color;
        }
        public Color GetColor(ItemRarity rarity)
        {
            return types[types.FindIndex(x => x.rarity == rarity)].color;
        }

        public Color GetColorForEmptySlot()
        {
            return types[0].color;
        }

        //for item enchantment addon
        public int GetRarityIndex(ScriptableItem item)
        {
            return types.FindIndex(x => x.rarity == item.rarity);
        }
    }
}


