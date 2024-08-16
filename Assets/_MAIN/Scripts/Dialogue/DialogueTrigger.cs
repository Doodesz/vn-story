using System.Collections.Generic;
using UnityEngine;

// public enum PortraitAnim { None, BounceUpOnce, BounceUpTwice, BounceDownOnce, BounceDownTwice }
public enum LineAction { None, ChangeIllustration, GoToScene}

[System.Serializable]
public class DialogueData // Contains anything abt the line data
{
    // public Sprite rightIcon; // unused
    public string name;
    public Sprite portrait;
    public Animation portraitAnim;
    public float typingInterval = 1;

    // Enter if there's a post-line action
    [Header("Line Action")]
    public LineAction action;
    public Sprite illustration;
    public string sceneDestinationName;
}


[System.Serializable]
public class DialogueLine // Contains only for the text data
{
    public DialogueData data;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    [Tooltip("Which task (in index) will trigger this dialogue. Must be unique.")] 
        public int taskIndex;
    [Tooltip("Whether or not this dialogue triggers after taskIndex task is completed")]
        public bool forPostTask;

    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

[RequireComponent(typeof(Interactable))]
public class DialogueTrigger : MonoBehaviour
{
    public List<Dialogue> dialogues;

    public void TriggerDialogue(int startLine = 0, int dialogueIndex = 0)
    {
        bool isCurrentTask = false;
        int thisLastTaskIndex = (TasksManager.instance.GetLatestTaskFor(GetComponent<TaskObject>()));
        
        // Checks if current task object is this object
        if (TasksManager.instance.GetCurrentTask() != null) // Exception
            if (TasksManager.instance.currTaskItem.taskObject == GetComponent<TaskObject>())
                isCurrentTask = true;

        // Finds and triggers task dialogue if this is the current task
        foreach (Dialogue dialogue in dialogues)
        {
            if (isCurrentTask)
            {
                DialogueManager.instance.StartDialogue(GetTaskDialogue(), gameObject, 0);
                return;
            }
        }

        // Finds and sets dialogueIndex that contains dialogue for post task shits
        if (thisLastTaskIndex < TasksManager.instance.currTaskIndex)
        {
            int i = 0;
            Debug.Log("line hits");

            foreach (Dialogue dialogue in dialogues)
            {
                if (dialogue.forPostTask == true)
                {
                    dialogueIndex = i;
                    Debug.Log("Post Task Dialogue found: " +  dialogueIndex);
                }
                i++;
            }
        }
        
        // Default trigger (also for resuming last save)
        DialogueManager.instance.StartDialogue(dialogues[dialogueIndex], gameObject, startLine);
    }

    private Dialogue GetTaskDialogue()
    {
        foreach (Dialogue dialogue in dialogues)
        {
            if (dialogue.taskIndex == TasksManager.instance.currTaskIndex)
                return dialogue;
        }

        Debug.Log("No matching task index in dialogues. Returning index default 0");
        return dialogues[0];
    }
}
