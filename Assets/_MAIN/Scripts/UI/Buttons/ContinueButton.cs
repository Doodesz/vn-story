using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    [Tooltip("Determines which scene to go to")]
    public string chapterName;
    public string text;
    
    ContinueButton continueButton;

    private void Start()
    {   
        continueButton = GetComponent<ContinueButton>();
        continueButton.chapterName = DataPersistenceManager.instance.gameData.newSceneName;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DataPersistenceManager.instance.gameData.interSceneText;
    }

    public void NewSpecificChGame()
    {
        GameManager.instance.GoToScene(chapterName, true);
    }

    public void LoadData(GameData data)
    {
        this.chapterName = data.newSceneName;
    }

    public void SaveData(GameData data)
    {
        data.newSceneName = this.chapterName;
    }
}
