using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterButtons : MonoBehaviour
{
    [Tooltip("Determines which scene to go to")]
    public string chapterName;

    public void NewSpecificChGame()
    {
        GameManager.instance.GoToScene(chapterName, true);
    }
}
