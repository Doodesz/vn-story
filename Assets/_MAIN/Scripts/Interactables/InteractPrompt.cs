using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        transform.GetComponentInParent<Interactable>().TriggerInteraction();
        PlayerController.instance.CancelMovement();
    }
}
