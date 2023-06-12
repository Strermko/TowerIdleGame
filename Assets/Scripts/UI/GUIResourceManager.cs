using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIResourceManager : MonoBehaviour, IDataPersistance
{
    private Dictionary<ResourceType, ResourceComponent> _resourceComponents = new Dictionary<ResourceType, ResourceComponent>();

    private void Start()
    {
        Array allResources = GetComponentsInChildren<ResourceComponent>();
        foreach (ResourceComponent resource in allResources)
        {
            Debug.Log(resource.resourceType);
            _resourceComponents.Add(resource.resourceType, resource);
        }
        GameEventManager.Instance.addResource += AddResource;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.addResource -= AddResource;
    }
    
    private void AddResource(ResourceType resourceType, int value)
    {
        ResourceComponent resourceComponent = _resourceComponents[resourceType];
        resourceComponent.UpdateComponent(value);
    }

    public void LoadData(GameData gameData)
    {
        //textField.text = gameData.resources[resourceType].ToString();
    }

    public void SaveData(ref GameData gameData)
    {
        throw new System.NotImplementedException();
    }
}
