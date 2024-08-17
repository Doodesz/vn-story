using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// public enum ButtonType { Save, Load, ReviewDialogue, ExitToMainMenu, Settings }
public class ButtonManager : MonoBehaviour
{
    // unused

    /*[SerializeField] private ButtonType thisButtonType;

    public void TriggerButton()
    {
        switch (thisButtonType)
        {
            case ButtonType.Save:
                OnSaveClicked();
                break;
            case ButtonType.Load:
                // load game
                break;
            case ButtonType.ExitToMainMenu:
                OnMainMenuClicked();
                break;
            case ButtonType.Settings:
                // open settings;
                break;
            case ButtonType.ReviewDialogue:
                // open dialogue review panel;
                break;
            default:
                break;
        }
    }*/

    public void OnSaveClicked()
    {
        DataPersistenceManager.instance.SaveGame();
    }

    public void OnMainMenuClicked()
    {
        DataPersistenceManager.instance.SaveGame();
        StartCoroutine(GoToMainMenu());
    }

    IEnumerator GoToMainMenu()
    {
        ScreenTransition.instance.PlayTransitionOut();
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
