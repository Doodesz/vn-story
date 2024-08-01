using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private float horizontalInput;
    [SerializeField] private float moveSpeed = 1f;
    public bool playerInControl = true;
    private DialogueManager dialogueManager;
    public static PlayerController Instance;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = GameObject.Find("Game Manager").GetComponent<DialogueManager>();
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (playerInControl)
        {
            Move(horizontalInput);            
        }
    }

    private void Move(float input)
    {
        transform.Translate(new Vector3(input, 0f, 0f) * moveSpeed * Time.deltaTime);
        LimitPlayerPosition();
    }

    private void LimitPlayerPosition()
    {
        MapManager mapManager = GameObject.Find("Map Manager").GetComponent<MapManager>();
        float leftBoundary = mapManager.leftBoundary;
        float rightBoundary = mapManager.rightBoundary;
        float playerPosX = transform.position.x;

        if (playerPosX > mapManager.rightBoundary)
        {
            transform.position = new Vector2(rightBoundary, transform.position.y);
        }
        else if (playerPosX < mapManager.leftBoundary)
        {
            transform.position = new Vector2(leftBoundary, transform.position.y);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "NPC" && playerInControl)
        {
            if (!collision.GetComponent<DialogueTrigger>().triggerOnTriggerenter)
                if (Input.GetKeyDown(KeyCode.Space))
                    collision.GetComponent<DialogueTrigger>().TriggerDialogue();
            else if (collision.GetComponent<DialogueTrigger>().triggerOnTriggerenter)
                    collision.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }

    public void SaveData(GameData data)
    {
        data.playerPosition = this.gameObject.transform.position;
        data.playerInControl = this.playerInControl;
    }

    public void LoadData(GameData data) 
    {
        this.gameObject.transform.position = data.playerPosition;
        this.playerInControl = data.playerInControl;
    }
}