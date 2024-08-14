using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum InteractableType { Dialogue, ChangeArea, SwitchScenes };

public class Interactable : MonoBehaviour
{
    [Header("Assign GameObjects")]
    public GameObject objectiveIndicator;

    [Header("This Interactable Parameters")]
    public InteractableType thisInteractableType;
    public string sceneDestination;
    public string areaDestination;
    public bool triggerOnTriggerEnter = false;

    [Header("Task Parameters")]
    public bool hasBeenCompleted = false;
    
    public void TriggerInteraction()
    {
        if (thisInteractableType == InteractableType.Dialogue)
        {
            GetComponent<DialogueTrigger>().TriggerDialogue();
        }

        else if (thisInteractableType == InteractableType.SwitchScenes)
        {
            SwitchScene();
        }

        else if (thisInteractableType == InteractableType.ChangeArea)
        {
            Debug.Log("InteractableType.ChangeArea has not yet been coded.");
            // ChangeArea(); // unused
        }

        if (IsTaskObject())
        {
            hasBeenCompleted = true;
            TasksManager.instance.UpdateTaskList();
        }
    }

    private void SwitchScene()
    {
        if (thisInteractableType == InteractableType.SwitchScenes)
        {
            GameManager.Instance.doorDestination = gameObject.name;
            GameManager.Instance.playerChangingMap = true;
            SceneManager.LoadSceneAsync(sceneDestination, LoadSceneMode.Single);
        }
    }

    public void ShowInteractPrompt()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void HideInteractPrompt()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private bool IsTaskObject()
    {
        List<TaskObject> taskList = TasksManager.instance.GetTaskList();

        foreach (TaskObject taskObject in taskList)
            if (taskObject.taskObject == this.gameObject)
            {
                Debug.Log("Is a task object");
                return true;
            }

        Debug.Log("Is not a task object");
        return false;
    }
}
