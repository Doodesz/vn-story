using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(Interactable),typeof(DialogueTrigger))]
public class TaskObject : MonoBehaviour, IDataPersistence
{
    public bool isCompleted;
    [Tooltip("Must match the related task item")] public string taskId;

    [HideInInspector] public string taskGameObjectName;

    // For serializable dictionary
    [SerializeField] string id;
    [ContextMenu("Generate GUID for ID")]
    private void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Awake()
    {
        taskGameObjectName = gameObject.name;

        if (id == string.Empty) // Doesn't work if it uses null instead
            GenerateGUID();
    }

    public void LoadData(GameData data)
    {
        data.taskObjectsDictionary.TryGetValue(id, out isCompleted);

        // should throw an exception when key not found and is not the first time in scene
    }

    public void SaveData(GameData data)
    {
        if (data.taskObjectsDictionary.ContainsKey(id))
        {
            data.taskObjectsDictionary.Remove(id);
        }
        data.taskObjectsDictionary.Add(id, isCompleted);
    }
}

