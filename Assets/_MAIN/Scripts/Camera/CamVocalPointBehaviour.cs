using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamVocalPointBehaviour : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private bool followsPlayer;
    [SerializeField] public bool isMoving;
    [SerializeField] private bool isCamRepositioned;
    [SerializeField] private float camLookAheadValue;
    [SerializeField] private float movingCamLerpValue;
    [SerializeField] private float idleCamLerpValue;
    [SerializeField] private float movingCamTimeout;
    
    private PlayerController playerController;

    public static CamVocalPointBehaviour instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (followsPlayer)
        {
            if (playerController.isMoving)
            {
                StopAllCoroutines();

                Vector3 playerPos = player.transform.position;
                Vector3 camLookAheadResult = new Vector3(camLookAheadValue, 0, 0);

                if (playerController.isMovingLeft)
                {
                    transform.position = Vector3.Lerp
                        (transform.position, playerPos - camLookAheadResult, movingCamLerpValue);
                }
                else if (playerController.isMovingRight)
                {
                    transform.position = Vector3.Lerp
                        (transform.position, playerPos + camLookAheadResult, movingCamLerpValue);
                }

                isCamRepositioned = false;
            }

            else
                transform.position = Vector3.Lerp(transform.position, player.transform.position, idleCamLerpValue);

            // Kalau perlu
            /*if (DialogueManager.Instance.isInDialogue)
            {

            }*/
            
            if (!isCamRepositioned)
                StartCoroutine(ResetCamPos());
        }

    }

    private IEnumerator ResetCamPos()
    {
        yield return new WaitForSeconds(movingCamTimeout);
        transform.position = player.transform.position;
        isCamRepositioned = true;
    }
}
