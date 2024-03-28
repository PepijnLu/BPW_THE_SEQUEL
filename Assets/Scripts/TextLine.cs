using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLine : MonoBehaviour
{
    public string textToDisplay;
    public int duration;
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            StartCoroutine(ShowText());
        }
    }

    IEnumerator ShowText()
    {
        while (!TurnManager.instance.isPlayerTurn)
        {
            yield return null;
        }
        UIManager.instance.StartShowTextRoutine(textToDisplay, duration, UIManager.instance.tutorialText);
        gameObject.SetActive(false);
    }
}
