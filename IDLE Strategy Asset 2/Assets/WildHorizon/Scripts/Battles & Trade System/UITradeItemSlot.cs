using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UITradeItemSlot : MonoBehaviour
    {
        public Image image;
        public Text textName;
        public Text textAmount;
        public Button buttonPlus;
        public Button buttonMinus;

        public ScriptableItem item;
        //public int amount;
    }
}


