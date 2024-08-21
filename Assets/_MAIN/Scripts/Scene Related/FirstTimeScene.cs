using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTimeScene : MonoBehaviour, IDataPersistence
{
    [Header("Assign GameObjects")]
    public GameObject firstTimeSceneScreen;
    public PlayerController playerController;

    [Header("This Scene Parameters")]
    public bool isFirstTimeInScene = true;
    public bool hasFirstTimeSceneAction = false;

    [SerializeField] private bool hasPendingFirstTimeSceneAction = false;

    public static FirstTimeScene instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        isFirstTimeInScene = false;
    }

    private void Update()
    {
        if (hasPendingFirstTimeSceneAction
            && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ))
        {
            InitializeFirstTimeSceneBehaviour();
            //DataPersistenceManager.instance.SaveGame();
        }
    }

    public void InitializeFirstTimeSceneBehaviour()
    {
        hasPendingFirstTimeSceneAction = false;
        playerController.isPlayerInControl = false;
        isFirstTimeInScene = false;
        hasFirstTimeSceneAction = false;

        GetComponent<DialogueTrigger>().TriggerDialogue();

        StartCoroutine(HideFirstTimeSceneScreen());

        DataPersistenceManager.instance.SaveGame();
    }

    private IEnumerator HideFirstTimeSceneScreen()
    {
        firstTimeSceneScreen.GetComponent<Animator>().Play("hide");

        yield return new WaitForSecondsRealtime(2f);

        firstTimeSceneScreen.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        this.isFirstTimeInScene = data.isFirstTimeInScene;
        //this.hasFirstTimeSceneAction = data.hasFirstTimeSceneAction;

        if (isFirstTimeInScene && hasFirstTimeSceneAction)
        {
            firstTimeSceneScreen.SetActive(true);
            hasPendingFirstTimeSceneAction = true;
        }
        else
        {
            firstTimeSceneScreen.SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        data.isFirstTimeInScene = this.isFirstTimeInScene;
        //data.hasFirstTimeSceneAction = this.hasFirstTimeSceneAction;
    }
}