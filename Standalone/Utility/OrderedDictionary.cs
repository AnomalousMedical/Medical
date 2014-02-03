using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Medical
{
    public class OrderedDictionary<TKey, TValue>
    {
        private OrderedDictionary orderedDictionary = new OrderedDictionary();

        public OrderedDictionary()
        {

        }

        public void Insert(int index, TKey key, TValue value)
        {
            orderedDictionary.Insert(index, key, value);
        }

        public void RemoveAt(int index)
        {
            orderedDictionary.RemoveAt(index);
        }

        public TValue this[int index]
        {
            get
            {
                return (TValue)orderedDictionary[index];
            }
            set
            {
                orderedDictionary[index] = value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            orderedDictionary.Add(key, value);
        }

        public void Clear()
        {
            orderedDictionary.Clear();
        }

        public bool Contains(TKey key)
        {
            return orderedDictionary.Contains(key);
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return orderedDictionary.IsReadOnly;
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                foreach (var key in orderedDictionary.Keys)
                {
                    yield return (TKey)key;
                }
            }
        }

        public void Remove(TKey key)
        {
            orderedDictionary.Remove(key);
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (var value in orderedDictionary.Values)
                {
                    yield return (TValue)value;
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return (TValue)orderedDictionary[key];
            }
            set
            {
                orderedDictionary[key] = value;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (orderedDictionary.Contains(key))
            {
                value = this[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

        public void CopyTo(Array array, int index)
        {
            orderedDictionary.CopyTo(array, index);
        }

        public int Count
        {
            get
            {
                return orderedDictionary.Count;
            }
        }
    }
}
