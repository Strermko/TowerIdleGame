using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> _keys = new List<TKey>();

    [SerializeField]
    private List<TValue> _values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach (var pair in this)
        {
            _keys.Add(pair.Key);
            _values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < _keys.Count; i++)
        {
            this.Add(_keys[i], _values[i]);
        }
    }

    public List<DictionaryNode<TKey, TValue>> GetPairs()
    {
        var pairs = new List<DictionaryNode<TKey, TValue>>();
        for (int i = 0; i < _keys.Count; i++)
        {
            pairs.Add(new DictionaryNode<TKey, TValue>(_keys[i], _values[i]));
        }

        return pairs;
    }
}