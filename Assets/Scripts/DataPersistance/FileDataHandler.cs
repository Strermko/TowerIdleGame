using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public class FileDataHandler
{
    private string _dataDirPath;
    private string _dataFileName;


    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);
        GameData loadedData = null;
        
        if (File.Exists(fullPath))
        {
            try
            {
                string dataAsJson = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataAsJson = reader.ReadToEnd();
                    }
                }
                
                loadedData = JsonConvert.DeserializeObject<GameData>(dataAsJson);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading data from path: {fullPath} \n {e.Message}");
            }
        }

        return loadedData;
    }

    public void Save(GameData gameData)
    {
        //used Path.Combine to avoid problems with different separators in OS
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);
        try
        {
            //create directory and file if not exists
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);

            //serialize C# data to JSON
            string jsonedData = JsonConvert.SerializeObject(gameData, Formatting.Indented);

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.Write(jsonedData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while saving data in path: {fullPath} \n {e.Message}");
        }
    }
}