using System.Collections.Generic;
using Unity.VisualScripting;
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
    [Tooltip("First element must be neutral as it's for a fallback when no dialogue conditions are fulfilled")]
    public List<Dialogue> dialogues;

    public void TriggerDialogue(int startLine = 0, int dialogueIndex = 0, bool resumingLastDialogue = false)
    {
        Dialogue dialogueToTrigger = null;

        if (!resumingLastDialogue)
        {
            bool isCurrentTask = false;

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


            int thisLastTaskIndex = (TasksManager.instance.GetLatestTaskFor(GetComponent<TaskObject>()));

            // Finds and sets dialogueIndex that contains dialogue for post task shits
            if (thisLastTaskIndex < TasksManager.instance.currTaskIndex)
            {
                // Finds every dialogue that has passed the task index and has a true postTask value
                foreach (Dialogue dialogue in dialogues)
                {
                    if (dialogue.taskIndex <= TasksManager.instance.currTaskIndex && dialogue.forPostTask == true)
                    {
                        if (dialogueToTrigger == null)
                            dialogueToTrigger = dialogue;

                        // Changes dialogueToTrigger to the newer index of task dialogue
                        else if (dialogue.taskIndex > dialogueToTrigger.taskIndex)
                            dialogueToTrigger = dialogue;

                        Debug.Log("Post Task Dialogue index found: " +  dialogues.IndexOf(dialogueToTrigger));
                    }
                }
                DialogueManager.instance.StartDialogue(dialogueToTrigger, gameObject, startLine);
                return;
            }
        }
        
        // Default trigger (also for resuming last save)
        DialogueManager.instance.StartDialogue(dialogues[dialogueIndex], gameObject, startLine);

        if (resumingLastDialogue) resumingLastDialogue = false;
    }

    private Dialogue GetTaskDialogue()
    {
        foreach (Dialogue dialogue in dialogues)
        {
            if (dialogue.taskIndex == TasksManager.instance.currTaskIndex && !dialogue.forPostTask)
                return dialogue;
        }

        Debug.LogWarning("No matching task index in dialogues. Returning index default 0");
        return dialogues[0];
    }
}
