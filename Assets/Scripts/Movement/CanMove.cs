using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CanMove : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    Stats enemyStats;
    Vector2 distanceToCenterRoom;
    float roomSizeToCheck;
    EnemyController ec;

    void Start()
    {
        if (enemy != null)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            ec = enemy.GetComponent<EnemyController>();
            enemyStats = enemy.GetComponent<Stats>();
            if (enemyStats.mainRoomInt != 0)
            {
                distanceToCenterRoom = (gameObject.transform.position - ProcGen.instance.mainRoomLocations[("mainRoomLocation" + enemyStats.mainRoomInt.ToString())]);
                roomSizeToCheck = ProcGen.instance.mainRoomLengthWidthSize[(("mainRoomLWS") + enemyStats.mainRoomInt.ToString())].z;
            }
            else if (enemyStats.extraRoomInt != 0)
            {
                distanceToCenterRoom = (gameObject.transform.position - ProcGen.instance.extraRoomLocations[("extraRoomLocation" + enemyStats.extraRoomInt.ToString())]);
                roomSizeToCheck = ProcGen.instance.extraRoomLengthWidthSize[(("extraRoomLWS") + enemyStats.extraRoomInt.ToString())].z;
            }
            if ((distanceToCenterRoom.magnitude > (roomSizeToCheck - 2)) && enemy != null && Tutorial.instance == null)
            {
                Debug.Log("too far away: " + distanceToCenterRoom .magnitude + " > " + (roomSizeToCheck - 2));
                //set it to null
                switch(gameObject.transform.rotation.z)
                {
                    case 0:
                        ec.up = null;
                        break;
                    case 180:
                        ec.down = null;
                        break;
                    case -90:
                        ec.right = null;
                        break;
                    case 90:
                        ec.left = null;
                        break;
                }
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if ( (collider.gameObject.tag == "Tilemap") || 
             (collider.gameObject.tag == "Arrow") || 
             ((collider.gameObject.tag == "Chest") && (!TurnManager.instance.isPlayerTurn)) ||
             ( (collider.gameObject.tag == "Enemy") && ( (collider.gameObject != enemy) && (enemy != null) ) ) )
        {
            //Debug.Log("distanceToCenter" + (distanceToCenterRoom.magnitude).ToString());
            //Debug.Log("room size to check: " + roomSizeToCheck);
            if ((enemy != null) && (collider.gameObject.tag == "Tilemap"))
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                enemyController.blocksSurrounding++;
                Debug.Log("blocks surrouding: " + enemyController.blocksSurrounding);
                if (enemyController.blocksSurrounding == 4)
                {
                    Destroy(enemy);
                }
                else
                {
                    enemyController.blocksSurrounding = 0;
                }
            }
            Destroy(gameObject);
        }

        if ( (collider.gameObject.tag == "Player") && (enemy != null) )
        {
            BattleManager.instance.Battle(GameManager.instance.player, enemy, true, false);
            EnemyController enemyController = enemy.gameObject.GetComponent<EnemyController>();
            enemyController.NukeDirections();
        }

        if ( (collider.gameObject.tag == "Enemy") && (enemy == null) )
        {
            BattleManager.instance.Battle(GameManager.instance.player, collider.gameObject, true, true);
            PlayerController playerController = player.gameObject.GetComponent<PlayerController>();
            playerController.NukeDirections();
        }
    }
}
