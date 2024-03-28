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
        GameManager.instance.stopped = true;
        yield return new WaitForSeconds(1f);
        GameManager.instance.stopped = false;
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
        yield return new WaitForSeconds(0.5f);
        UIManager.instance.StartShowTextRoutine("Good job killing your first enemy!", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        UIManager.instance.StartShowTextRoutine("Now that there are no enemies left in the room, the doors will open!", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        foreach(Stats stats in BattleManager.instance.needToFinish)
        {
            Movement.instance.EndMove(stats);
        }
        AudioManager.instance.PlaySound(AudioManager.instance.audioSources["openRoomSFX"]);
        OpenTutorialDoor(doorLocations[0]);
        OpenTutorialDoor(doorLocations[1]);
        OpenTutorialDoor(doorLocations[2]);
        tutorialEnemiesSpawned++;
        GameObject newEnemy2 = Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[1].position, Quaternion.identity);
        GameObject enemyCard = newEnemy2.GetComponent<Stats>().card;
        while (!enemyCard.activeSelf) {yield return null;}
        yield return new WaitForSeconds(1f);
        UIManager.instance.StartShowTextRoutine("Press TAB to see your own stats. How do you compare?", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        while (newEnemy2.activeSelf) {yield return null;}
        UIManager.instance.StartShowTextRoutine("Looks like the enemy dropped something on death, try and pick it up.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        while (!PickupManager.instance.selecting) {yield return null;}
        UIManager.instance.StartShowTextRoutine("You can only choose on of the options, so choose carefully!", 3f, UIManager.instance.tutorialText);
        while (!PickupManager.instance.selecting) {yield return null;}
        yield return new WaitForSeconds(2f);
        GameObject newEnemy3 = Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[2].position, Quaternion.identity);
        EnemyController ec = newEnemy3.GetComponent<EnemyController>();
        GameObject newEnemy4 = Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[3].position, Quaternion.identity);
        UIManager.instance.StartShowTextRoutine("Before heading towards the exit, let's go left and look in the extra room first.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        UIManager.instance.StartShowTextRoutine("The way to the exit will always be up and right. Extra rooms down or left.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        while (ec.distanceToPlayer.magnitude < 7) {yield return null;}
        UIManager.instance.StartShowTextRoutine("There's a chest in the middle of the room, but it appears to be locked.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        UIManager.instance.StartShowTextRoutine("Clear the room first.", 1f, UIManager.instance.tutorialText);
        while (!ChestManager.instance.openingChest) {yield return null;}

    }

    void OpenTutorialDoor(Transform location)
    {
        Vector3Int cellPosition = tutorialCollisionMap.WorldToCell(location.position);
        tutorialCollisionMap.SetTile(cellPosition, null);
        tutorialDecorationMap.SetTile(cellPosition, null);
    }   
}
