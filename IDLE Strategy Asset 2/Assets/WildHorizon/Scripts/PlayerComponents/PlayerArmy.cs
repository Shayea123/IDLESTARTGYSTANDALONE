using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public class PlayerArmy : MonoBehaviour
    {
        public ScriptableItemAndAmount[] addingArmyOnStart = new ScriptableItemAndAmount[] { };

        [Header("Components")]
        public Player player;

        [HideInInspector] public List<Army> armies = new List<Army>();

        public void StartNewGame()
        {
            armies.Clear();
            for (int i = 0; i < addingArmyOnStart.Length; i++)
            {
                armies.Add(new Army((ScriptableArmy)addingArmyOnStart[i].item, addingArmyOnStart[i].amount));
            }
        }

        public void AddArmy(ScriptableArmy army, uint amount)
        {
            int index = GetArmyIndex(army);

            if (index != -1)
            {
                Army temp = armies[index];
                temp.amount += amount;
                armies[index] = temp;
            }
        }
        public int GetArmyIndex(ScriptableArmy army)
        {
            for (int i = 0; i < armies.Count; i++)
            {
                if (armies[i].data.Equals(army)) return i;
            }

            return -1;
        }
        public uint GetArmyAmount(ScriptableArmy army)
        {
            for (int i = 0; i < armies.Count; i++)
            {
                if (armies[i].data.Equals(army)) return armies[i].amount;
            }

            return 0;
        }
        /*public List<Army> AvailebleArmy()
        {
            List<Army> list = new List<Army>();
            for (int i = 0; i < armies.Count; i++)
            {
                if (armies[i].amount > 0) list.Add(armies[i]);
            }
            return list;
        }*/

        public List<ItemSlot> AvailebleArmy()
        {
            List<ItemSlot> list = new List<ItemSlot>();
            for (int i = 0; i < armies.Count; i++)
            {
                if (armies[i].amount > 0) list.Add(new ItemSlot(new Item(armies[i].data), armies[i].amount));
            }
            return list;
        }

        public uint InhabitantsInArmy()
        {
            uint amount = 0;
            for (int i = 0; i < armies.Count; i++)
            {
                amount += armies[i].amount;
            }

            return amount;
        }

        private void OnValidate()
        {
            for (int i = 0; i < addingArmyOnStart.Length; i++)
            {
                addingArmyOnStart[i].name = addingArmyOnStart[i].item.name;
            }
        }
    }
}