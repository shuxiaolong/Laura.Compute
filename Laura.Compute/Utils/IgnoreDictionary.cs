using System;
using System.Collections;
using System.Collections.Generic;
#if (!WindowsCE && !PocketPC) 
using System.Runtime.Serialization;
#endif

namespace Laura.Compute.Utils
{
    /// <summary>
    /// 字符串 泛型字典,该类的引用 是线程安全的
    /// </summary>
    [Serializable]
    internal class IgnoreDictionary<T> : Dictionary<string, T>
    {
        public IgnoreDictionary() : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }

#if (!WindowsCE && !PocketPC) 
        public IgnoreDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif

        private static readonly T defaultValue = default(T);

        public new T this[string key]
        {
            get
            {
                T record;
                if (TryGetValue(key, out record)) return record;
                else return defaultValue;
            }
            set
            {
                try
                {
                    if (base.ContainsKey(key)) base[key] = value;
                    else base.Add(key, value);
                }
                catch(Exception) { }
            }
        }
        public virtual void AddRange(IDictionary<string, T> values)
        {
            if (values == null || values.Count <= 0) return;
            foreach (string k in values.Keys)
            {
                this[k] = values[k];
            }
        }
        public virtual void AddRange(IgnoreDictionary<T> values)
        {
            if (values == null || values.Count <= 0) return;
            foreach (string k in values.Keys)
            {
                this[k] = values[k];
            }
        }


        public virtual Hashtable ToHashtable()
        {
            Hashtable hashtable = new Hashtable();

            try
            {
                foreach (KeyValuePair<string, T> pair in this)
                    hashtable[pair.Key] = pair.Value;
            }
            catch(Exception) { }

            return hashtable;
        }
        public static IgnoreDictionary<T> FromHashtable(Hashtable hashtable)
        {
            IgnoreDictionary<T> dictionary = new IgnoreDictionary<T>();
            try
            {
                foreach (string key in hashtable.Keys)
                {
                    object record = hashtable[key];
                    dictionary[key] = record is T ? (T) record : defaultValue;
                }
            }
            catch (Exception) { }

            return dictionary;
        }
    }


    /// <summary>
    /// 不区分大小写键值的哈希表
    /// </summary>
    [Serializable]
    internal class IgnoreHashtable : IgnoreDictionary<object>
    {
        public IgnoreHashtable() { }
#if (!WindowsCE && !PocketPC) 
        public IgnoreHashtable(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }

}
