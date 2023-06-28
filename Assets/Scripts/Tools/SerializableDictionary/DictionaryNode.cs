using System;

[Serializable]
public class DictionaryNode<TKey, TValue>
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }
    public int HashCode { get; set; }

    public DictionaryNode(TKey key, TValue value)
    {
        Key = key;
        Value = value;
        HashCode = key.GetHashCode();
    }
}