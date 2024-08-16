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
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

[RequireComponent(typeof(Interactable))]
public class DialogueTrigger : MonoBehaviour
{
    [Tooltip("Uncheck this to ignore dialogue index out of range warning message")]
    [SerializeField] bool hasMultipleDialogues = false;

    [Tooltip("0: Default, 1: Triggers when this is the current task, 2: Triggers when this is a completed task")]
    public List<Dialogue> dialogue;


    public void TriggerDialogue(int startLine = 0, int dialogueIndex = 0)
    {
        // When the dialogues count is below the called index or doesn't have multiple dialogues
        if (dialogue.Count < dialogueIndex-1 || !hasMultipleDialogues)
        {
            // Sets index to default (0)
            dialogueIndex = default;

            // Logs a warning if there are multiple dialogues but dialogue count is below called index (out of range)
            if(hasMultipleDialogues)
                Debug.LogWarning("Dialogue index out of range, assigning default value");
        }
        
        DialogueManager.Instance.StartDialogue(dialogue[dialogueIndex], gameObject, startLine);
    }
}
