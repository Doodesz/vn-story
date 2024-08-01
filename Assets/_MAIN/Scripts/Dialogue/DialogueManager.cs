using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.ComponentModel;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour, IDataPersistence
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
    public PlayerController playerController;

    [Header("Current State")]
    public bool isInDialogue = false;
    public bool isTyping = false;
    [SerializeField] private int currentLineIndex = 0;
    [SerializeField] private GameObject npcBeingInteracted;

    [Header("Mod")]
    public float typingSpeed = 0.2f;
    
    private Queue<DialogueLine> lines;

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

    public void StartDialogue(Dialogue dialogue, GameObject gameObject, int startLine = 0)
    {
        // If last scene and loaded scene mismatch on last save during a dialogue
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController.Instance not found. May be a mismatch of scene in last conversation");
            return;
        }

        isInDialogue = true;
        dialogueScreen.SetActive(true);
        npcBeingInteracted = gameObject;
        playerController.playerInControl = false;

        animator.Play("show");

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        for (int i=0; i<startLine; i++)
            DisplayNextDialogueLine();

        currentLineIndex = startLine;
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Peek();

        currentLineIndex++;

        characterIconLeft.sprite = currentLine.data.leftIcon;
        // characterIconRight.sprite = currentLine.data.rightIcon;
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
        currentLineIndex = 0;
        npcBeingInteracted = null;
        playerController.playerInControl = true;
        DataPersistenceManager.Instance.SaveGame(); // Fix bug + autosave
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

    public void SaveData(GameData data)
    {
        if (npcBeingInteracted == null)
        {
            data.lastDialogueLineIndex = 0;
            data.npcBeingInteracted = null;
            data.isInDialogue = false;
        }        
        else
        {
            data.npcBeingInteracted = this.npcBeingInteracted.name;
            data.lastDialogueLineIndex = this.currentLineIndex;
            data.isInDialogue = true;
        }
    }

    public void LoadData(GameData data)
    {
        this.currentLineIndex = data.lastDialogueLineIndex;
        this.isInDialogue = data.isInDialogue;
        this.npcBeingInteracted = GameObject.Find(data.npcBeingInteracted);

        if (isInDialogue) 
            LoadLastConversation();
    }

    public void LoadLastConversation()
    {
        npcBeingInteracted.GetComponent<DialogueTrigger>().TriggerDialogue(currentLineIndex);
        if (npcBeingInteracted == null) Debug.Log("Referenced npcBeingInteracted GameObject not found");
    }
}
