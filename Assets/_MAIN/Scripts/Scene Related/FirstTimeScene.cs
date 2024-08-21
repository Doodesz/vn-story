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

    public static FirstTimeScene instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (hasFirstTimeSceneAction 
            && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ) )
        {
            isFirstTimeInScene = false;
            hasFirstTimeSceneAction = false;
            GetComponent<DialogueTrigger>().TriggerDialogue();

            StartCoroutine(HideFirstTimeSceneScreen());
        }
    }

    public void InitializeFirstTimeSceneBehaviour()
    {
        playerController.isPlayerInControl = false;
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

        if (isFirstTimeInScene && hasFirstTimeSceneAction)
        {
            firstTimeSceneScreen.SetActive(true);
            data.playerInControl = false;
            InitializeFirstTimeSceneBehaviour();
        }
        else if (!hasFirstTimeSceneAction)
        {
            firstTimeSceneScreen.SetActive(false);
            //this.enabled = false;
            data.playerInControl = true;
        }
    }

    public void SaveData(GameData data)
    {
        data.isFirstTimeInScene = this.isFirstTimeInScene;
    }
}