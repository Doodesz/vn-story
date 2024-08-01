using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum InteractableType { Dialogue, ChangeArea, SwitchScenes };

public class Interactable : MonoBehaviour
{
    public InteractableType thisInteractableType;
    public string sceneDestination;
    public string areaDestination;
    public bool triggerOnTriggerEnter = false;

    public void SwitchScene()
    {
        if (thisInteractableType == InteractableType.SwitchScenes)
        {
            GameManager.Instance.doorDestination = gameObject.name;
            GameManager.Instance.playerChangingMap = true;
            SceneManager.LoadSceneAsync(sceneDestination, LoadSceneMode.Single);
        }
    }
}
