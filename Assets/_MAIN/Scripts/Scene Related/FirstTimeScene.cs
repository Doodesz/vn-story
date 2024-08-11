using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTimeScene : MonoBehaviour, IDataPersistence
{
    public bool isFirstTimeInScene = true;
    public bool hasFirstTimeSceneAction = false;
    public GameObject firstTimeSceneScreen;

    private void Update()
    {
        if (isFirstTimeInScene && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ) )
        {
            GetComponent<DialogueTrigger>().TriggerDialogue();
            isFirstTimeInScene = false;

            StartCoroutine(HideFirstTimeSceneScreen());
        }
    }

    public void InitializeFirstTimeSceneBehaviour()
    {
        PlayerController.Instance.playerInControl = false;
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
            InitializeFirstTimeSceneBehaviour();
        }
        else
        {
            firstTimeSceneScreen.SetActive(false);
            this.enabled = false;
        }
    }

    public void SaveData(GameData data)
    {
        data.isFirstTimeInScene = this.isFirstTimeInScene;
    }
}
