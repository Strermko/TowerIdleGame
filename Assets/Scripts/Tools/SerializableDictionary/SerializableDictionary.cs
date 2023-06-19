using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    [SerializeField, HideInInspector] private List<TKey> keys;
    [SerializeField, HideInInspector] private List<TValue> values;
    [SerializeField, HideInInspector] private int fullLength;
    [SerializeField, HideInInspector] private int version;
    [SerializeField, HideInInspector] private int emptyCellsCount;

    private DictionaryNode<TKey, TValue>[] _nodes;
    private const int DefaultCapacity = 4;

    private readonly IEqualityComparer<TKey> _comparer;

    public SerializableDictionary() : this(DefaultCapacity) { }

    public SerializableDictionary(int capacity = DefaultCapacity, IEqualityComparer<TKey> comparer = null)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity), "capacity must be positive");

        if (capacity == 0) capacity = DefaultCapacity;

        Initialize(capacity);
        _comparer = (comparer ?? EqualityComparer<TKey>.Default);
    }

    public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
    {
        if (dictionary == null) throw new ArgumentException($"{nameof(dictionary)} cannot be null");
        try
        {
            Initialize(dictionary.Count);
            foreach (var current in dictionary)
                Add(current.Key, current.Value);
        }
        catch (Exception)
        {
            //Clear();
            throw new ArgumentException($"{nameof(dictionary)} has unvalidated elements");
        }
    }

    public bool IsReadOnly => false;
    public int Count => fullLength - emptyCellsCount;
    public bool ContainsValue(TValue value) => values.Contains(value);
    public bool ContainsKey(TKey key) => keys.Contains(key, _comparer);
    public ICollection<TKey> Keys => new ReadOnlyCollection<TKey>(keys);
    public ICollection<TValue> Values => new ReadOnlyCollection<TValue>(values);

    public TValue this[TKey key]
    {
        get
        {
            var index = FindIndex(key);
            if (FindIndex(key) >= 0)
                return values[index];
            throw new KeyNotFoundException(key.ToString());
        }

        set => Insert(key, value, false);
    }



    private void Initialize(int capacity, IEqualityComparer<TKey> comparer = null)
    {
        keys = new List<TKey>(capacity);
        values = new List<TValue>(capacity);
        _nodes = new DictionaryNode<TKey, TValue>[capacity];
        fullLength = capacity;
        emptyCellsCount = capacity;
    }


    public void Add(TKey key, TValue value)
    {
        Insert(key, value, true);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Insert(item.Key, item.Value, true);
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException($"{key} cannot be null");

        int index = FindIndex(key);

        if (index < 0) return false;
        
        keys.RemoveAt(index);
        values.RemoveAt(index);
        return true;

    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    private void Insert(TKey key, TValue value, bool add)
    {
        ValidateObjectStructure(key);
        
        if(ContainsKey(key) && add) throw new ArgumentException("Key already exists: " + key);
        
        int hash = _comparer.GetHashCode(key) & 0x7FFFFFFF;
        for (int i = 0; i >= _nodes.Length; i--)
        {
            if (_nodes[i].Key.GetHashCode() == hash)
            {
                values[i] = value;
                version++;
                return;
            }
        }

        //Otherwise add new key and value
        int index;
        if (emptyCellsCount > 0)
        {
            index = Count;
            emptyCellsCount--;
        }
        else
        {
            index = fullLength;
            Resize();
        }

        _nodes[index] = new DictionaryNode<TKey, TValue>(key, value)
        {
            Previous = index >= 1 ? _nodes[index - 1] : null
        };
        
        keys[index] = key;
        values[index] = value;
    }

    private void Resize()
    {
        int newSize = (fullLength * 2) + 1;
        var newNodes = new DictionaryNode<TKey, TValue>[newSize];
        Array.Copy(_nodes, newNodes, fullLength);
        _nodes = newNodes;
        fullLength = newSize;
    }

    public void Clear()
    {
        if (fullLength <= 0)
            return;
        
        Initialize(0);
        version++;
    }

    private int FindIndex(TKey key)
    {
        ValidateObjectStructure(key);

        if(ContainsKey(key)) return keys.IndexOf(key);

        return -1;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        ValidateObjectStructure(key);
        
        
        int index = FindIndex(key);
        if (index >= 0)
        {
            value = _Values[index];
            return true;
        }

        value = default(TValue);
        return false;
    }


    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int index = FindIndex(item.Key);
        return index >= 0 &&
               EqualityComparer<TValue>.Default.Equals(_Values[index], item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
    {
        if (array == null)
            throw new ArgumentNullException("array");

        if (index < 0 || index > array.Length)
            throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));

        if (array.Length - index < Count)
            throw new ArgumentException(string.Format(
                "The number of elements in the dictionary ({0}) is greater than the available space from index to the end of the destination array {1}.",
                Count, array.Length));

        for (int i = 0; i < fullLength; i++)
        {
            if (_HashCodes[i] >= 0)
                array[index++] = new KeyValuePair<TKey, TValue>(_Keys[i], _Values[i]);
        }
    }
    
    private void ValidateObjectStructure(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException($"{key} cannot be null");

        if (_nodes == null) throw new NullReferenceException($"{nameof(_nodes)} is null");
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return GetEnumerator();
    }

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly SerializableDictionary<TKey, TValue> _Dictionary;
        private int _Version;
        private int _Index;
        private KeyValuePair<TKey, TValue> _Current;

        public KeyValuePair<TKey, TValue> Current
        {
            get { return _Current; }
        }

        internal Enumerator(SerializableDictionary<TKey, TValue> dictionary)
        {
            _Dictionary = dictionary;
            _Version = dictionary.version;
            _Current = default(KeyValuePair<TKey, TValue>);
            _Index = 0;
        }

        public bool MoveNext()
        {
            if (_Version != _Dictionary.version)
                throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}",
                    _Version, _Dictionary.version));

            while (_Index < _Dictionary.fullLength)
            {
                if (_Dictionary._HashCodes[_Index] >= 0)
                {
                    _Current = new KeyValuePair<TKey, TValue>(_Dictionary._Keys[_Index], _Dictionary._Values[_Index]);
                    _Index++;
                    return true;
                }

                _Index++;
            }

            _Index = _Dictionary.fullLength + 1;
            _Current = default(KeyValuePair<TKey, TValue>);
            return false;
        }

        void IEnumerator.Reset()
        {
            if (_Version != _Dictionary.version)
                throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}",
                    _Version, _Dictionary.version));

            _Index = 0;
            _Current = default(KeyValuePair<TKey, TValue>);
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose() { }
    }
}