using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Descant.Utilities
{
    /// <summary>
    /// A Serializable entry in a ListDictionary
    /// </summary>
    /// <typeparam name="K">The key type for this entry</typeparam>
    /// <typeparam name="V">The value type for this entry</typeparam>
    [Serializable]
    public class ListDictionaryEntry<K, V>
    {
        /// <summary>
        /// The key for this entry
        /// </summary>
        public K Key;
        
        /// <summary>
        /// The value for this entry
        /// </summary>
        public V Value;
    }

    /// <summary>
    /// A Serializable list-based dictionary class
    /// </summary>
    /// <typeparam name="K">The key type for the dictionary</typeparam>
    /// <typeparam name="V">The value type for the dictionary</typeparam>
    [Serializable]
    public class ListDictionary<K, V> : IEnumerable
    {
        /// <summary>
        /// The inner list of entries in the dictionary
        /// </summary>
        public List<ListDictionaryEntry<K, V>> InnerList = new();
        
        /// <summary>
        /// The number of entries in the dictionary
        /// </summary>
        public int Count => InnerList.Count;

        /// <summary>
        /// All the keys in the dictionary
        /// </summary>
        public List<K> Keys => InnerList.Select(entry => entry.Key).ToList();
        
        /// <summary>
        /// All the values in the dictionary
        /// </summary>
        public List<V> Values => InnerList.Select(entry => entry.Value).ToList();

        /// <summary>
        /// Method to add new entries to the dictionary
        /// </summary>
        /// <param name="key">The key for the new entry</param>
        /// <param name="value">The value for the new entry</param>
        public void Add(K key, V value)
        {
            if (!ContainsKey(key)) InnerList.Add(new ListDictionaryEntry<K, V> {Key = key, Value = value});
            else DescantUtilities.ErrorMessage(GetType(), $"{this} already contains an entry with key {key}");
        }

        /// <summary>
        /// Method to get a specific entry from the dictionary
        /// </summary>
        /// <param name="key">The key of the entry to get</param>
        public ListDictionaryEntry<K, V> GetEntry(K key)
        {
            var hits = InnerList.Where(entry => entry.Key.Equals(key)).ToList();

            switch (hits.Count)
            {
                case 0: throw new Exception($"{this} contains no entry with key {key}");
                case 1: return hits[0];
                default:
                {
                    DescantUtilities.ErrorMessage(GetType(), $"{this} already contains an entry with key {key}");
                    return hits[0];
                }
            }
        }

        /// <summary>
        /// Method to get a specific value from the dictionary
        /// </summary>
        /// <param name="key">The key of the value to get</param>
        public V Get(K key)
        {
            var entry = GetEntry(key);
            
            if (entry != null) return entry.Value;
            else throw new Exception($"{this} contains no entry with key {key}");
        }

        /// <summary>
        /// Method to remove a specific entry from the dictionary
        /// </summary>
        /// <param name="key">The key of the value to remove</param>
        public void Remove(K key) => InnerList.Remove(GetEntry(key));
        
        /// <summary>
        /// Method to clear the dictionary
        /// </summary>
        public void Clear() => InnerList.Clear();
        
        /// <summary>
        /// Determines whether the dictionary contains an entry
        /// </summary>
        /// <param name="key">The key of the entry to check</param>
        public bool ContainsKey(K key) => Keys.Contains(key);
        
        #region IEnumerator methods
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public ListDictionaryEnum<K, V> GetEnumerator() => new (InnerList);
        
        public V this[K key]
        {
            get => Get(key);
            set => GetEntry(key).Value = value;
        }
        
        #endregion
    }
    
    #region ListDictionary IEnumerator

    [Serializable]
    public class ListDictionaryEnum<K, V> : IEnumerator
    {
        List<ListDictionaryEntry<K, V>> innerList;
        int index = -1;

        public ListDictionaryEnum(List<ListDictionaryEntry<K, V>> innerList) => this.innerList = innerList;
        
        public bool MoveNext()
        {
            index++;
            return index < innerList.Count;
        }

        public void Reset() => index = -1;

        object IEnumerator.Current => innerList[index];

        public ListDictionaryEntry<K, V> Current => innerList[index];
    }
    
    #endregion
}