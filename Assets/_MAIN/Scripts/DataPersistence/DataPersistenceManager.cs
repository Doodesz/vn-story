using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    public GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake() 
    {
        if (instance != null) 
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        /*if (SceneManager.GetActiveScene().name != "MainMenu")
            SaveGame();*/
    }

    public void NewGame(string chapterName = "Prologue") 
    {
        this.gameData = new GameData();
        this.gameData.sceneName = chapterName;
        
        SaveGame();
    }

    public void LoadGame()
    {
        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();

        // start a new game if the data is null and we're configured to initialize data for debugging purposes
        if (this.gameData == null && initializeDataIfNull) 
        {
            NewGame();
        }

        // if no data can be loaded, don't continue
        if (this.gameData == null) 
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }

        // If the data has the isGoingToNewScene on true, make a new game data with specific datas
        // related to scenes changed
        if (this.gameData.isGoingToNewScene)
        {
            NewGame(gameData.newSceneName);
        }

        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        // if we don't have any data to save, log a warning here
        if (this.gameData == null) 
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            // pass the data to other scripts so they can update it
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
            {
                dataPersistenceObj.SaveData(gameData);
            }

        }

        // save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit() 
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData() 
    {
        return gameData != null;
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    private void InitiateNewSceneData(string sceneName)
    {
        gameData = new GameData();

        gameData.sceneName = sceneName;
        gameData.isFirstTimeInScene = true;
        SaveGame();
    }
}
