using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public bool tutorial;
    public Transform firstEnemyLocation;
    public static Tutorial instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        StartCoroutine(StartTutorial());
    }

    IEnumerator StartTutorial()
    {
        tutorial = true;
        while (TurnManager.instance.isPlayerTurn)
        {
            yield return null;
        }
        StartCoroutine(UIManager.instance.ShowTextForDuration("This game is turn based, so you need to wait until all enemies have done their turn", 4f, UIManager.instance.tutorialText));
        yield return new WaitForSeconds(5f);
        StartCoroutine(UIManager.instance.ShowTextForDuration("Right now there are no enemies yet. Let's fix that", 4f, UIManager.instance.tutorialText));
        yield return new WaitForSeconds(5f);
        Instantiate(ProcGen.instance.enemyPrefab, firstEnemyLocation.position, Quaternion.identity);
    }
}
