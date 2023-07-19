using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIResourceManager : MonoBehaviour, IDataPersistance
{
    private Dictionary<ResourceType, ResourcesUI> _resourceComponents = new();

    private void Awake()
    {
        Array allResources = GetComponentsInChildren<ResourcesUI>();
        foreach (ResourcesUI resource in allResources)
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
            var resourceData = gameData.GetResourceByType(resource.Key);
            resource.Value.UpdateComponent(resourceData.Value);
        }
    }

    public void SaveData(ref GameData gameData)
    {
        foreach (var resource in _resourceComponents)
        {
            var resourceData = gameData.GetResourceByType(resource.Key);
            resourceData.value = resource.Value.currentValue;
        }
    }
}
