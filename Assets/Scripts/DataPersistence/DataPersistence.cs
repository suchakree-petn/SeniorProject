using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace DataPersistence
{
    public class DataPersistence<T> where T : ISaveData
    {
        private T gameData;
        public FileDataHandler<T> dataHandler;

        public static Action OnLoadSuccess;


        public DataPersistence(string fileName, T defaultData)
        {
            gameData = defaultData;
            dataHandler = new(Application.persistentDataPath, fileName + "." + "SAVE");
        }

        public void NewData()
        {

        }

        public void LoadData()
        {
            List<IDataPersistence<T>> dataPersistencesObjects = FindAllDataPersistenceObjects();

            // load saved data
            T _gameData = dataHandler.Load();


            // if no data can be loaded, init a new game data
            if (_gameData == null)
            {
                Debug.LogWarning("No data was found");
            }
            else
            {
                gameData = _gameData;
            }

            //push loaded data to other scripts that need
            foreach (IDataPersistence<T> dataPersistenceObj in dataPersistencesObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }
            Debug.Log("Loaded Player data");
            OnLoadSuccess?.Invoke();

            if(_gameData == null){
                SaveData();
            }

        }

        public void SaveData()
        {

            List<IDataPersistence<T>> dataPersistencesObjects = FindAllDataPersistenceObjects();

            //pass loaded data to other scripts so  they can update it
            foreach (IDataPersistence<T> dataPersistenceObj in dataPersistencesObjects)
            {
                dataPersistenceObj.SaveData(ref gameData);
            }

            //save data to file using data handler
            dataHandler.Save(gameData);
            Debug.Log("Saved Player data");

        }

        private List<IDataPersistence<T>> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistence<T>> dataPersistenceObjects = GameObject.FindObjectsOfType<MonoBehaviour>()
                .OfType<IDataPersistence<T>>();
            return new List<IDataPersistence<T>>(dataPersistenceObjects);
        }

    }
}
