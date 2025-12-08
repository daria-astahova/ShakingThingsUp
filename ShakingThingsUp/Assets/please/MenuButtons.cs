using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuButtons : MonoBehaviour
{
    //Creates a set of functions that any buttons can use
    public GameObject currentMenu;
    public GameObject creditsMenu;

    //Moves to the second scene in the list
    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void PauseGame() {
        Time.timeScale = 0f;
    } 

    public void UnpauseGame() {
        Time.timeScale = 1f;
    }

    public void OpenCredits() {
        currentMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void CloseCredits() {
        currentMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    //Quits the application
    public void ExitGame() {
        Debug.Log("Quiting Game.");
        Application.Quit();
    }
}
