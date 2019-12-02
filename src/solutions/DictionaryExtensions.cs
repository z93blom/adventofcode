using System.Collections.Generic;

namespace AdventOfCode
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Initialize<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            IEnumerable<TKey> keys, TValue initialValue)
        {
            foreach (var key in keys)
            {
                dictionary[key] = initialValue;
            }

            return dictionary;
        }
    }
}