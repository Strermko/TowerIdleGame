using System;

[Serializable]
public class DictionaryNode<TKey, TValue>
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }

    public DictionaryNode(TKey key, TValue value, DictionaryNode<TKey, TValue> next = null)
    {
        Key = key;
        Value = value;
    }
}