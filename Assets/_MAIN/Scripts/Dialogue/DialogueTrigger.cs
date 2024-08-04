using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]

public class DialogueData
{
    public string name;
    public Sprite portrait;
    // public Sprite rightIcon;
}

public enum PostLineAction { None, ChangeIllustration, GoToScene}
[System.Serializable]
public class DialogueLine
{
    public Animation portraitAnim;
    public DialogueData data;
    [TextArea(3, 10)]
    public string line;
    public PostLineAction action;

    // Enter if there's a post-line action
    public Sprite illustration;
    public string sceneDestinationName;
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
