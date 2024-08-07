using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    //[SerializeField] private GameObject blackScreen;
    [SerializeField] private Animator animator;
    public static ScreenTransition instance;

    private void Awake()
    {
        instance = this;

        animator = GetComponent<Animator>();

        //blackScreen.SetActive(true);
        animator.Play("transition in");
    }

    public void PlayTransitionIn()
    {
        animator.Play("transition in");
    }

    public void PlayTransitionOut()
    {
        animator.Play("transition out");
    }
}
