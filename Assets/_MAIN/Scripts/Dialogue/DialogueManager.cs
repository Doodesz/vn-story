using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.ComponentModel;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour, IDataPersistence
{
    public static DialogueManager instance;

    [Header("Assign GameObjects")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueTextArea;
    public GameObject dialogueObject;
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
    public bool hasPendingTaskListUpdate = false;
    [SerializeField] private int currentLineIndex = 0;
    [SerializeField] private GameObject npcBeingInteracted;
    [SerializeField] private bool isIllustHidden = true;
    [SerializeField] private bool isSwitchingIllust;
    
    float switchingIllustTimeoutTimer;

    [Header("Mod")]
    [SerializeField] private float typeSpdMultiplierPref = 1f;
    
    private Queue<DialogueLine> lines;

    // Bug fix: Resets all variables first
    private void Awake()
    {
        if (instance == null)
            instance = this;

        lines = new Queue<DialogueLine>();

        dialogueObject.SetActive(false);

        portraitAnimator = portraitObject.GetComponent<Animator>();
        portraitImage = portraitObject.GetComponent<Image>();
        illustrationAnimator = illustrationObject.GetComponent<Animator>();
        illustrationImageObject = illustrationObject.transform.GetChild(0).GetComponent<Image>();
        illustrationTransitionObject = illustrationObject.transform.GetChild(1).GetComponent<Image>();

        illustrationImageObject.sprite = null;
        illustrationTransitionObject.sprite = null;
    }

    private void Update()
    {

        if (isInDialogue && !isTyping && ( Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) )
            && !UIManager.instance.isMouseOverButton && !UIManager.instance.isOnCoveredScreenUI)
        {
            DisplayNextDialogueLine();
        }

        // Bug fix: Forces to stop switchillust coroutine when loading scene mid-illust transition
        switchingIllustTimeoutTimer -= Time.deltaTime;
        if (switchingIllustTimeoutTimer < 0 && isSwitchingIllust)
        {
            StopCoroutine(SwitchIllustrationImage(null));
            ForceStopSwitchIllustCoroutine();
        }
    }

    public void StartDialogue(Dialogue dialogue, GameObject gameObject, int startLine = 0)
    {
        // If last scene and loaded scene mismatch on last save during a dialogue
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController.Instance not found. May be a mismatch of scene from last dialogue");
            return;
        }

        // Sets default variables and fixes unexpected bug
        isInDialogue = true;
        dialogueObject.SetActive(true);
        npcBeingInteracted = gameObject;
        playerController.isPlayerInControl = false;
        illustrationImageObject.sprite = null;
        illustrationTransitionObject.sprite = null;

        dialogueScreenAnimator.Play("show");

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        // Skips to specific line if resuming from saved data
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

        StopAllCoroutines();

        // Go to next line
        DialogueLine currentLine = lines.Dequeue();
        currentLineIndex++;

        portraitImage.sprite = currentLine.data.portrait;
        characterName.text = currentLine.data.name;
        //characterIconRight.sprite = currentLine.data.rightIcon; // unused

        if (currentLine.data.action != LineAction.None)
        {
            TriggerDialogueAction(currentLine);
        }

        StartCoroutine(TypeSentence(currentLine, currentLine.data.typingInterval * typeSpdMultiplierPref));
        AddNewConversationLog(currentLine.data.name, currentLine.line);
    }

    private void TriggerDialogueAction(DialogueLine dialogueLine)
    {
        if (dialogueLine.data.action == LineAction.ChangeIllustration)
        {
            ChangeIllustration(dialogueLine.data.illustration);
        }

        else if (dialogueLine.data.action == LineAction.GoToScene)
        {
            // LoadScene... used later
        }
    }

    private void ChangeIllustration(Sprite spriteIllustration) 
    {
        // Show illust transition
        if (illustrationImageObject.sprite == null)
        {
            isIllustHidden = false;
            illustrationImageObject.sprite = spriteIllustration;
            illustrationImageObject.preserveAspect = true;
            
            illustrationAnimator.Play("show");
        }

        // Hide illust transition
        else if (spriteIllustration == null)
        {
            isIllustHidden = true;
            illustrationAnimator.Play("hide");

            StartCoroutine(ResetIllustrationImage());
        }

        // Switch illust transition
        else
        {
            StopCoroutine(SwitchIllustrationImage(spriteIllustration));
            isSwitchingIllust = false;
            StartCoroutine(SwitchIllustrationImage(spriteIllustration));

            illustrationTransitionObject.preserveAspect = true;
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
        StartCoroutine(playerController.ResumePlayerControlAfterSeconds(0.5f));

        ClearConversationLog();
        HideDialogueScreen();

        if (hasPendingTaskListUpdate)
        {
            TasksManager.instance.UpdateTaskItemsList(true);
            hasPendingTaskListUpdate = false;
        }

        DataPersistenceManager.Instance.SaveGame(); // Fix bug + autosave coy awowakw
    }

    private void HideDialogueScreen()
    {
        if (!isIllustHidden)
            illustrationAnimator.Play("hide");

        dialogueScreenAnimator.Play("hide");
        StartCoroutine(DeactivateObject(dialogueObject));
    }

    IEnumerator ResetIllustrationImage()
    {
        yield return new WaitForSeconds(1f);
        illustrationImageObject.sprite = null;
        illustrationTransitionObject.sprite = null;
    }

    IEnumerator SwitchIllustrationImage(Sprite illustrationTotransitionTo)
    {
        illustrationTransitionObject.sprite = illustrationTotransitionTo;
        illustrationAnimator.Play("switch");
        isSwitchingIllust = true;
        switchingIllustTimeoutTimer = 1.8f;
        yield return new WaitForSecondsRealtime(1.5f);
        illustrationImageObject.sprite = illustrationTransitionObject.sprite;
        isSwitchingIllust = false;
    }

    private void ForceStopSwitchIllustCoroutine()
    {
        illustrationImageObject.sprite = illustrationTransitionObject.sprite;
        isSwitchingIllust = false;
    }

    IEnumerator DeactivateObject(GameObject gameObject)
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    public void ToggleActivateLogPanel()
    {
        logPanel.SetActive(!logPanel.activeInHierarchy);

        if (logPanel.activeInHierarchy)
            UIManager.instance.isOnCoveredScreenUI = true;
        else
            UIManager.instance.isOnCoveredScreenUI = false;
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
