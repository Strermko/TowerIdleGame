using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")] [SerializeField]
    private string fileName;

    public static DataPersistenceManager Instance { get; private set; }

    private GameData _gameData;
    private List<IDataPersistance> _dataPersistenceObjects;
    private FileDataHandler _fileDataHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There can only be one DataPersistenceManager!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _dataPersistenceObjects = FindAllDataPersistanceObjects();
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath ,fileName);
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        _gameData = new GameData();
    }

    public void SaveGame()
    {
        foreach (var dataPersistenceObject in _dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref _gameData);
        }
        
        _fileDataHandler.Save(_gameData);
    }

    public void LoadGame()
    {
        _gameData = _fileDataHandler.Load();
        
        //if no data founded
        if (_gameData == null)
        {
            Debug.Log("No game data to load!");
            NewGame();
            return;
        }
        
        foreach (var dataPersistenceObject in this._dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(_gameData);
        }
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        var dataPersistenceObjects =
            FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistenceObjects);
    }
}