using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool isMouseOverButton;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetTrueMouseOverUI()
    {
        isMouseOverButton = true;
    }

    public void SetFalseMouseOverUI()
    {
        isMouseOverButton = false;
    }
}
