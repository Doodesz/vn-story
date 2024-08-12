using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIndicator : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(Vector3 position)
    {
        gameObject.SetActive(true);

        transform.position = new Vector3(position.x, transform.position.y, transform.position.z);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
