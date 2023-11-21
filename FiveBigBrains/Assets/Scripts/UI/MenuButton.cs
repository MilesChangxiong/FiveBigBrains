using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public GameObject MenuBgObject;

    private void Start()
    {
        MenuBgObject.SetActive(false);
    }


    public void OnPauseButtonClick()
    {
        if (MenuBgObject.activeSelf)
        {
            return;
        }
        GameManager.instance.PauseGame();
        MenuBgObject.SetActive(true);
    }

    public void OnResumeButtonClick()
    {
        GameManager.instance.ResumeGame();
        MenuBgObject.SetActive(false);
    }

    public void OnMainMenuButtonClick()
    {
        GameManager.instance.ResumeGame();
        MenuBgObject.SetActive(false);
        GameManager.instance.RestartGameAfterVictory();
    }

}
