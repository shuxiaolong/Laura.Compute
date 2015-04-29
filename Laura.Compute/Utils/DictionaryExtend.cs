using System.Collections.Generic;

namespace Laura.Compute.Utils
{
    internal class DictionaryExtend
    {
        public static object GetValue(Dictionary<string, object> hash, string key)
        {
            if (hash == null || string.IsNullOrEmpty(key)) return null;

            object record;
            bool result = hash.TryGetValue(key, out record);
            return result ? record : null;
        }

        public static void SetValue(Dictionary<string, object> hash, string key, object value)
        {
            if (hash == null || string.IsNullOrEmpty(key)) return;

            if (hash.ContainsKey(key)) hash[key] = value;
            else hash.Add(key, value);
        }

    }
}
