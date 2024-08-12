using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [Header("Assign GameObjects")]
    public MoveIndicator moveIndicator;

    [Header("Parameters")]
    [SerializeField] private float moveSpeed = 1f;

    [Header("Current State")]
    public bool playerInControl = true;
    public bool isMoving = false;
    public bool isMovingLeft;
    public bool isMovingRight;
    [SerializeField] float horizontalInput;
    [SerializeField] Vector3 moveDestination = Vector3.zero;

    private Animator animator;

    public static PlayerController instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        animator = GetComponent<Animator>();

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

            // Movements
            if (playerInControl)
            {
                // Deprecated
                // When receiving keyboard input, override isMovingToCursor
                horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput != 0f)
                {
                    Move(horizontalInput);
                    isMoving = false;
                }
            
                // Move to clicked position
                if (Input.GetMouseButtonDown(0))
                {
                    // Overwrites current movement first
                    CancelMovement();

                    isMoving = true;
                    moveDestination = worldPosition;
                    moveIndicator.Show(worldPosition);
                }
            }
            if (isMoving)
            {
                MoveToCursor(moveDestination);
            }

            LimitPlayerPosition();
        }
    }

    // Deprecated
    private void Move(float input)
    {
        transform.Translate(new Vector3(input, 0f, 0f) * moveSpeed * Time.deltaTime);
    }

    private void MoveToCursor(Vector3 destination)
    {
        animator.SetBool("isMoving", true);

        // If clicked postion on the left side of the player, move to the left
        if (destination.x < gameObject.transform.localPosition.x)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            isMovingLeft = true;
            transform.localScale = new Vector3(1, 1, 1);   

            // If exceeding the targeted position, cancels movement, this fixes bug
            if (destination.x > gameObject.transform.localPosition.x)
                CancelMovement();
        }

        // If opposite
        else if (destination.x > gameObject.transform.localPosition.x)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            isMovingRight = true;
            transform.localScale = new Vector3(-1, 1, 1);

            // If exceeding the targeted position, cancels movement
            if (destination.x < gameObject.transform.localPosition.x)
                CancelMovement();
        }
    }

    private void CancelMovement()
    {
        isMoving = false;
        isMovingRight = false;
        isMovingLeft = false;
        animator.SetBool("isMoving", false);
        moveIndicator.Hide();
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
            CancelMovement();
        }
        else if (playerPosX < mapManager.leftBoundary)
        {
            transform.position = new Vector2(leftBoundary, transform.position.y);
            CancelMovement();
        }
    }

    private void RepositionPlayerToDoor(string doorDestination)
    {
        Vector3 doorPos = GameObject.Find(doorDestination).transform.position;

        transform.position = new Vector2(doorPos.x, transform.position.y);
        GameManager.Instance.playerChangingMap = false;
        GameManager.Instance.UpdateDoorDestinationPos(doorPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Interactable currentInteractable = other.GetComponent<Interactable>();

        if (other.gameObject.CompareTag("Interactable") && playerInControl)
        {
            if (currentInteractable.triggerOnTriggerEnter)
            {
                currentInteractable.TriggerInteraction();
                isMoving = false;
                //camVocalPoint.SwitchToDialogueCam();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                currentInteractable.TriggerInteraction();
                isMoving = false;
                //camVocalPoint.SwitchToDialogueCam();
            }
        
            other.GetComponent<Interactable>().ShowInteractPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
            other.GetComponent<Interactable>().HideInteractPrompt();
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