using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIArmySlot : MonoBehaviour
    {
        public Image image;
        public Text textName;
        public Text textAmount;
        public Button buttonPlus;
        public Button buttonMinus;

        public ScriptableWeapon item;
        public uint amount;
    }
}


