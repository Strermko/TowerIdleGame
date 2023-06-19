public class DictionaryNode<TKey, TValue>
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }
    public DictionaryNode<TKey, TValue> Next  { get; set; }
    public DictionaryNode<TKey, TValue> Previous { get; set; }
    
    public DictionaryNode(TKey key, TValue value)
    {
        Key = key;
        Value = value;
        Next = null;
        Previous = null;
    }
}