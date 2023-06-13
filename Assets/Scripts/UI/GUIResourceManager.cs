using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GUIResourceManager : MonoBehaviour, IDataPersistance
{
    private Dictionary<ResourceType, ResourceComponent> _resourceComponents = new Dictionary<ResourceType, ResourceComponent>();

    private void Awake()
    {
        Array allResources = GetComponentsInChildren<ResourceComponent>();
        foreach (ResourceComponent resource in allResources)
        {
            _resourceComponents.Add(resource.resourceType, resource);
        }
    }

    private void Start()
    {
        GameEventManager.Instance.addResource += AddResource;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.addResource -= AddResource;
    }
    
    private void AddResource(ResourceType resourceType, int value)
    {
        _resourceComponents[resourceType].UpdateComponent(value);
    }

    public void LoadData(GameData gameData)
    {

        foreach (var resource in _resourceComponents)
        {
            gameData.resources.TryGetValue(resource.Key, out int value);
            resource.Value.UpdateComponent(value);
        }
    }

    public void SaveData(ref GameData gameData)
    {
        foreach (var resource in _resourceComponents)
        {
            gameData.resources[resource.Key] = resource.Value.currentValue;
        }
    }
}
