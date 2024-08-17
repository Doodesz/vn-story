using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private string newGameScene;

    private void Start() 
    {
        if (!DataPersistenceManager.instance.HasGameData()) 
        {
            continueGameButton.interactable = false;
        }
    }

    public void OnNewGameClicked() 
    {
        StartCoroutine(StartGame(true));
    }

    public void OnContinueGameClicked() 
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame(bool newGame = false)
    {
        DisableMenuButtons();
        ScreenTransition.instance.PlayTransitionOut();

        yield return new WaitForSecondsRealtime(1f);

        if (newGame)
        {
            // create a new game - which will initialize our game data
            DataPersistenceManager.instance.NewGame();
            // load the gameplay scene - which will in turn save the game because of
            // OnSceneUnloaded() in the DataPersistenceManager
            SceneManager.LoadSceneAsync(newGameScene);
        }
        else
        {
            // load the next scene - which will in turn load the game because of 
            // OnSceneLoaded() in the DataPersistenceManager
            SceneManager.LoadSceneAsync(DataPersistenceManager.instance.GetGameData().sceneName);
            DataPersistenceManager.instance.LoadGame();
        }
    }

    private void DisableMenuButtons() 
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }
}
