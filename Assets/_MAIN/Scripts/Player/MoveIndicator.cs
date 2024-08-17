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
        GetComponent<Animator>().SetBool("isClose", false);

        transform.position = new Vector3(position.x, transform.position.y, transform.position.z);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Vector3.Distance(PlayerController.instance.gameObject.transform.position, transform.position) <= 3f)
        {
            GetComponent<Animator>().SetBool("isClose", true);
        }
    }
}
