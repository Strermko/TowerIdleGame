using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public Dictionary<ResourceType, int> Resources = new();

    public GameData()
    {
        foreach (ResourceType resource in System.Enum.GetValues(typeof(ResourceType)))
        {
            Resources.Add(resource, 0);
        }
    }
}
