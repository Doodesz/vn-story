using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (PlayerController.instance.PlayerInControl() && !GameManager.instance.isGamePaused)
        {
            transform.GetComponentInParent<Interactable>().TriggerInteraction();
            PlayerController.instance.CancelMovement();
        }
    }
}
