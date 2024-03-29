using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public bool isPlayerTurn, manuallyPassed;
    public PlayerController playerController;
    public int enemiesTurnDone;
    bool failSafe;
    void Awake()
    {
        isPlayerTurn = true;
        instance = this;
    }
    // Start is called before the first frame update

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

        if ((!TurnManager.instance.isPlayerTurn) && (!failSafe))
        {
            StartCoroutine(EnemyTurnFailSafe());
        }
    }

    IEnumerator EnemyTurnFailSafe()
    {
        failSafe = true;
        bool broken = false;
        float duration = 0f;
        while (duration < 5)
        {
            duration += Time.deltaTime;
            if (GameManager.instance.stopped)
            {
                broken = true;
                break;
            }
            yield return null;
        }
        if (!broken)
        {
            PassToPlayer();
        }
        failSafe = false;
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
            PassToEnemy();
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

    public void PassToEnemy()
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
        Movement.instance.movesRemainingTxt.text = ("Moves remaining: " + 0);
        Debug.Log("turn started enemy");
    }
}
