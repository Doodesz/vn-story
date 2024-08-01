using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour, IDataPersistence
{
    [Header("Current Map Info")]
    public float leftBoundary;
    public float rightBoundary;
    public string currentArea;
    [SerializeField] private string sceneName;

    [Header("Assign Area GameObject")]
    [SerializeField] private List<GameObject> areaGameObjects;

    public static MapManager Instance;

    private void Awake()
    {
        Instance = this;
        sceneName = SceneManager.GetActiveScene().name;
    }

    public void SaveData(GameData data)
    {
        data.currentArea = this.currentArea;
        data.sceneName = this.sceneName;
    }

    public void LoadData(GameData data)
    {
        this.currentArea = data.currentArea;
    }
}
