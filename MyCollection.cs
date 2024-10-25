using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Forsight_Test
{
    public class MyCollection<K, V> : ICollection<K, V>
    {
        private int _count = 0;

        private Node<K,V>? _start;
        private Node<K,V>? current;
        Node<K, V> IEnumerator<Node<K, V>>.Current => (Node<K, V>)Current;

        public object Current => Current;

        public Node<K, V> this[int index]
        {
            // Indexation
            get
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException("Index is out of range.");
                current = _start;
                for (int i = 0; i < index; i++)
                {
                    MoveNext();
                }
                // Moving till expected index
                var v = current;
                Reset();
                return v;
            }
            set
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException("Index is out of range.");

                if (this.ContainsKey(value.Key))
                    throw new Exception("Key already percist in collection. Setting value is disallowed");

                current = _start;

                for (int i = 0; i < index; i++)
                {
                    MoveNext();
                }
                // Moving till expected index
                if (current != null)
                {
                    value.Next = current.Next;
                    value.Previous = current.Previous;
                    if (current.Previous != null)
                        current.Previous.Next = value;
                    if (current.Next != null)
                        current.Next.Previous = value;
                }
                Reset();
            }
        }
        /// <summary>
        /// Adds or replaces value in collection depending on its existence inside the collection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns> Return True if key doesnt exist in collection. False otherwise</returns>
        
        public bool Add(K key, V value) 
        {
            if (_start == null)
            {
                _count++;
                _start = new Node<K, V>(key, value);
            }
            else
            {
                var current = _start;
                var node = new Node<K, V> (key, value);
                while (current != null)
                {
                    if (current.Equals(node))
                    {
                        current.Value = value;
                        return false;
                    }
                    if (current.Next == null)
                    {
                        current.Next = new Node<K, V>(key, value);
                        current.Next.Previous = current;
                        _count++;
                        break;
                    }
                    else
                        current = current.Next;
                }
            }
            return true;
        }

        /// <summary>
        /// Set collection empty
        /// </summary>
        public void Clear()
        {
            var current = _start;
            _count=0;
            while (current != null)
            {
                var v = current.Next;
                current.Next = null;
                current.Previous = null;
                current = v;
            }
            _start = null;
        }

        /// <summary>
        /// Check if Key exist in collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if key exist on collection. False otherwise</returns>
        public bool ContainsKey(K key) //It works
        {
            //default is a roundabout of nonexistant value null is allowed so it stays
            var node = new Node<K, V>(key, default);
            var current = _start;
            while (current != null)
            {
                if (current.Equals(node))
                {
                    return true;
                }
                current = current.Next;
            }
            return false;
        }
        /// <summary>
        /// Check if Value exist in collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if value exist on collection. False otherwise</returns>
        public bool ContainsValue(V value) 
        {
            var current = _start;
            while (current != null)
            {
                if (EqualityComparer<V>.Default.Equals(current.Value, value))
                {
                    return true;
                }
                current = current.Next;
            }
            return false;
        }
        /// <summary>
        /// Amount of items in collection
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _count; 
        }

        /// <summary>
        /// Returns all keys within the collection
        /// </summary>
        /// <returns>Array of keys</returns>
        public K[] GetAllKeys()
        {
            var keys = new K[_count];
            var current = _start;
            int i = 0;
            while (current != null)
            {
                keys[i] = current.Key;
                current = current.Next;
                i++;
            }
            return keys;
        }
        /// <summary>
        /// Returns all values within the collection
        /// </summary>
        /// <returns>Array of values</returns>
        public V[] GetAllValues() 
        {
            var values = new V[_count];
            var current = _start;
            int i = 0;
            while (current != null)
            {
                values[i] = current.Value;
                current = current.Next;
                i++;
            }
            return values;
        }
        /// <summary>
        /// Find first key by given value within the collection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public K GetKeyByValue(V value)
        {
            var current = _start;
            while (current != null)
            {
                if (current.Value is Array currentArray && value is Array valueArray)
                {
                    // TODO: Check for every complex type?
                   if (currentArray.Length == valueArray.Length && currentArray.Cast<object>().SequenceEqual(valueArray.Cast<object>()))
                   {
                        return current.Key;
                   }
                }
                // For non-complex types, use default equality
                else
                    if (EqualityComparer<V>.Default.Equals(current.Value, value))
                        return current.Key;
                current = current.Next;
            }
            throw new KeyNotFoundException(); // Use generic Exception?
        }

        /// <summary>
        /// Get value by given key. Returned value can be null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public V GetValueByKey(K key)
        {
            var node = new Node<K, V>(key, default); // Null is allowed here. Resolving warning creates more warnings

            // It works for my implementation of Equals and GetHashCode 
            // While this doesnt EqualityComparer<V>.Default.Equals(current.Value, value) beacase arrays are a ref types
            var current = _start;
            while (current != null)
            {
                if (current.Equals(node))
                {
                    return current.Value;
                }
                current = current.Next;
            }
            throw new KeyNotFoundException(); // Use generic Exseption?
        }

        /// <summary>
        /// Removes key if it is found within the collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Deleted key</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public V Remove(K key)
        {
            if (_start == null)
            {
                throw new KeyNotFoundException();
            }
            _count--;
            var current = _start;
            while (current != null)
            {
                if (EqualityComparer<K>.Default.Equals(_start.Key, key))
                {
                    if (current.Previous != null)
                        current.Previous.Next = current.Next;
                    if (current.Next != null)
                        current.Next.Previous = current.Previous;
                    var v = _start.Value;
                    _start = current;
                    return v;
                }
                current = current.Next;
            }
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Merges given collection into the current colelction
        /// </summary>
        /// <param name="other">Same type collection</param>
        /// <param name="skipEquals">If set to false. It will replace values with values from other collection if keys are equal</param>
        public void Merge(MyCollection<K, V> other, bool skipEquals = true)
        {
            // if only i was able to iterate throw collection....
            var current = other._start; 
            while (current != null)
            {
                if (!this.ContainsKey(current.Key) || !skipEquals)
                    this.Add(current.Key, current.Value);
                current = current.Next;
            }
        }

        private void Show()
        {
            var s = _start;
            while (s != null)
            {
                Console.WriteLine(s.Key + " " + s.Value);
                s = s.Next;
            }
            Console.WriteLine(); 
        }

        // From there it is IEnumerable and IEnumerator implementation

        public bool MoveNext()
        {
            if (current == null)
            {
                current = _start;
            }
            else
            {
                current = current.Next;
            }
            return current != null;
        }

        public void Reset()
        {
            current = null;
        }

        public void Dispose() // It is not "required" by me and works without it
        {
            throw new NotImplementedException();
        }

        IEnumerator<Node<K, V>> IEnumerable<Node<K, V>>.GetEnumerator()
        {
            if (current == null)
                MoveNext();
            while (current != null)
            {
                yield return current;
                current = current.Next; 
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Node<K, V>>)this).GetEnumerator();
        }
    }
}
