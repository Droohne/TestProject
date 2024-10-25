using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsight_Test
{
    internal interface ICollection<K, V> : IEnumerable<Node<K,V>> , IEnumerator<Node<K, V>>
    {
        public bool Add(K key, V value);
        public V Remove(K key);
        public void Clear();
        public K GetKeyByValue(V value);    
        public V GetValueByKey(K key);
        public bool ContainsKey(K key);
        public bool ContainsValue(V value);

        public int Count();

        public V[] GetAllValues();

        public K[] GetAllKeys();
    }
}
