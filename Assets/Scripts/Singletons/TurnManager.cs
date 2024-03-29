using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public bool isPlayerTurn, manuallyPassed;
    public PlayerController playerController;
    public int enemiesTurnDone;
    void Awake()
    {
        isPlayerTurn = true;
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //failsafe
        if (!isPlayerTurn && GameManager.instance.enemies.Count <= 0)
        {
            PassToPlayer();
        }

        if (!isPlayerTurn && (!GameManager.instance.stopped) && (TurnManager.instance.enemiesTurnDone == GameManager.instance.enemies.Count))
        {
            PassToPlayer();
        }
    }

    public void CheckActions(GameObject obj)
    {
        if ( (obj.tag == "Player") )
        {
            if ( (obj.GetComponent<Stats>().turnDone == true) && (PickupManager.instance.selecting == false))
            {
                SwapTurns(obj);
                Debug.Log("swap to enemy");
            }
        }
        if ( (obj.tag == "Enemy") )
        {
            Debug.Log("Enemy Turn Done");
            enemiesTurnDone++;
            if ( (enemiesTurnDone >= GameManager.instance.enemies.Count))
            {
                enemiesTurnDone = 0;
                SwapTurns(obj);
                Debug.Log("swap to player");
            }
        }
    }
    public void SwapTurns(GameObject obj)
    {
        //Pass the turn to the enemy
        if (obj.tag == "Player")
        {
            PassToEnemy(obj);
        }
        if (obj.tag == "Enemy")
        {
            PassToPlayer();
        }
    }

    public void PassToPlayer() 
    {
        if (!manuallyPassed)
        {
            manuallyPassed = true;
            GameManager.instance.playerStats.moves = 0;
            ProcGen.instance.playerController.turnStarted = false;
            GameManager.instance.playerStats.turnDone = false;
            isPlayerTurn = true;
            Debug.Log("turn started player");
            Movement.instance.movesRemainingTxt.text = ("Moves remaining: " + (GameManager.instance.playerStats.maxMoves - GameManager.instance.playerStats.moves).ToString());
            TurnManager.instance.enemiesTurnDone = 0;
            manuallyPassed = false;
        }
    }

    public void PassToEnemy(GameObject obj)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Stats enemyStats = enemy.GetComponent<Stats>();
            enemyStats.fired = false;
            enemyStats.moves = 0;
            enemyStats.turnDone = false;
            enemy.GetComponent<EnemyController>().turnStarted = false;
        }

        isPlayerTurn = false;
        Movement.instance.movesRemainingTxt.text = ("Moves remaining: " + (obj.GetComponent<Stats>().moves).ToString());
        Debug.Log("turn started enemy");
    }
}