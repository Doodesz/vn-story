using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.ComponentModel;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Assign GameObjects")]
    public Image characterIconLeft;
    public Image characterIconRight;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public GameObject dialogueScreen;
    public GameObject logPanel;
    public Animator animator;
    public GameObject dialogueItemPrefab;
    public GameObject dialogueLogContainer;

    [Header("Current State")]
    public bool isInDialogue = false;
    public bool isTyping = false;

    [Header("Mod")]
    public float typingSpeed = 0.2f;
    
    private Queue<DialogueLine> lines;
    private DialogueLine[] dialogueLineArr;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
    }

    private void LateUpdate()
    {
        if (isInDialogue && !isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextDialogueLine();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isInDialogue = true;
        dialogueScreen.SetActive(true);

        animator.Play("show");

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Peek();

        characterIconLeft.sprite = currentLine.data.leftIcon;
        characterIconRight.sprite = currentLine.data.rightIcon;
        characterName.text = currentLine.data.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
        AddNewConversationLog(currentLine.data.name, currentLine.line);

        lines.Dequeue();
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForFixedUpdate();
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        ClearConversationLog();
        isInDialogue = false;
        animator.Play("hide");
        DeactivateDialogueScreen();
    }

    IEnumerator DeactivateDialogueScreen()
    {
        yield return new WaitForSeconds(1f);
        dialogueScreen.SetActive(false);
    }

    public void ToggleActivateLogPanel()
    {
        logPanel.SetActive(!logPanel.activeInHierarchy);
    }

    public void AddNewConversationLog(string name, string text)
    {
        GameObject logItem = Instantiate(dialogueItemPrefab, dialogueLogContainer.transform);
        logItem.GetComponent<TextMeshProUGUI>().text = name;
        logItem.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text;
    }

    private void ClearConversationLog()
    {
        for (int i = 0; i < dialogueLogContainer.transform.childCount; i++)
        {
            Destroy(dialogueLogContainer.transform.GetChild(i).gameObject);
        }
    }
}
