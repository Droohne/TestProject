using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsight_Test
{
    // It was sealed. Made it public for indexer to work
    public class Node<K, V>(K key, V v) : IEquatable<Node<K, V>> 
    {
        // basically a cropped LinkedListNode

        private K _key = key;
        private V _value = v;
        private Node<K, V>? _next = null;
        private Node<K, V>? _previous = null;
        public K Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public V Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Node<K, V>? Previous
        {
            get { return _previous; }
            set { _previous = value; }
        }
        public Node<K, V>? Next
        {
            get { return _next; }
            set { _next = value; }
        }
        public bool Equals(Node<K, V>? other)
        {
            if (other is null)
                return false;
            if (_key is Array thisArray && other._key is Array otherArray)
                // For now it works, but this way of thinking as wrong (probably)
                return thisArray.Length == otherArray.Length && thisArray.Cast<object>().SequenceEqual(otherArray.Cast<object>());
            return other != null && EqualityComparer<K>.Default.Equals(_key, other.Key);
        }
        public override bool Equals(object? other)
        {
            return Equals(other as Node<K,V>);
        }

        public override int GetHashCode()
        {

            if (_key is Array keyArray)
            {
                // For now it works, but this way of thinking as wrong (probably)
                int hash = 17;
                foreach (var item in keyArray)
                {
                    hash = hash * 31 + (item?.GetHashCode() ?? 0);
                }
                return hash + _value?.GetHashCode() ?? 0;
            }
            return _key?.GetHashCode() ?? 0 + _value?.GetHashCode() ?? 0;
        }
    }
}
