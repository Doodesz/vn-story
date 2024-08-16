using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TaskItem
{
    public TaskObject taskObject;
    public string taskObjectName;
    public string taskName;
    public bool isCompleted;
}

public class TasksManager : MonoBehaviour, IDataPersistence
{
    [Header("Assign GameObjects")]
    public TextMeshProUGUI taskText;
    public GameObject taskIndicator;

    [Header("Task Items")]
    public int currTaskIndex = 0;
        [Tooltip("Index next to each other MUST NOT have the same TaskObject")]
    public List<TaskItem> taskItemsList;
    public TaskItem currTaskItem;

    public static TasksManager instance;

    private void Awake()
    {
        instance = this;

        foreach (TaskItem taskItem in taskItemsList)
        {
            taskItem.taskObjectName = taskItem.taskObject.gameObject.name;
        }

        currTaskItem = GetCurrentTask();
    }

    private void Start()
    {
        UpdateTaskItemsList();
    }

    // Updates the taskitems list
    public void UpdateTaskItemsList(bool incrementTaskIndex = false)
    {
        if (incrementTaskIndex) ++currTaskIndex;

        List<TaskObject> taskObjectsList = new (FindObjectsOfType<TaskObject>());

        int i = 0;
        foreach (TaskItem taskItem in taskItemsList)
        {
            if (i < currTaskIndex) taskItem.isCompleted = taskItem.taskObject.isCompleted;
            i++;
        }

        currTaskItem = GetCurrentTask();

        if (currTaskItem == null)
        {
            taskText.text = "Error: No task item found";
            return;
        }
        else 
            taskText.text = currTaskItem.taskName;
            taskIndicator.transform.position = currTaskItem.taskObject.gameObject.transform.GetChild(0).transform.position;
    }

    public TaskItem GetCurrentTask()
    {
        int count = 0;

        // Returns the first uncompleted task in list
        foreach (TaskItem task in taskItemsList)
        {
            count++;

            if (!task.isCompleted)
            {
                return task;
            }
        }

        // Throws warning log if there are no more uncompleted task
        if (taskItemsList.Count >= count)
        {
            Debug.LogWarning("All tasks are completed");
            return null;
        }

        // Throws warning log when task is not found in scene
        Debug.LogWarning("Failed to get current task");
        return null;
    }

    // Returns the latest completed task for an object
    public int GetLatestTaskFor(TaskObject taskObject)
    {
        int i = 0;

        foreach (TaskItem task in taskItemsList)
        {
            i++;
            if (task.taskObject == taskObject && task.isCompleted) break;
        }

        Debug.Log("Last task index was " + i);
        return i;
    }

    public bool IsTaskIndexCompleted(int taskIndex)
    {
        TaskItem taskItem = taskItemsList[taskIndex];
        return taskItem.isCompleted;
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

            foreach (TaskItem item in this.taskItemsList)
            {
                item.taskObject = GameObject.Find(item.taskObjectName).GetComponent<TaskObject>();
            }
        }
    }
}

// omg im kms wtdf is this code man 
// note to self: pls ffs never code like this again
// update 16/8/24 00:30 ahh fucking much better