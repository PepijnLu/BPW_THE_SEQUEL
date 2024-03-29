using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool turnStarted;
    public int randomDirection;
    private GameObject player;
    public List<GameObject> loot;

    //CODE A NEV MESH AGENT
    int firstDir;
    int secondDir;
    int thirdDir;
    int fourthDir;
    public int direction;
    int zero;
    bool succesfullyMoved;
    public bool enemyChillTime;
    public bool collidingWithEnemy;
    List<int> directions;
    List<GameObject> directionsGO;
    public GameObject square, laser;
    public GameObject up, down, left, right;
    public GameObject laserUp, laserDown, laserLeft, laserRight;
    public Sprite attacker, defender;
    Stats enemyStats;
    bool chillTimeStarted;
    public int blocksSurrounding;
    public bool notDoneFiring;
    public Vector2 distanceToPlayer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        directions = new List<int>(){zero, zero, zero, zero};
        directionsGO = new List<GameObject>(){null, null, null, null};
        GameManager.instance.enemies.Add(gameObject);
        enemyStats = gameObject.GetComponent<Stats>();
        enemyStats.collisionCheck = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnManager.instance.isPlayerTurn == false && turnStarted == false && enemyStats.moveStarted == false && (enemyStats.moves < enemyStats.maxMoves))
        {
            if (enemyStats.collisionCheck)
            {
                enemyStats.collisionCheck = false;
                CheckForPossibleMovement();
            }
            else if (!GameManager.instance.stopped)
            {
                //enemyChillTime = true;
                EnemyTurn();
            }
            // enemyChillTime = true;
            // EnemyTurn();
            // turnStarted = true;
        }
    }

    public IEnumerator EnemyChillTime()
    {
        //chillTimeStarted = true;
        yield return new WaitForSeconds(0.05f);
        //chillTimeStarted = false;
        //enemyChillTime = false;
        EnemyMove();
    }
    public void CheckForPossibleMovement()
    {
        Debug.Log("check for possible movement enemy");
        Vector2 roundedVector = new Vector2((Mathf.Round(transform.position.x * 2) / 2f), (Mathf.Round(transform.position.y * 2) / 2f));
        up = Instantiate(square, roundedVector + new Vector2(0, 1), Quaternion.identity);
        up.GetComponent<CanMove>().enemy = gameObject;
        directionsGO[0] = up;
        down = Instantiate(square, roundedVector + new Vector2(0, -1), Quaternion.identity);
        down.GetComponent<CanMove>().enemy = gameObject;
        down.transform.rotation = Quaternion.Euler(0, 0, 180);
        directionsGO[1] = down;
        right = Instantiate(square, roundedVector + new Vector2(1, 0), Quaternion.identity);
        right.GetComponent<CanMove>().enemy = gameObject;
        right.transform.rotation = Quaternion.Euler(0, 0, -90);
        directionsGO[2] = right;
        left = Instantiate(square, roundedVector + new Vector2(-1, 0), Quaternion.identity);
        left.GetComponent<CanMove>().enemy = gameObject;
        left.transform.rotation = Quaternion.Euler(0, 0, 90);
        directionsGO[3] = left;
    }

    public void NukeDirections()
    {
        Destroy(up);
        Destroy(down);
        Destroy(left);
        Destroy(right);
    }
    
    void EnemyTurn()
    {
        if (!enemyStats.moveStarted)
        {
            enemyStats.moveStarted = true;
            enemyStats.collisionCheck = true;
            StartCoroutine(EnemyChillTime());
        }
    }

    public void EnemyMove()
    {
        Debug.Log("Start void enemy move");
        bool succesfullyMovedLocal = false;
        Debug.Log("EnemyMove: enemymove");
        if (!enemyStats.inBattle)
        {
            Vector2 distanceToCenterRoom = new Vector2(0, 0);
            Vector2 distanceToPlayer = (gameObject.transform.position - GameManager.instance.player.transform.position);
            // float roomSizeToCheck = 0;
            // if (enemyStats.mainRoomInt != 0)
            // {
            //     distanceToCenterRoom = (gameObject.transform.position - ProcGen.instance.mainRoomLocations[("mainRoomLocation" + enemyStats.mainRoomInt.ToString())]);
            //     roomSizeToCheck = ProcGen.instance.mainRoomLengthWidthSize[(("mainRoomLWS") + enemyStats.mainRoomInt.ToString())].z;
            // }
            // else if (enemyStats.extraRoomInt != 0)
            // {
            //     distanceToCenterRoom = (gameObject.transform.position - ProcGen.instance.extraRoomLocations[("extraRoomLocation" + enemyStats.extraRoomInt.ToString())]);
            //     roomSizeToCheck = ProcGen.instance.extraRoomLengthWidthSize[(("extraRoomLWS") + enemyStats.extraRoomInt.ToString())].z;
            // }
            succesfullyMoved = false;
            //randomDirection = Random.Range(1, 5);
            CalculateDirection();

            for (int i = 0; i < 4; i++)
            {
                if ((enemyStats.enemyInt == 3) && (distanceToPlayer.magnitude < 5))
                {
                    if ( (directionsGO[directions[directions.Count - 1 - i] - 1] != null) && (succesfullyMovedLocal == false) /* && (distanceToPlayer.magnitude < 10) && ((roomSizeToCheck == 0) || (distanceToCenterRoom.magnitude < roomSizeToCheck - 2))*/ )
                    {
                        //Debug.Log("EnemyMove: LaserEnemy");
                        Movement.instance.MoveTile(directions[(directions.Count - 1 - i)], gameObject, up, down, right, left);
                        succesfullyMovedLocal = true;
                    }
                }
                else
                {
                    if ( (directionsGO[directions[i] - 1] != null) && (succesfullyMovedLocal == false) && (distanceToPlayer.magnitude < 10) /* && ((roomSizeToCheck == 0) || (distanceToCenterRoom.magnitude < roomSizeToCheck - 2))*/ )
                    {
                        //Debug.Log("EnemyMove: NormalEnemy");
                        Movement.instance.MoveTile(directions[i], gameObject, up, down, right, left);
                        succesfullyMovedLocal = true;
                    }
                }
            }

            if (succesfullyMovedLocal == false)
            {
                //enemyStats.moves++;
                succesfullyMovedLocal = true;
                //TurnManager.instance.CheckActions(gameObject);
                Movement.instance.MoveTile(0, gameObject, up, down, right, left);
            }
            Debug.Log("succesfullyMoved: " + succesfullyMovedLocal);
        }
        Debug.Log("succesfullyMoved: " + succesfullyMovedLocal);
        Debug.Log("End void enemy move");
    }

    void CalculateDirection()
    {
        direction = 0;
        
        Vector2 enemyPos = transform.position;
        Vector2 playerPos = player.transform.position;

        Vector2 difference = enemyPos - playerPos;
        //Debug.Log(difference);  
        
        if ( (Mathf.Abs(difference.x) >= Mathf.Abs(difference.y)) && difference.x >= 0)
        {
            //right first
            directions[0] = 4;
            directions[3] = 3;

            if (difference.y >= 0)
            {
                directions[1] = 2;
                directions[2] = 1;
            }
            else
            {
                directions[1] = 1;
                directions[2] = 2;
            }
        }
       
        if ( (Mathf.Abs(difference.x) >= Mathf.Abs(difference.y)) && difference.x < 0)
        {
            //left first
            directions[0] = 3;
            directions[3] = 4;

            if (difference.y >= 0)
            {
                directions[1] = 2;
                directions[2] = 1;
            }
            else
            {
                directions[1] = 1;
                directions[2] = 2;
            }
        }

        if ( (Mathf.Abs(difference.x) < Mathf.Abs(difference.y)) && difference.y >= 0)
        {
            //down first
            directions[0] = 2;
            directions[3] = 1;

            if (difference.x >= 0)
            {
                directions[1] = 4;
                directions[2] = 3;
            }
            else
            {
                directions[1] = 3;
                directions[2] = 4;
            }
        }

        if ( (Mathf.Abs(difference.x) < Mathf.Abs(difference.y)) && difference.y < 0)
        {
            //up first
            directions[0] = 1;
            directions[3] = 2;

            if (difference.x >= 0)
            {
                directions[1] = 4;
                directions[2] = 3;
            }
            else
            {
                directions[1] = 3;
                directions[2] = 4;
            }
        }
    }

    public IEnumerator FireLaser()
    {
        notDoneFiring = true;
        laserUp = Instantiate(laser, gameObject.transform.position + new Vector3(0, 0.5f, 0), gameObject.transform.rotation);
        laserUp.transform.Rotate(0f, 0f, 0f);
        laserUp.GetComponent<Laser>().enemy = gameObject;
        laserDown = Instantiate(laser, gameObject.transform.position + new Vector3(0, -0.5f, 0), gameObject.transform.rotation);
        laserDown.transform.Rotate(0f, 0f, 180f);
        laserDown.GetComponent<Laser>().enemy = gameObject;
        laserLeft = Instantiate(laser, gameObject.transform.position + new Vector3(-0.5f, 0, 0), gameObject.transform.rotation);
        laserLeft.transform.Rotate(0f, 0f, 90f);
        laserLeft.GetComponent<Laser>().enemy = gameObject;
        laserRight = Instantiate(laser, gameObject.transform.position + new Vector3(0.5f, 0, 0), gameObject.transform.rotation);
        laserRight.transform.Rotate(0f, 0f, -90f);
        laserRight.GetComponent<Laser>().enemy = gameObject;

        while ((laserUp != null) || (laserDown != null) || (laserLeft != null) || (laserRight != null))
        {
            yield return null;
        }
        Debug.Log("lasers: " + laserUp + laserDown + laserLeft + laserRight);
        notDoneFiring = false;
        yield return null;
    }

    void OnDestroy()
    {
        GameManager.instance.enemies.Remove(gameObject);
    }
    void OnDisable()
    {
        GenerateLoot();
        GameManager.instance.enemies.Remove(gameObject);
        if (!Tutorial.instance)
        {
            GameManager.instance.EnemyDeath(enemyStats.mainRoomInt, enemyStats.extraRoomInt);
            Debug.Log("RIP");
        }
        Destroy(up);
        Destroy(down);
        Destroy(left);
        Destroy(right);
    }

    void GenerateLoot()
    {
        int lootInt = Random.Range(1, 4);
        //int lootInt = 1;
        Debug.Log("tutorial instance: " + Tutorial.instance);
        if (Tutorial.instance != null)
        {
            if(Tutorial.instance.tutorialPhase == 2)
            {
                GameObject newLoot = Instantiate(loot[0], transform.position, transform.rotation);
            }
        }
        else
        {
            if (lootInt == 1)
            {
                GameObject newLoot = Instantiate(loot[lootInt - 1], transform.position, transform.rotation);
            }
        }

    }

    void OnMouseEnter()
    {
        if ((GameManager.instance.stopped == false) || (Tutorial.instance == true))
        {
            gameObject.GetComponent<Stats>().card.SetActive(true);
        }
    }
    void OnMouseExit()
    {
        if (GameManager.instance.stopped == false || (Tutorial.instance == true))
        {
            gameObject.GetComponent<Stats>().card.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            collidingWithEnemy = true;
        }
    }

    void OnCollisionExit2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            collidingWithEnemy = false;
        }
    }
}
