public class DictionaryNode<TKey, TValue>
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }
    public int HashCode { get; set; }
    public DictionaryNode<TKey, TValue> Next  { get; set; }

    public DictionaryNode(TKey key, TValue value, DictionaryNode<TKey, TValue> next = null)
    {
        Key = key;
        Value = value;
        Next = next;
        HashCode = 0;
    }
}