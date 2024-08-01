using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private float horizontalInput;
    [SerializeField] private float moveSpeed = 1f;
    public bool playerInControl = true;
    public static PlayerController Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        if (SceneManager.GetActiveScene().name == "MainMenu")
            playerInControl = false;

        if (GameManager.Instance.playerChangingMap && SceneManager.GetActiveScene().isLoaded)
        {
            RepositionPlayerToDoor(GameManager.Instance.doorDestination);
            DataPersistenceManager.Instance.SaveGame(); // Fix bug +fitur auto save anjay:D
        }
    }

    private void FixedUpdate()
    {
        if (playerInControl)
        {
            horizontalInput = Input.GetAxis("Horizontal");
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
        MapManager mapManager = MapManager.Instance;
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
        InteractableType interactableType = collision.GetComponent<Interactable>().thisInteractableType;
        Interactable interactable = collision.GetComponent<Interactable>();

        if (collision.gameObject.CompareTag("Interactable") && playerInControl)
        {
            // If trigger object is a dialogue type
            if (interactableType == InteractableType.Dialogue)
                if (interactable.triggerOnTriggerEnter)
                    collision.GetComponent<DialogueTrigger>().TriggerDialogue();
            
                else if (Input.GetKeyDown(KeyCode.Space))
                    collision.GetComponent<DialogueTrigger>().TriggerDialogue();

            // Move player to x area
            if (interactableType == InteractableType.SwitchScenes)
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StopAllCoroutines();
                    
                    interactable.SwitchScene();
                }
        }
    }

    private void RepositionPlayerToDoor(string doorDestination)
    {
        Vector3 doorPos = GameObject.Find(doorDestination).transform.position;

        transform.position = new Vector2(doorPos.x, transform.position.y);
        GameManager.Instance.playerChangingMap = false;
        GameManager.Instance.UpdateDoorDestinationPos(doorPos);
    }

    public void SaveData(GameData data)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            data.playerPosition = this.gameObject.transform.position;
            data.playerInControl = this.playerInControl;
        }
    }

    public void LoadData(GameData data) 
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (!GameManager.Instance.playerChangingMap)
                this.gameObject.transform.position = data.playerPosition;
            this.playerInControl = data.playerInControl;
        }
    }
}