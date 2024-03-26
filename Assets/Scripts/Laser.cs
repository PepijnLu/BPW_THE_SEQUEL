using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public int speed;
    public int damage;
    public GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(0f, (1f * speed * Time.deltaTime), 0f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if ( (collider.gameObject.tag == "Tilemap") || (collider.gameObject.tag == "Player") )
        {
            if (collider.gameObject.tag == "Player")
            {
                BattleManager.instance.StartTakeDamageRoutine(damage, enemy);
            }

            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController.laserUp == gameObject)  {   enemyController.laserUp = null; }
            if (enemyController.laserDown == gameObject)  {   enemyController.laserDown = null; }
            if (enemyController.laserLeft == gameObject)  {   enemyController.laserLeft = null; }
            if (enemyController.laserRight == gameObject)  {   enemyController.laserRight = null; }
            Destroy(gameObject);
        }
    }
}
