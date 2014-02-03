using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// A simple class that holds data in a dictionary up to a certain count.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class MemoryCache<TKey, TValue>
    {
        private OrderedDictionary<TKey, TValue> cahcedValues = new OrderedDictionary<TKey, TValue>();

        public MemoryCache()
            :this(10)
        {
            
        }

        public MemoryCache(int maxCachedValues)
        {
            MaxCachedValues = maxCachedValues;
        }

        /// <summary>
        /// Add or set a given value for key. If there are more than MaxCachedValues and the value does not already exist in the
        /// cache an item will be removed in fifo order. If it does exist, it will be moved back to the end of the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                cahcedValues.TryGetValue(key, out value);
                return value;
            }
            set
            {
                if (cahcedValues.Contains(key))
                {
                    cahcedValues.Remove(key);
                    cahcedValues.Add(key, value); //Move to end of list
                }
                else
                {
                    if (cahcedValues.Count + 1 > MaxCachedValues)
                    {
                        cahcedValues.RemoveAt(0);
                    }
                    cahcedValues[key] = value;
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return cahcedValues.TryGetValue(key, out value);
        }

        public void Clear()
        {
            cahcedValues.Clear();
        }

        public void Remove(TKey key)
        {
            cahcedValues.Remove(key);
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                return cahcedValues.Keys;
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                return cahcedValues.Values;
            }
        }

        public int MaxCachedValues { get; set; }
    }
}
