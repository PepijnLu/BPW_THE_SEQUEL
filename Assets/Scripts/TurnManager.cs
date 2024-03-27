using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public bool isPlayerTurn;
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
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     isPlayerTurn = true;
        //     playerController.turnStarted = false;
        //     GameManager.instance.player.GetComponent<PlayerController>().CheckForPossibleMovement();
        // }
        
    }

    public void CheckActions(GameObject obj)
    {

        if ( (obj.tag == "Player") )
        {
            if ( (obj.GetComponent<Stats>().turnDone == true) && (PickupManager.instance.selecting == false))
            {
                obj.GetComponent<Stats>().turnDone = false;
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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (obj.tag == "Player")
            {
                isPlayerTurn = false;
                obj.GetComponent<Stats>().moves = 0;
                Movement.instance.movesRemainingTxt.text = ("Moves remaining: " + (obj.GetComponent<Stats>().moves).ToString());
            
                Debug.Log("turn started enemy");
            }
            if (obj.tag == "Enemy")
            {
                foreach (GameObject enemy in enemies)
                {
                    Stats enemyStats = enemy.GetComponent<Stats>();
                    enemyStats.fired = false;
                    enemyStats.moves = 0;
                    enemyStats.turnDone = false;
                    enemy.GetComponent<EnemyController>().turnStarted = false;
                    //enemy.GetComponent<EnemyController>().CheckForPossibleMovement();
                    //StartCoroutine(enemy.GetComponent<EnemyController>().EnemyChillTime());
                }
                Debug.Log("turn started player");
                isPlayerTurn = true;
                GameManager.instance.player.GetComponent<PlayerController>().turnStarted = false;
                Movement.instance.movesRemainingTxt.text = ("Moves remaining: " + (GameManager.instance.playerStats.maxMoves - GameManager.instance.playerStats.moves).ToString());
                //GameManager.instance.player.GetComponent<PlayerController>().CheckForPossibleMovement();
                //StartCoroutine(Movement.instance.EndMove(GameManager.instance.player.GetComponent<Stats>()));
                // GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                // foreach (GameObject player in players)
                // {
                //     player.GetComponent<PlayerController>().turnStarted = false;
                // }
            }
    }
}
