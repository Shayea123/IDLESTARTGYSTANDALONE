// Inventories need a slot type to hold Item + Amount. This is better than
// storing .amount in 'Item' because then we can use Item.Equals properly
// any workarounds to ignore the .amount.
//
// Note: always check .amount > 0 before accessing .item.
//       set .amount=0 to clear it.
using System;
using System.Text;

namespace IdleStrategyKit
{
    [Serializable]
    public struct ItemSlot
    {
        public Item item;
        public uint amount;
        public uint amountShop;

        // constructors
        public ItemSlot(Item item, uint amount = 1, uint amountShop = 0)
        {
            this.item = item;
            this.amount = amount;
            this.amountShop = amountShop;
        }

        // helper functions to increase/decrease amount more easily
        // -> returns the amount that we were able to increase/decrease by
        /*public int DecreaseAmount(int reduceBy)
        {
            // as many as possible
            long limit = Mathf.Clamp(reduceBy, 0, (int)amount);
            amount -= limit;
            return limit;
        }*/

        /*public int IncreaseAmount(int increaseBy)
        {
            // as many as possible
            int limit = Mathf.Clamp(increaseBy, 0, item.maxStack - amount);
            amount += limit;
            return limit;
        }*/

        // tooltip
        public string ToolTip()
        {
            if (amount == 0) return "";

            // we use a StringBuilder so that addons can modify tooltips later too
            // ('string' itself can't be passed as a mutable object)
            StringBuilder tip = new StringBuilder(item.ToolTip());
            tip.Replace("{AMOUNT}", amount.ToString());
            return tip.ToString();
        }
    }
}


