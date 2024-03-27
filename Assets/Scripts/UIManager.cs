using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI tutorialText;

    void Awake()
    {
        instance = this;
    }
    public IEnumerator ShowTextForDuration(string text, float duration, TextMeshProUGUI textToWriteTo)
    {
        Debug.Log("show text: " + text);
        GameManager.instance.stopped = true;
        textToWriteTo.gameObject.SetActive(true);
        textToWriteTo.text = text;
        yield return new WaitForSeconds(duration);
        textToWriteTo.text = "";
        textToWriteTo.gameObject.SetActive(false);
        GameManager.instance.stopped = false;
    }
}
