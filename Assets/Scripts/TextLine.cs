using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLine : MonoBehaviour
{
    public string textToDisplay;
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("Collision with player");
            StartCoroutine(UIManager.instance.ShowTextForDuration(textToDisplay, 5f, UIManager.instance.tutorialText));
        }
    }
}
