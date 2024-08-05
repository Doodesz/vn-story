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
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueTextArea;
    public GameObject dialogueScreen;
    public GameObject logPanel; // Review dialogue panel
    public GameObject illustrationObject;
    public GameObject portraitObject;
    public Animator dialogueScreenAnimator;
    public GameObject dialogueItemPrefab;
    public GameObject dialogueLogContainer;
    public PlayerController playerController;
    //public Image characterIconRight; // unused

    [SerializeField] private Image portraitImage;
    [SerializeField] private Image illustrationImageObject;
    [SerializeField] private Image illustrationTransitionObject;
    [SerializeField] private Animator portraitAnimator;
    [SerializeField] private Animator illustrationAnimator;

    [Header("Current State")]
    public bool isInDialogue = false;
    public bool isTyping = false;
    [SerializeField] private int currentLineIndex = 0;
    [SerializeField] private GameObject npcBeingInteracted;

    [Header("Mod")]
    [SerializeField] private float typeSpdMultiplierPref = 1f;
    
    private Queue<DialogueLine> lines;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();

        dialogueScreen.SetActive(false);

        portraitAnimator = portraitObject.GetComponent<Animator>();
        portraitImage = portraitObject.GetComponent<Image>();
        illustrationAnimator = illustrationObject.GetComponent<Animator>();
        illustrationImageObject = illustrationObject.transform.GetChild(0).GetComponent<Image>();
        illustrationTransitionObject = illustrationObject.transform.GetChild(1).GetComponent<Image>();
    }

    private void Update()
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

        // Sets default variables and fixes unexpected bug
        isInDialogue = true;
        dialogueScreen.SetActive(true);
        npcBeingInteracted = gameObject;
        playerController.playerInControl = false;
        illustrationImageObject.sprite = null;
        illustrationTransitionObject.sprite = null;

        dialogueScreenAnimator.Play("show");

        lines.Clear();

        // Enqueues all lines from target TriggerDialogue
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        // Skips to specific line if resuming from saved date
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

        // Go to next line
        DialogueLine currentLine = lines.Dequeue();
        currentLineIndex++;

        portraitImage.sprite = currentLine.data.portrait;
        characterName.text = currentLine.data.name;
        //characterIconRight.sprite = currentLine.data.rightIcon; // unused

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine, currentLine.data.typingInterval * typeSpdMultiplierPref));
        AddNewConversationLog(currentLine.data.name, currentLine.line);

        if (currentLine.data.action != PostLineAction.None)
        {
            TriggerDialogueAction(currentLine);
        }
    }

    private void TriggerDialogueAction(DialogueLine dialogueLine)
    {
        if (dialogueLine.data.action == PostLineAction.ChangeIllustration)
        {
            ChangeIllustration(dialogueLine.data.illustration);
        }
        else if (dialogueLine.data.action == PostLineAction.GoToScene)
        {
            // LoadScene...
        }
    }

    private void ChangeIllustration(Sprite spriteIllustration)
    {
        // Show illust transition
        if (illustrationImageObject.sprite == null)
        {
            illustrationImageObject.sprite = spriteIllustration;
            illustrationImageObject.preserveAspect = true;
            
            illustrationAnimator.Play("show");
        }

        // Hide illust transition
        else if (spriteIllustration == null)
        {
            illustrationAnimator.Play("hide");

            StartCoroutine(ResetIllustrationImage());
        }

        // Switch illust transition
        else
        {
            StartCoroutine(SwitchIllustrationImage(spriteIllustration));
        }

    }

    IEnumerator TypeSentence(DialogueLine dialogueLine, float typingDelay)
    {
        isTyping = true;
        dialogueTextArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueTextArea.text += letter;
            yield return new WaitForSecondsRealtime(typingDelay);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        isInDialogue = false;
        currentLineIndex = 0;
        npcBeingInteracted = null;
        playerController.playerInControl = true;

        ClearConversationLog();
        HideDialogueScreen();

        DataPersistenceManager.Instance.SaveGame(); // Fix bug + autosave coy awowakw
    }

    private void HideDialogueScreen()
    {
        illustrationAnimator.Play("hide");
        dialogueScreenAnimator.Play("hide");

        StartCoroutine(DeactivateObject(dialogueScreen));
    }

    IEnumerator ResetIllustrationImage()
    {
        yield return new WaitForSeconds(2f);
        illustrationImageObject.sprite = null;
    }

    IEnumerator SwitchIllustrationImage(Sprite illustrationTotransitionTo)
    {
        illustrationTransitionObject.sprite = illustrationTotransitionTo;
        illustrationAnimator.Play("switch");
        yield return new WaitForSeconds(1f);
        illustrationImageObject.sprite = illustrationTotransitionTo;
    }

    IEnumerator DeactivateObject(GameObject gameObject)
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
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
