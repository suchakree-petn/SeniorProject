using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace DataPersistence
{

    public class FileDataHandler<T>
    {
        public string dataDirPath = "";
        public string dataFileName = "";

        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
        }

        public T Load()
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            T loadedData = default;
            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = "";
                    using (FileStream stream = new(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                        loadedData = JsonUtility.FromJson<T>(dataToLoad);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while trying to load data from file" + fullPath + "\n" + e);
                }
            }
            else
            {
                Debug.LogWarning("Path not exist");
            }
            return loadedData;
        }
        public void Save(T data)
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonUtility.ToJson(data, true);
                using (FileStream stream = new(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error while trying to save data to file" + fullPath + "\n" + e);
            }
        }
    }
}
