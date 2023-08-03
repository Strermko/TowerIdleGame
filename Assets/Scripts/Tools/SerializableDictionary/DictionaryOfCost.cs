using System;

[Serializable]
public class DictionaryOfCost : SerializableDictionary<ResourceType, int>
{
    public void UpCost(float scale)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            values[i] = (int) Math.Floor(values[i] * scale);
        }
    }
}