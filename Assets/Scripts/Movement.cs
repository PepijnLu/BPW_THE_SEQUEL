using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{
    public static Movement instance;
    public bool resetting;
    public TextMeshProUGUI movesRemainingTxt;

    void Awake()
    {
        instance = this;
    }
    
    public void MoveTile(int direction, GameObject objectToMove, GameObject up, GameObject down, GameObject right, GameObject left)
    {
        Debug.Log("EnemyMove: " + objectToMove.tag);
        Vector3 oldPosition = objectToMove.transform.position;
        Vector3 newPosition = new Vector3(0, 0, 0);
        bool moved = false;
        Stats objectToMoveStats = objectToMove.GetComponent<Stats>();
        objectToMoveStats.moves++;

        switch(direction)
        {
            case 1:
                if (up != null)
                {
                    StartCoroutine(MoveSmoothly(objectToMove, up, 0.2f, up, down, right, left));
                    moved = true;
                }
                break;
            case 2:
                if (down != null)
                {
                    StartCoroutine(MoveSmoothly(objectToMove, down, 0.2f, up, down, right, left));
                    moved = true;
                }
                break;
            case 3:
                if (right != null)
                {
                    StartCoroutine(MoveSmoothly(objectToMove, right, 0.2f, up, down, right, left));
                    moved = true;
                }
                break;
            case 4:
                if (left != null)
                {
                    StartCoroutine(MoveSmoothly(objectToMove, left, 0.2f, up, down, right, left));
                    moved = true;
                }
                break;
        }

        if (!moved)
        {
            StartCoroutine(EndMove(objectToMoveStats));
            Debug.Log("wrong");
        }
    }

    public IEnumerator MoveSmoothly(GameObject objectToMove, GameObject objectToMoveTo, float duration, GameObject up, GameObject down, GameObject right, GameObject left)
    {
        Stats stats = objectToMove.GetComponent<Stats>();
        stats.moveStarted = true;

        if ( (objectToMove.tag == "Enemy") && (objectToMove.GetComponent<Stats>().inBattle == true))
        {
            // Debug.Log("In battle");
            // stats.moves++;
        }
        else
        {
            Debug.Log("MoveSmoothly");
            Vector2 startPosition = objectToMove.transform.position;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                if (Movement.instance.resetting == false)
                {
                    objectToMove.transform.position = Vector2.Lerp(startPosition, objectToMoveTo.transform.position, t);
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            if (Movement.instance.resetting == false)
            {
                objectToMove.transform.position = objectToMoveTo.transform.position;
            }

            Destroy(up);
            Destroy(down);
            Destroy(right);
            Destroy(left);

            Debug.Log("nuke directions");

            //stats.moves++;

            if (objectToMove.tag == "Player")
            {
                movesRemainingTxt.text = ("Moves remaining: " + (stats.maxMoves - stats.moves).ToString());
            }
        }
        yield return new WaitForSeconds(0.025f);
        // if (objectToMove.tag == "Enemy")
        // {
        //     EnemyController enemyController = stats.gameObject.GetComponent<EnemyController>();
        //     while (enemyController.notDoneFiring)
        //     {
        //         yield return null;
        //     }
        // }
        if (!GameManager.instance.stopped)
        {
            StartCoroutine(EndMove(stats));
        }
        else
        {
            if (!BattleManager.instance.needToFinish.Contains(stats))
            {
                BattleManager.instance.needToFinish.Add(stats);
            }
        }
    }

    public IEnumerator EndMove(Stats stats)
    {
        Debug.Log("EnemyMove: end move: " + stats.gameObject.tag);
        if ( (stats.moves >= stats.maxMoves) && GameManager.instance.stopped == false)
        {
            if (stats.gameObject.tag == "Player" && TurnManager.instance.isPlayerTurn)
            {
                PlayerController playerController = stats.gameObject.GetComponent<PlayerController>();
                playerController.NukeDirections();
                playerController.CheckForPossibleMovement();
                yield return new WaitForSeconds(0.025f);
                playerController.NukeDirections();
            }
            else if ((!TurnManager.instance.isPlayerTurn) && stats.gameObject.tag == "Enemy")
            {
                EnemyController enemyController = stats.gameObject.GetComponent<EnemyController>();
                enemyController.NukeDirections();
                enemyController.CheckForPossibleMovement();
                yield return new WaitForSeconds(0.025f);
                enemyController.NukeDirections();
                if ((stats.enemyInt == 3) && (!stats.fired))
                {
                    Vector2 distanceToPlayer = (stats.gameObject.transform.position - GameManager.instance.player.transform.position);
                    stats.fired = true;
                    Debug.Log("fire attempt at: " + distanceToPlayer.magnitude);
                    if ((distanceToPlayer.magnitude < 8) && (distanceToPlayer.magnitude > 1.5))
                    {
                        StartCoroutine(enemyController.FireLaser());
                        while (enemyController.notDoneFiring)
                        {
                            yield return null;
                        }
                    }
                }
            }
            if ((!GameManager.instance.stopped) && (!stats.turnDone))
            {
                stats.turnDone = true;
                Debug.Log(stats.gameObject + "turn done");
                if (BattleManager.instance.needToFinish.Contains(stats))
                {
                    BattleManager.instance.needToFinish.Remove(stats);
                }
                TurnManager.instance.CheckActions(stats.gameObject);
                //StartCoroutine(EndMove(stats));
            }
            else if (GameManager.instance.stopped)
            {
                if (!BattleManager.instance.needToFinish.Contains(stats))
                {
                    BattleManager.instance.needToFinish.Add(stats);
                }
            }
        }
        else if (!GameManager.instance.stopped)
        {
            foreach(Stats listStats in BattleManager.instance.needToFinish)
            {
                StartCoroutine(Movement.instance.EndMove(listStats));
            }
        }
        Debug.Log("12345 SET BOOLS TO FALSE");
        stats.collisionCheck = true;
        stats.moveStarted = false;
    }
}
