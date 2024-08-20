using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void TogglePauseMenu()
    {
        StopAllCoroutines();

        if (!gameObject.activeSelf)
            ShowPauseMenu();
        else 
            HidePauseMenu();
    }

    private void ShowPauseMenu()
    {
        GameManager.instance.isGamePaused = true;
        gameObject.SetActive(true);
        animator.Play("show");
    }

    private void HidePauseMenu()
    {
        GameManager.instance.isGamePaused = false;
        animator.Play("hide");
        StartCoroutine(ToggleSetActiveSelf());
    }

    IEnumerator ToggleSetActiveSelf()
    {
        yield return new WaitForSecondsRealtime(0.7f);
        gameObject.SetActive(!gameObject.activeSelf);
    }

}
