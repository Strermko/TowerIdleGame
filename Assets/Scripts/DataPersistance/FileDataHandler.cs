using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public class FileDataHandler
{
    private string _dataDirPath;
    private string _dataFileName;
    private bool _useEncryption;
    private readonly string _encryptionCodeWord = "guineapig";


    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption = false)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
        _useEncryption = useEncryption;
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

                if (_useEncryption) dataAsJson = EncryptDecrypt(dataAsJson);
                
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

            if (_useEncryption) jsonedData = EncryptDecrypt(jsonedData);

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

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char) (data[i] ^ _encryptionCodeWord[i % _encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}