using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CanMove : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;

    void Start()
    {
        if (enemy != null)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "Tilemap")
    //     {
    //         if (enemy != null)
    //         {
    //             EnemyController enemyController = enemy.GetComponent<EnemyController>();
    //             enemyController.blocksSurrounding++;
    //             if (enemyController.blocksSurrounding == 4)
    //             {
    //                 Destroy(enemy);
    //             }
    //             else
    //             {
    //                 enemyController.blocksSurrounding = 0;
    //             }
    //         }
    //         Debug.Log("destroy canmove");
    //         Destroy(gameObject);
    //     }
    // }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if ( (collider.gameObject.tag == "Tilemap") || 
             (collider.gameObject.tag == "Arrow") || 
             (collider.gameObject.tag == "Chest") ||
             ( (collider.gameObject.tag == "Enemy") && ( (collider.gameObject != enemy) && (enemy != null) ) ) )
        {
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
