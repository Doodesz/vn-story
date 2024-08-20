using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Debugging")]
    public string doorDestination;
    public bool playerChangingMap = false;
    public bool isGamePaused = false;

    [SerializeField] Vector3 doorPos; // unused
    [SerializeField] DataPersistenceManager dataPersistenceManager;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Start()
    {
        dataPersistenceManager = DataPersistenceManager.instance;
    }

    public void UpdateDoorDestinationPos(Vector3 pos)
    {
        doorPos = pos;
    }

    public void GoToScene(string scene, bool initNewData = false)
    {
        if (initNewData)
        {
            DataPersistenceManager.instance.NewGame(scene);
            DataPersistenceManager.instance.SaveGame();
            Debug.Log("Line hit");
        }

        Debug.Log("Line hit");

        // Disables all UI button to avoid unexpected bugs
        List<Button> UIbuttons = new(GameObject.FindObjectsOfType<Button>());

        foreach (Button button in UIbuttons)
        {
            button.interactable = false;
        }

        ScreenTransition.instance.PlayTransitionOut();
        StartCoroutine(LoadScene(scene));

    }

    private IEnumerator LoadScene(string scene)
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(scene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        List<Button> UIbuttons = new (GameObject.FindObjectsOfType<Button>());

        foreach (Button button in UIbuttons)
        {
            button.interactable = true;
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {

    }
}
