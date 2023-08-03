using System;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There can only be one GameEventManager!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public event Action<ResourceType, int> addResource;
    public void AddResource(ResourceType type, int value)
    {
        addResource?.Invoke(type, value);
    }
    
    public event Action<ResourceType, int> buyUpgrade;
    public void BuyUpgrade(ResourceType type, int value)
    {
        buyUpgrade?.Invoke(type, value);
    }
}