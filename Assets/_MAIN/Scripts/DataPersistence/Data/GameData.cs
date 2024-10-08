using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public bool playerInControl;
    public bool isInDialogue;
    public bool isFirstTimeInScene;
    //public bool hasFirstTimeSceneAction;
    public bool hasPendingTaskListUpdate;
    public int objectiveIndex;
    public int lastDialogueIndex;
    public int lastDialogueLineIndex;
    public int lastTaskIndex;
    public string npcBeingInteracted;
    public string sceneName;
    public string currentArea;
    public string interSceneText;
    public List<TaskItem> taskItemsList;
    public SerializableDictionary<string, bool> taskObjectsDictionary;

    // Important for changing scenes to work
    public bool isGoingToNewScene;
    public string newSceneName;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        playerPosition = Vector3.zero;
        playerInControl = true;
        isInDialogue = false;
        isFirstTimeInScene = true;
        //hasFirstTimeSceneAction = false;
        objectiveIndex = 0;
        lastDialogueIndex = 0;
        lastDialogueLineIndex = 0;
        lastTaskIndex = 0;
        npcBeingInteracted = null;
        sceneName = "Prologue";
        currentArea = "0";
        interSceneText = string.Empty;
        taskItemsList = new List<TaskItem>();
        taskObjectsDictionary = new SerializableDictionary<string, bool>();

        isGoingToNewScene = false;
        newSceneName = string.Empty;
    }
}
