using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    public GameData()
    {
        foreach (ResourceType resource in System.Enum.GetValues(typeof(ResourceType)))
        {
            resources.Add(resource, 0);
        }
    }
}
