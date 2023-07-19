using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    [SerializeField]
    public List<SerializableResource> resources = new();

    public GameData()
    {
        foreach (ResourceType resource in System.Enum.GetValues(typeof(ResourceType)))
        {
            resources.Add(new SerializableResource(resource, 0));
        }
    }

    public SerializableResource GetResourceByType(ResourceType resourceType)
    {
        return resources.Find(x => x.Resource == resourceType);
    }
}

[System.Serializable]
public class SerializableResource
{
    public ResourceType resourceType;
    public int value;

    public ResourceType Resource => resourceType;
    public int Value => value;

    public SerializableResource(ResourceType resourceType, int value)
    {
        this.resourceType = resourceType;
        this.value = value;
    }

    public void IncreaseResource(int value)
    {
        this.value += value;
    }

    public void DecreaseResource(int value)
    {
        this.value = Value - value < 0 ? 0 : Value - value;
    }
}