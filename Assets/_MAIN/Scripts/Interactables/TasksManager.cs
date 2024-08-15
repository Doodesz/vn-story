using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TaskItem
{
    [Tooltip("Must match the related task object")] public string id;

    [Header("Task Requirements")]
    public TaskObject taskObject;
    public string taskObjectName;
    public string taskName;
    public bool isCompleted;

    [Header("Dialogue to Unlock")]
    public DialogueTrigger targetObject;
    public string targetObjectName;
    [Tooltip("Which dialogue (in index) does this object will trigger after completing this task?")]
        public int dialogueToTrigger;
}

public class TasksManager : MonoBehaviour, IDataPersistence
{
    public List<TaskItem> taskItemsList;

    public static TasksManager instance;

    private void Awake()
    {
        instance = this;

        // Assigns the target TriggerDialogues on start
        foreach (TaskItem taskItem in taskItemsList)
        {
            taskItem.taskObjectName = taskItem.taskObject.gameObject.name;

            taskItem.targetObject = taskItem.targetObject.GetComponent<DialogueTrigger>();
            taskItem.targetObjectName = taskItem.targetObject.gameObject.name;
        }
    }

    // Updates the taskitems list
    public void UpdateTaskItemsList()
    {
        // For every task items
        foreach (TaskItem taskItem in taskItemsList)
        {
            // Check if the game object on the scene is completed
            if (taskItem.taskObject.isCompleted)
            {   // Updates the taskitems list
                taskItem.isCompleted = true;
            }
        }
    }

    public void SaveData(GameData data)
    {
        UpdateTaskItemsList();

        data.taskItemsList = this.taskItemsList;
    }

    public void LoadData(GameData data)
    {
        if (!FirstTimeScene.instance.isFirstTimeInScene)
        {
            this.taskItemsList = data.taskItemsList;

            // For every taskitem in the list
            foreach (TaskItem item in this.taskItemsList)
            {
                item.taskObject = GameObject.Find(item.taskObjectName).GetComponent<TaskObject>();
                item.targetObject = GameObject.Find(item.targetObjectName).GetComponent<DialogueTrigger>();
            }
            
            UpdateTaskItemsList();
        }
    }
}

// omg im kms wtdf is this code man 
// note to self: pls ffs never code like this again
// update 16/8/24 00:30 ahh fucking much better