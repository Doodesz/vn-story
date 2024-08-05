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
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

[RequireComponent(typeof(Interactable))]
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue(int startLine = 0)
    {
        DialogueManager.Instance.StartDialogue(dialogue, gameObject, startLine);
    }
}
