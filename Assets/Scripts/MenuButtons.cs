using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public int buttonInt;
    public void WhenClicked()
    {
        switch(buttonInt)
        {
            //START
            case 1:
            SceneManager.LoadScene("GameScene");
                break;
            //SETTINGS
            case 2:
                MenuManager.instance.SetActiveUI(MenuManager.instance.mainMenu, false);
                MenuManager.instance.SetActiveUI(MenuManager.instance.settingsMenu, true);
                break;
            //BACK TO MENU
            case 3:
                MenuManager.instance.SetActiveUI(MenuManager.instance.mainMenu, true);
                MenuManager.instance.SetActiveUI(MenuManager.instance.settingsMenu, false);
                break;
            //BACK TO MAIN MENU
            case 4:
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }
}
