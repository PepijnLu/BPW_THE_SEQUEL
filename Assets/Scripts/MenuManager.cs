using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public GameObject mainMenu, settingsMenu;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        GameObject[] deactivates = GameObject.FindGameObjectsWithTag("Deactivate");
        foreach (GameObject deactivate in deactivates)
        {
            deactivate.SetActive(false);
        }
    }

    public void SetActiveUI(GameObject obj, bool maybe)
    {
        obj.SetActive(maybe);
    }
}
