using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontalInput;
    [SerializeField] private float moveSpeed = 1f;
    private DialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = GameObject.Find("Game Manager").GetComponent<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (!dialogueManager.isInDialogue)
        {
            Move(horizontalInput);            
        }

        if (dialogueManager.isInDialogue && Input.GetKeyDown(KeyCode.Space))
        {
            dialogueManager.DisplayNextDialogueLine();
        }
    }

    private void Move(float input)
    {
        transform.Translate(new Vector3(input, 0f, 0f) * moveSpeed * Time.deltaTime);
        FixPlayerPosition();
    }

    private void FixPlayerPosition()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "NPC")
        {
            collision.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }
}
