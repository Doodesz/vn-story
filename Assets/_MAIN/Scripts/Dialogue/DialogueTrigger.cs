using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string name;
    public Sprite leftIcon;
    public Sprite rightIcon;
}

[System.Serializable]
public class DialogueLine
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

public class DialogueTrigger : MonoBehaviour
{
    public bool triggerOnTriggerenter;

    public Dialogue dialogue;

    public void TriggerDialogue(int startLine = 0)
    {
        DialogueManager.Instance.StartDialogue(dialogue, gameObject, startLine);
    }
}