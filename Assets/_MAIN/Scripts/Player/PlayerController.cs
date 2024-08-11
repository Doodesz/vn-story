using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [Header("Assign GameObjects")]
    public GameObject camVocalPoint;

    [Header("Parameters")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float camLookAhead;

    [Header("Camera Parameters")]
    [SerializeField] private float idleCamLerpValue;
    [SerializeField] private float movingCamLerpValue;
    [SerializeField] private float movingCamTimer;
    [SerializeField] private float movingCamTimeout = 5f;

    [Header("Current State")]
    public bool playerInControl = true;
    [SerializeField] float horizontalInput;
    [SerializeField] bool isMovingToCursor = false;
    [SerializeField] Vector3 moveDestination = Vector3.zero;

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

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Vector3 mousePos = Input.mousePosition + new Vector3(0,0, 10f);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 camVocalPointPos = camVocalPoint.transform.localPosition;

            movingCamTimer -= Time.deltaTime;

            if (playerInControl)
            {
                // When receiving keyboard input, override isMovingToCursor
                horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput != 0f)
                {
                    Move(horizontalInput);
                    isMovingToCursor = false;
                }
            
                // Move to clicked position
                if (Input.GetMouseButtonDown(0))
                {
                    isMovingToCursor = true;
                    moveDestination = worldPosition;
                }
            }

            if (isMovingToCursor)
            {
                MoveToCursor(moveDestination);
            }

            // Resets cam position slowly when idle
            if (!isMovingToCursor && playerInControl)
            {
                camVocalPoint.transform.localPosition = 
                    Vector3.Lerp(camVocalPointPos, Vector3.zero, idleCamLerpValue);

                if (movingCamTimer <= 0)
                    camVocalPoint.transform.localPosition = Vector3.zero;
            }

            if (!playerInControl && DialogueManager.Instance.isInDialogue)
            {
                camVocalPoint.transform.localPosition = new Vector3(0, -2.5f, 0);
            }

            LimitPlayerPosition();
        }
    }

    private void Move(float input)
    {
        movingCamTimer = movingCamTimeout;

        transform.Translate(new Vector3(input, 0f, 0f) * moveSpeed * Time.deltaTime);
    }

    private void MoveToCursor(Vector3 destination)
    {
        Vector3 camVocalPointPos = camVocalPoint.transform.localPosition;
        Vector3 camDestination = new Vector3(camLookAhead, 0, 0);
        movingCamTimer = movingCamTimeout;

        // If clicked postion on the left side of the player, move to the left
        if (destination.x < gameObject.transform.localPosition.x)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            camVocalPoint.transform.localPosition = 
                Vector3.Lerp(camVocalPointPos, -camDestination, movingCamLerpValue);

            // If exceeding the targeted position, cancels movement, this fixes bug
            if (destination.x > gameObject.transform.localPosition.x)
                isMovingToCursor = false;
        }

        // If opposite
        else if (destination.x > gameObject.transform.localPosition.x)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            camVocalPoint.transform.localPosition = 
                Vector3.Lerp(camVocalPointPos, camDestination, movingCamLerpValue);
            
            // If exceeding the targeted position, cancels movement
            if (destination.x < gameObject.transform.localPosition.x)
                isMovingToCursor = false;
        }
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
            isMovingToCursor = false;
        }
        else if (playerPosX < mapManager.leftBoundary)
        {
            transform.position = new Vector2(leftBoundary, transform.position.y);
            isMovingToCursor = false;
        }
    }

    private void RepositionPlayerToDoor(string doorDestination)
    {
        Vector3 doorPos = GameObject.Find(doorDestination).transform.position;

        transform.position = new Vector2(doorPos.x, transform.position.y);
        GameManager.Instance.playerChangingMap = false;
        GameManager.Instance.UpdateDoorDestinationPos(doorPos);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Interactable currentInteractable = collision.GetComponent<Interactable>();

        if (collision.gameObject.CompareTag("Interactable") && playerInControl)
        {
            if (currentInteractable.triggerOnTriggerEnter)
            {
                currentInteractable.TriggerInteraction();
                isMovingToCursor = false;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                currentInteractable.TriggerInteraction();
                isMovingToCursor = false;
            }
        }
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