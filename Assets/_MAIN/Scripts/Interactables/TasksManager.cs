using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskObject
{
    public GameObject taskObject;
    public bool hasBeenCompleted;
    public string taskObjectName;
}

public class TasksManager : MonoBehaviour, IDataPersistence
{
    public List<TaskObject> taskList;

    //[SerializeField] private Interactable currInteractable;
    //[SerializeField] private int currTaskIndex;

    public static TasksManager instance;

    private void Awake()
    {
        instance = this;

        foreach (TaskObject task in taskList)
        {
            task.taskObjectName = task.taskObject.name;
        }
    }

    public void UpdateTaskList()
    {
        foreach (TaskObject task in taskList)
        {
            Interactable obj = task.taskObject.GetComponent<Interactable>();

            task.hasBeenCompleted = obj.hasBeenCompleted;
        }
    }

    public void UpdateTaskObjects()
    {
        foreach (TaskObject task in taskList)
        {
            Interactable obj = task.taskObject.GetComponent<Interactable>();

            obj.hasBeenCompleted = task.hasBeenCompleted;
        }
    }

    // This code is so bad imma kms
    private void LoadTaskObjects(GameData data)
    {
        List<TaskObject> taskDataList = data.taskList;

        // For every TaskObjects in the scene tasklist
        foreach (TaskObject task in taskList)
        {
            // Inserts the GameObject
            task.taskObject = GameObject.Find(task.taskObjectName);

            // Then finds the variables in the data and assigns it accordingly
            foreach (TaskObject taskData in taskDataList)
            {
                if (taskData.taskObjectName == task.taskObjectName)
                {
                    task.hasBeenCompleted = taskData.hasBeenCompleted; 
                    break;
                }
            }
        }

        UpdateTaskObjects();
    }

    public List<TaskObject> GetTaskList()
    {
        return taskList;
    }

    public void SaveData(GameData data)
    {
        UpdateTaskList();

        data.taskList = this.taskList;
    }

    public void LoadData(GameData data) 
    {
        if (!FirstTimeScene.instance.isFirstTimeInScene)
        {
            this.taskList = data.taskList;
            LoadTaskObjects(data);
        }
    }
}
