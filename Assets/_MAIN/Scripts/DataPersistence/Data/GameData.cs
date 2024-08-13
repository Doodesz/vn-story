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
    public int lastDialogueLineIndex;
    public string npcBeingInteracted;
    public string sceneName;
    public string currentArea;

    // public SerializableDictionary<string, bool> coinsCollected;
    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        playerInControl = true;
        lastDialogueLineIndex = 0;
        playerPosition = Vector3.zero;
        npcBeingInteracted = null;
        isInDialogue = false;
        sceneName = "Prologue";
        currentArea = "0";
        isFirstTimeInScene = true;

        // coinsCollected = new SerializableDictionary<string, bool>();
    }
}
