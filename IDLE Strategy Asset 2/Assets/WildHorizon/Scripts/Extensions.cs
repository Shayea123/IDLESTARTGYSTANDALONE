// This class adds functions to built-in types.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace IdleStrategyKit
{
    public static class Extensions
    {
        // string to int (returns errVal if failed)
        public static int ToInt(this string value, int errVal = 0)
        {
            Int32.TryParse(value, out errVal);
            return errVal;
        }
        public static uint ToUInt(this string value, uint errVal = 0)
        {
            UInt32.TryParse(value, out errVal);
            return errVal;
        }

        // string to long (returns errVal if failed)
        public static long ToLong(this string value, long errVal = 0)
        {
            Int64.TryParse(value, out errVal);
            return errVal;
        }

        // UI SetListener extension that removes previous and then adds new listener
        // (this version is for onClick etc.)
        public static void SetListener(this UnityEvent uEvent, UnityAction call)
        {
            uEvent.RemoveAllListeners();
            uEvent.AddListener(call);
        }

        // UI SetListener extension that removes previous and then adds new listener
        // (this version is for onEndEdit, onValueChanged etc.)
        public static void SetListener<T>(this UnityEvent<T> uEvent, UnityAction<T> call)
        {
            uEvent.RemoveAllListeners();
            uEvent.AddListener(call);
        }

        // check if a list has duplicates
        // new List<int>(){1, 2, 2, 3}.HasDuplicates() => true
        // new List<int>(){1, 2, 3, 4}.HasDuplicates() => false
        // new List<int>().HasDuplicates() => false
        public static bool HasDuplicates<T>(this List<T> list)
        {
            return list.Count != list.Distinct().Count();
        }

        // find all duplicates in a list
        // note: this is only called once on start, so Linq is fine here!
        public static List<U> FindDuplicates<T, U>(this List<T> list, Func<T, U> keySelector)
        {
            return list.GroupBy(keySelector)
                       .Where(group => group.Count() > 1)
                       .Select(group => group.Key).ToList();
        }

        // string.GetHashCode is not guaranteed to be the same on all machines, but
        // we need one that is the same on all machines. simple and stupid:
        public static int GetStableHashCode(this string text)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in text)
                    hash = hash * 31 + c;
                return hash;
            }
        }

        // Collider2D has no ClosestPointOnBounds
        public static Vector2 ClosestPointOnBounds(this Collider2D collider, Vector2 position)
        {
            return collider.bounds.ClosestPoint(position);
        }
    }
}


