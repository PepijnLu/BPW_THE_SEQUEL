using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tutorial : MonoBehaviour
{
    public int tutorialEnemiesSpawned;
    public bool tutorial;
    public static Tutorial instance;
    public List<Transform> doorLocations, enemyLocations;
    public Tilemap tutorialCollisionMap, tutorialDecorationMap;
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
        while (TurnManager.instance.isPlayerTurn) {yield return null;}
        UIManager.instance.StartShowTextRoutine("This game is turn based, so you need to wait until all enemies have done their turn.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        UIManager.instance.StartShowTextRoutine("Right now there are no enemies yet. Let's fix that.", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        tutorialEnemiesSpawned++;
        Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[0].position, Quaternion.identity);
        while (!TurnManager.instance.isPlayerTurn) {yield return null;}
        UIManager.instance.StartShowTextRoutine("Now that the enemy has made it's turn, it's your turn again.", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        while (TurnManager.instance.isPlayerTurn) {yield return null;}
        while (!TurnManager.instance.isPlayerTurn) {yield return null;}
        UIManager.instance.StartShowTextRoutine("If you move next to an enemy, you'll iniate an battle, and vice versa.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        UIManager.instance.StartShowTextRoutine("Try iniating a battle!", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        while (!BattleManager.instance.battle) {yield return null;}
        UIManager.instance.StartShowTextRoutine("Attack!", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        while (BattleManager.instance.battle) {yield return null;}
        UIManager.instance.StartShowTextRoutine("Good job killing your first enemy!", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        UIManager.instance.StartShowTextRoutine("Now that there are no enemies left in the room, the doors will open!", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        OpenTutorialDoor(doorLocations[0]);
        OpenTutorialDoor(doorLocations[1]);
        OpenTutorialDoor(doorLocations[2]);
        tutorialEnemiesSpawned++;
        Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[1].position, Quaternion.identity);
    }

    void OpenTutorialDoor(Transform location)
    {
        Vector3Int cellPosition = tutorialCollisionMap.WorldToCell(location.position);
        tutorialCollisionMap.SetTile(cellPosition, null);
        tutorialDecorationMap.SetTile(cellPosition, null);
    }   
}
