using System;
using UnityEngine;

public class DataPersistanceManager : MonoBehaviour
{
    public static DataPersistanceManager instance { get; private set; }
    
    private GameData gameData;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There can only be one DataPersistanceManager!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void SaveGame()
    {
        //TODO: Collect data from other scripts
        
        //TODO: Save data to file
    }

    public void LoadGame()
    {
        //TODO: Load game data from file
        
        //if no data founded
        if (this.gameData == null)
        {
            Debug.Log("No game data to load!");
            NewGame();
            return;
        }
        
        //TODO: pass loaded data to other scripts
    }
}