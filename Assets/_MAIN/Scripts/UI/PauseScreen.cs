using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public void TogglePauseMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        GameManager.instance.isGamePaused = gameObject.activeSelf;
    }
}
