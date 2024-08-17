using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    [Tooltip("Determines which scene to go to")]
    public string chapterName;

    private void Start()
    {
        GetComponent<ContinueButton>().chapterName = DataPersistenceManager.instance.gameData.newSceneName;
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
