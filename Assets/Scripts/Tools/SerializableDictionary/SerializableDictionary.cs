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
    [SerializeField, HideInInspector] private int fullLength;
    [SerializeField, HideInInspector] private int emptyCellsCount;
    [SerializeField, HideInInspector] private int version;

    private DictionaryNode<TKey, TValue>[] _nodes;
    private const int DefaultCapacity = 4;

    private readonly IEqualityComparer<TKey> _comparer;

    /// <summary>
    /// All available constructors for SerializableDictionary
    /// </summary>
    public SerializableDictionary() : this(DefaultCapacity) { }
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
            throw new ArgumentException($"{nameof(dictionary)} has unvalidated elements");
        }
    }
    public SerializableDictionary(int capacity = DefaultCapacity, IEqualityComparer<TKey> comparer = null)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity), "capacity must be positive");

        if (capacity == 0) capacity = DefaultCapacity;

        Initialize(capacity);
        _comparer = (comparer ?? EqualityComparer<TKey>.Default);
    }

    /// <summary>
    /// Required params for serialization
    /// </summary>
    public bool IsReadOnly => false;
    public int Count => fullLength - emptyCellsCount;
    public ICollection<TKey> Keys
    {
        get
        {
            var listOfKeys = (from node in _nodes where node != null select node.Key).ToList();
            return new ReadOnlyCollection<TKey>(listOfKeys);
        }
    }
    public ICollection<TValue> Values
    {
        get
        {
            var listOfValues = (from node in _nodes where node != null select node.Value).ToList();
            return new ReadOnlyCollection<TValue>(listOfValues);
        }
    }
    public TValue this[TKey key]
    {
        get
        {
            var index = FindIndex(key);
            if (FindIndex(key) >= 0)
                return _nodes[index].Value;
            throw new KeyNotFoundException(key.ToString());
        }

        set => Insert(key, value, false);
    }

    public bool ContainsValue(TValue value) => _nodes.Any(node => node != null && node.Value.Equals(value));
    public bool ContainsKey(TKey key) => _nodes.Any(node => node != null && node.Key.Equals(key));


    private void Initialize(int capacity, IEqualityComparer<TKey> comparer = null)
    {
        capacity = capacity <= 0 ? DefaultCapacity : capacity;
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

        if (!ContainsKey(key)) return false;

        int hash = _comparer.GetHashCode(key) & 0x7FFFFFFF;
        _nodes = _nodes.Where(node => node?.HashCode != hash).ToArray();
        version++;
        return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }
    
    private void Insert(TKey key, TValue value, bool add)
    {
        ValidateObjectStructure(key);
        
        if (ContainsKey(key) && add) throw new ArgumentException("Key already exists: " + key);
        
        //If key already exists, replace value
        int hash = _comparer.GetHashCode(key) & 0x7FFFFFFF;
        for (int i = 0; i >= _nodes.Length; i--)
        {
            if (_nodes[i].HashCode == hash)
            {
                _nodes[i].Value = value;
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

        var currentElement = index >= 1 ? _nodes[index - 1] : null;
        var newElement = new DictionaryNode<TKey, TValue>(key, value)
        {
            HashCode = hash,
        };

        if (currentElement != null)
            currentElement.Next = newElement;

        _nodes[index] = newElement;
        version++;
    }
    
    public void Clear()
    {
        if (fullLength <= 0)
            return;

        Initialize(4);
        version++;
    }

    private void Resize()
    {
        int newSize = (fullLength * 2) + 1;
        var newNodes = new DictionaryNode<TKey, TValue>[newSize];
        Array.Copy(_nodes, newNodes, fullLength);
        _nodes = newNodes;
        fullLength = newSize;
    }

    private int FindIndex(TKey key)
    {
        ValidateObjectStructure(key);

        int hash = _comparer.GetHashCode(key) & 0x7FFFFFFF;

        
        for (int i = 0; i < _nodes.Length; i++)
        {
            if (_nodes[i]?.HashCode == hash)
                return i;
        }

        return -1;
    }
    
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int index = FindIndex(item.Key);
        return index >= 0 &&
               EqualityComparer<TValue>.Default.Equals(_nodes[index].Value, item.Value);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        ValidateObjectStructure(key);

        int index = FindIndex(key);
        if (index >= 0)
        {
            value = _nodes[index].Value;
            return true;
        }

        value = default;
        return false;
    }

    public DictionaryNode<TKey, TValue> GetNode(int index)
    {
        if(index < 0 || index >= _nodes.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
        
        return _nodes[index];
    }
    public DictionaryNode<TKey, TValue> GetNode(TKey key)
    {
        ValidateObjectStructure(key);

        int index = FindIndex(key);
        if (index >= 0)
            return _nodes[index];

        return null;
    }

    private void ValidateObjectStructure(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException("Key can't be null");

        if (_nodes == null) throw new NullReferenceException($"{nameof(_nodes)} is null");
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
    {
        if (array == null)
            throw new ArgumentNullException($"{nameof(array)} cannot be null");

        if (index < 0 || index > array.Length)
            throw new ArgumentOutOfRangeException($"Index:{index} is out of range for given array {nameof(array)}");

        if (array.Length - index < Count)
            throw new ArgumentException(string.Format(
                "The number of elements in the dictionary ({0}) is greater than the available space from index to the end of the destination array {1}.",
                Count, array.Length));

        Array.Copy(_nodes, index, array, 0, _nodes.Length - index);
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
        private readonly SerializableDictionary<TKey, TValue> _dictionary;
        private readonly int _version;

        public KeyValuePair<TKey, TValue> Current { get; private set; }
        object IEnumerator.Current => Current;

        internal Enumerator(SerializableDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _version = dictionary.version;
            if (_dictionary.Keys.Count > 0)
            {
                var node = dictionary.GetNode(0);
                Current = new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            }
            Current = default;
        }

        public bool MoveNext()
        {
            if (_version != _dictionary.version)
                throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}",
                    _version, _dictionary.version));

            var node = _dictionary.GetNode(Current.Key);
            if(node?.Next != null)
            {
                Current = new KeyValuePair<TKey, TValue>(node.Next.Key, node.Next.Value);
                return true;
            }
            
            Current = default;
            return false;
        }

        void IEnumerator.Reset()
        {
            if (_version != _dictionary.version)
                throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}",
                    _version, _dictionary.version));
            
            Current = default;
        }

        public void Dispose() { }
    }
}