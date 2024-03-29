using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tutorial : MonoBehaviour
{
    public int tutorialEnemiesSpawned;
    public bool tutorial;
    public static Tutorial instance;
    public List<Transform> doorLocations, enemyLocations, chestLocations;
    public Transform boxedEnemy, tutorialExitLocation;
    public Tilemap tutorialGroundMap, tutorialCollisionMap, tutorialDecorationMap, tutorialChestMap, tutorialExitMap;
    public int tutorialPhase;

    /* 
    -assign tutorial ground map
    -fix ranged enemy
    */

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
        tutorialPhase = 1;
        Instantiate(ProcGen.instance.enemyPrefab, boxedEnemy.position, Quaternion.identity);
        tutorial = true;
        while (TurnManager.instance.isPlayerTurn) {yield return null;}
        GameManager.instance.stopped = true;
        UIManager.instance.StartShowTextRoutine("This game is turn based, so you need to wait until all enemies have done their turn.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        UIManager.instance.StartShowTextRoutine("Right now there are no enemies yet. Let's fix that.", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        tutorialEnemiesSpawned++;
        Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[0].position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        tutorialEnemiesSpawned++;
        GameObject newEnemy2 = Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[1].position, Quaternion.identity);
        GameObject enemyCard = newEnemy2.GetComponent<Stats>().card;
        yield return new WaitForSeconds(0.1f);
        tutorialEnemiesSpawned++;
        GameObject newEnemy3 = Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[2].position, Quaternion.identity);
        EnemyController ec = newEnemy3.GetComponent<EnemyController>();
        yield return new WaitForSeconds(0.1f);
        tutorialEnemiesSpawned++;
        GameObject newEnemy4 = Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[3].position, Quaternion.identity);
        GameManager.instance.stopped = false;
         yield return new WaitForSeconds(0.1f);
        tutorialEnemiesSpawned++;
        GameObject newEnemy5 = Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[4].position, Quaternion.identity);
        GameManager.instance.stopped = false;
         yield return new WaitForSeconds(0.1f);
        tutorialEnemiesSpawned++;
        GameObject newEnemy6 = Instantiate(ProcGen.instance.enemyPrefab, enemyLocations[5].position, Quaternion.identity);
        GameManager.instance.stopped = false;

        while (!TurnManager.instance.isPlayerTurn) {yield return null;}
        //GameManager.instance.stopped = true;
        //GameManager.instance.stopped = false;
        UIManager.instance.StartShowTextRoutine("Now that the enemy has made it's turn, it's your turn again.", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        while (TurnManager.instance.isPlayerTurn) {yield return null;}
        while (!TurnManager.instance.isPlayerTurn) {yield return null;}
        UIManager.instance.StartShowTextRoutine("If you move next to an enemy, you'll iniate a battle, and vice versa.", 3f, UIManager.instance.tutorialText);
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
        AudioManager.instance.PlaySound(AudioManager.instance.audioSources["openRoomSFX"]);
        OpenTutorialDoor(doorLocations[0]);
        OpenTutorialDoor(doorLocations[1]);
        OpenTutorialDoor(doorLocations[2]);
        GameManager.instance.stopped = false;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            StartCoroutine(Movement.instance.EndMove(enemy.GetComponent<Stats>()));
        }
        while (!enemyCard.activeSelf) {yield return null;}
        tutorialPhase = 2;
        yield return new WaitForSeconds(1f);
        UIManager.instance.StartShowTextRoutine("Press TAB to see your own stats. How do you compare?", 3f, UIManager.instance.tutorialText);
        //GameManager.instance.stopped = false;
        yield return new WaitForSeconds(3f);
        while (newEnemy2.activeSelf) {yield return null;}
        UIManager.instance.StartShowTextRoutine("Looks like the enemy dropped something on death, try and pick it up.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        foreach(Stats stats in BattleManager.instance.needToFinish)
        {
            StartCoroutine(Movement.instance.EndMove(stats));
        }
        while (!PickupManager.instance.selecting) {yield return null;}
        UIManager.instance.StartShowTextRoutine("You can only choose on of the options, so choose carefully!", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        while (PickupManager.instance.selecting) {yield return null;}
        yield return new WaitForSeconds(2f);
        UIManager.instance.StartShowTextRoutine("Before heading towards the exit, let's go left and look in the extra room first.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        UIManager.instance.StartShowTextRoutine("The way to the exit will always be up and right. Extra rooms are down or left.", 3f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(3f);
        //while (ec.distanceToPlayer.magnitude > 7) {yield return null;}
        //UIManager.instance.StartShowTextRoutine("There's a chest in the middle of the room, but it appears to be locked.", 3f, UIManager.instance.tutorialText);
        //yield return new WaitForSeconds(3f);
        //UIManager.instance.StartShowTextRoutine("Clear the room first.", 1f, UIManager.instance.tutorialText);
        while (newEnemy3.activeSelf || newEnemy4.activeSelf)
        {
            yield return null;
        }
        Vector3Int cellPosition = tutorialChestMap.WorldToCell(chestLocations[0].position);
        tutorialDecorationMap.SetTile(cellPosition, null);
        tutorialCollisionMap.SetTile(cellPosition, null);
        tutorialChestMap.SetTile(cellPosition, ProcGen.instance.chest);
        while (!ChestManager.instance.chestOpen) {yield return null;}
        yield return new WaitForSeconds(1f);
        UIManager.instance.StartShowTextRoutine("Cards! Collect them all!", 3f, UIManager.instance.tutorialText);
        while (GameManager.instance.stopped) {yield return null;}
        OpenTutorialDoor(doorLocations[3]);
        OpenTutorialDoor(doorLocations[4]);
        OpenTutorialDoor(doorLocations[5]);
        UIManager.instance.StartShowTextRoutine("You can now proceed past the doors.", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        while (TurnManager.instance.playerController.orbsCollected == 0)
        {
            yield return null;
        }
        UIManager.instance.StartShowTextRoutine("Collect 9 more of these to gain another move!", 3f, UIManager.instance.tutorialText);
        while (TurnManager.instance.playerController.orbsCollected != 0)
        {
            yield return null;
        }
        UIManager.instance.StartShowTextRoutine("You can now move once more per turn!", 2f, UIManager.instance.tutorialText);
        yield return new WaitForSeconds(2f);
        tutorialPhase = 3;
        while (!BattleManager.instance.parried) {yield return null;}
        UIManager.instance.StartShowTextRoutine("Finish him!", 1f, UIManager.instance.tutorialText);
        tutorialPhase = 4;
        while (newEnemy5.activeSelf) {yield return null;}
        OpenTutorialDoor(doorLocations[6]);
        OpenTutorialDoor(doorLocations[7]);
        OpenTutorialDoor(doorLocations[8]);
        OpenTutorialDoor(doorLocations[9]);
        OpenTutorialDoor(doorLocations[10]);
        while (newEnemy6.activeSelf) {yield return null;}
        OpenTutorialExit(tutorialExitLocation);
        UIManager.instance.StartShowTextRoutine("Use the stairs to head to the next dungeon layer!", 3f, UIManager.instance.tutorialText);
    }

    void OpenTutorialDoor(Transform location)
    {
        Vector3Int cellPosition = tutorialCollisionMap.WorldToCell(location.position);
        tutorialCollisionMap.SetTile(cellPosition, null);
        tutorialDecorationMap.SetTile(cellPosition, null);
    }   

    void OpenTutorialExit(Transform location)
    {
        Vector3Int cellPosition = tutorialExitMap.WorldToCell(location.position);
        tutorialCollisionMap.SetTile(cellPosition, null);
        tutorialDecorationMap.SetTile(cellPosition, null);
        tutorialExitMap.SetTile(cellPosition, ProcGen.instance.exit);
    }  

    public void EndTutorial()
    {
        tutorialGroundMap.ClearAllTiles();
        tutorialChestMap.ClearAllTiles();
        tutorialCollisionMap.ClearAllTiles();
        tutorialDecorationMap.ClearAllTiles();
        tutorialExitMap.ClearAllTiles();
        UIManager.instance.tutorialTextBox.SetActive(false);
        UIManager.instance.tutorialCornerTextBox.SetActive(false);
        GameManager.instance.playerStats.maxHealth = 5;
        GameManager.instance.playerStats.health = 5;
        GameManager.instance.playerStats.damage = 1;
        GameManager.instance.playerStats.maxMoves = 1;
        ProcGen.instance.playerController.orbsCollected = 0;
        instance = null;
        Destroy(gameObject);
    }
}
