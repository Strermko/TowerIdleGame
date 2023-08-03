using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    [SerializeField]
    protected List<TKey> keys = new();

    [SerializeField]
    protected List<TValue> values = new();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

    public List<DictionaryNode<TKey, TValue>> GetPairs()
    {
        var pairs = new List<DictionaryNode<TKey, TValue>>();
        for (int i = 0; i < keys.Count; i++)
        {
            pairs.Add(new DictionaryNode<TKey, TValue>(keys[i], values[i]));
        }

        return pairs;
    }
}