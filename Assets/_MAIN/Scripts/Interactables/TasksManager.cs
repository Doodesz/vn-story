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

    [Header("Task Items")]
    public List<TaskItem> taskItemsList;
    public TaskItem currTaskItem;

    public static TasksManager instance;

    private void Awake()
    {
        instance = this;

        foreach (TaskItem simpleTaskItem in taskItemsList)
        {
            simpleTaskItem.taskObjectName = simpleTaskItem.taskObject.gameObject.name;
        }

        currTaskItem = GetCurrentTask();
    }

    private void Start()
    {
        UpdateTaskItemsList();
    }

    // Updates the taskitems list
    public void UpdateTaskItemsList()
    {
        foreach (TaskItem taskItem in taskItemsList)
        {
            taskItem.isCompleted = taskItem.taskObject.isCompleted;
        }

        currTaskItem = GetCurrentTask();

        taskText.text = currTaskItem.taskName;

        if (currTaskItem == null)
        {
            taskText.text = "teks error ini, task udh selesai semua";
            return;
        }
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
        if (taskItemsList.Count == count)
        {
            Debug.LogWarning("All tasks are completed");
            return null;
        }

        // Throws warning log when task is not found in scene
        Debug.LogWarning("Failed to get current task");
        return null;
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