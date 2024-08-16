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
    
    [SerializeField] TaskObject taskObject;
    [SerializeField] TasksManager tasksManager;
    [SerializeField] bool hasTaskObjectAttached;

    private void Awake()
    {
        if (GetComponent<TaskObject>() != null)
        {
            hasTaskObjectAttached = true;
        }
    }

    private void Start()
    {
        taskObject = GetComponent<TaskObject>();
        tasksManager = TasksManager.instance;    
    }

    public void TriggerInteraction()
    {
        switch (thisInteractableType)
        {
            case InteractableType.Dialogue:
                if (tasksManager.GetCurrentTask() == null)
                {
                    Debug.LogError("Failed to fetch current task. Triggering default value dialogue");
                    GetComponent<DialogueTrigger>().TriggerDialogue();
                }

                else if (hasTaskObjectAttached && tasksManager.GetCurrentTask().taskObject != null)
                {
                    if (tasksManager.GetCurrentTask().taskObject == taskObject)
                    {
                        GetComponent<DialogueTrigger>().TriggerDialogue(dialogueIndex: 1);
                        break;
                    }
                    else if (taskObject.isCompleted)
                    {
                        GetComponent<DialogueTrigger>().TriggerDialogue(dialogueIndex: 2);
                        break;
                    }
                }

                else
                {
                    Debug.LogError("No conditions fulfilled interactable dialogue");
                    break;
                }

                GetComponent<DialogueTrigger>().TriggerDialogue();
                break;

            case InteractableType.SwitchScenes:
                SwitchScene();
                break;

            case InteractableType.ChangeArea:
                // switch scene code here
                Debug.LogWarning("InteractableType.ChangeArea has not yet been coded.");
                break;

            default:
                break;
        }

        // Order of execution is important, this is put down below because it needs to trigger the interaction first before updating object
        // Sets complete if this interactable is the current task
        if (hasTaskObjectAttached && tasksManager.currTaskItem != null
            && tasksManager.currTaskItem.taskObject == taskObject)
        {
            if (taskObject.isCompleted)
                Debug.LogWarning("Task object " + taskObject + " has already been completed");

            taskObject.isCompleted = true;

            if (thisInteractableType == InteractableType.Dialogue)
                DialogueManager.instance.hasPendingTaskListUpdate = true;
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
}
