using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum InteractableType { Dialogue, ChangeArea, SwitchScenes };

public class Interactable : MonoBehaviour
{
    [Header("This Interactable Parameters")]
    public InteractableType thisInteractableType;
    public string sceneDestination;
    public string areaDestination;
    public bool triggerOnTriggerEnter = false;
    
    [SerializeField] bool hasTaskObjectAttached;

    private void Awake()
    {
        if (GetComponent<TaskObject>() != null)
            hasTaskObjectAttached = true;
    }

    public void TriggerInteraction()
    {
        if (thisInteractableType == InteractableType.Dialogue)
        {
            if (hasTaskObjectAttached)
                GetComponent<DialogueTrigger>().TriggerDialogue(CheckDialogueIndexToTrigger());
            else
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

        // Order of execution is important, this is put down below because it needs to trigger the interaction first before updating object
        if (hasTaskObjectAttached)
        {
            GetComponent<TaskObject>().isCompleted = true;
            TasksManager.instance.UpdateTaskItemsList();
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

    private void TriggerDialogueBasedOnTaskCompleted()
    {
        // For every task item
        foreach (TaskItem taskItem in TasksManager.instance.taskItemsList)
        {
            // Find the related game object in the targets task items
            if (taskItem.targetObject == this.gameObject)
            {
                GetComponent<DialogueTrigger>().TriggerDialogue(taskItem.dialogueToTrigger);
                break;
            }

            // If not found
            Debug.LogWarning("Completed task item target object not found, triggering default index");
        }
    }

    private int CheckDialogueIndexToTrigger()
    {
        TaskObject thisTaskObject = GetComponent<TaskObject>();
        List<TaskItem> taskObjectsList = TasksManager.instance.taskItemsList;
        
        foreach (TaskItem taskItem in taskObjectsList)
        {
            if (taskItem.targetObject == GetComponent<DialogueTrigger>())
                return 1;
        }

        return 0;
    }
}
