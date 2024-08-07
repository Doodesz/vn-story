using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    public void OnMainMenuClicked()
    {
        StartCoroutine(GoToMainMenu());
    }

    IEnumerator GoToMainMenu()
    {
        ScreenTransition.instance.PlayTransitionOut();
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
