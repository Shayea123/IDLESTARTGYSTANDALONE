using UnityEngine;

namespace IdleStrategyKit
{
    public static class UIUtils
    {
        // instantiate/remove enough prefabs to match amount
        public static void BalancePrefabs(GameObject prefab, int amount, Transform parent)
        {
            // instantiate until amount
            for (int i = parent.childCount; i < amount; ++i)
            {
                GameObject go = GameObject.Instantiate(prefab);
                go.transform.SetParent(parent, false);
            }

            // delete everything that's too much
            // (backwards loop because Destroy changes childCount)
            for (int i = parent.childCount - 1; i >= amount; --i)
                GameObject.Destroy(parent.GetChild(i).gameObject);
        }

        public static string LongToString(long value)
        {
            string temp = value.ToString();
            if (value >= 1000000000) temp = ((double)value / 1000000000).ToString("F2") + "B";
            else if (value >= 1000000) temp = ((double)value / 1000000).ToString("F2") + "M";
            else if (value >= 1000) temp = ((float)value / 1000).ToString("F2") + "K";

            return temp;
        }

        public static int ToInt(this bool Value)
        {
            return Value ? 1 : 0;
        }

        public static bool ToBool(this int Value)
        {
            return Value == 1 ? true : false;
        }
    }
}


