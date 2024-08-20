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
    [Tooltip("GoToScene only works for entering next chapter. Will end this dialogue")]
    public LineAction action;
    public Sprite illustration;
    public string sceneDestinationName;
    public string interSceneText;
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
    [Tooltip("Used for idle or single dialogues. Disregards taskIndex and forPostTask variable if this is checked.")]
    public bool isNeutral;
    [Tooltip("Which task (in index) will trigger this dialogue. Must be unique.")] 
    public int taskIndex;
    [Tooltip("Whether or not this dialogue triggers after taskIndex task is completed")]
    public bool forPostTask;

    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

[RequireComponent(typeof(Interactable))]
public class DialogueTrigger : MonoBehaviour
{
    [Tooltip("First element is recommended to be neutral as it's fallback when no dialogue conditions " +
        "are fulfilled")]
    public List<Dialogue> dialogues;

    public void TriggerDialogue(int startLine = 0, int dialogueIndex = 0, bool resumingLastDialogue = false)
    {
        #region hellzone, everything's fixed, don't touch anything here anymore

        Dialogue dialogueToTrigger = null;
        bool hasTriggered = false;

        if (!resumingLastDialogue)
        {
            // Triggers if this is the dialogue of the current task
                // Checks if current task object is this object
            if (TasksManager.instance.GetCurrentTask() != null) // Exception
                if (TasksManager.instance.currTaskItem.taskObject == GetComponent<TaskObject>())
                    // Finds and triggers task dialogue if this is the current task
                    foreach (Dialogue dialogue in dialogues)
                    {
                        DialogueManager.instance.StartDialogue(GetTaskDialogue(), gameObject,  
                            dialogues.IndexOf(GetTaskDialogue()));
                        return;
                    }

            // Triggers if not a task dialogue, but there's a post task dialogue
                // Finds and sets dialogueIndex that contains dialogue for post task shits
            int thisLastTaskIndex = (TasksManager.instance.GetLatestTaskFor(GetComponent<TaskObject>()));
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

                // Triggers if a post task dialogue is found
                if (dialogueToTrigger != null)
                {
                    DialogueManager.instance.StartDialogue(dialogueToTrigger, gameObject, 
                        dialogues.IndexOf(dialogueToTrigger), startLine);
                    return;
                }
            }

            // If still no condition met, search and trigger neutral dialogue (if there are any)
            {
                int i = 0;
                foreach (Dialogue dialogue in dialogues)
                    if (dialogue.isNeutral)
                        if (!hasTriggered)
                        {
                            DialogueManager.instance.StartDialogue(dialogue, gameObject, i);
                            hasTriggered = true;
                        }
                        else // Exception
                            Debug.LogError("Multiple neutral dialogue found. Ignoring the rest with higher index");
                        i++;
                if (hasTriggered)
                    return;
            }
        }

        // If all else fails, which means no task dialogue, no post task dialogue,
        // and no neutral dialogue is found, force start a dialogue of index 0
        if (!hasTriggered && !resumingLastDialogue)
        {
            Debug.LogWarning("No dialogue fulfills any conditions. Triggering dialogue with default index of 0");
        }
        
        // Default trigger (also for resuming last save)
        DialogueManager.instance.StartDialogue(dialogues[dialogueIndex], gameObject, dialogueIndex, startLine);
        if (resumingLastDialogue) resumingLastDialogue = false;

        #endregion
    }

    private Dialogue GetTaskDialogue()
    {
        foreach (Dialogue dialogue in dialogues)
        {
            if (dialogue.taskIndex == TasksManager.instance.currTaskIndex && !dialogue.forPostTask
                && !dialogue.isNeutral)
                return dialogue;
        }

        // Exception
        Debug.LogError("No matching task index in dialogues. Returning index default 0");
        return dialogues[0];
    }
}
