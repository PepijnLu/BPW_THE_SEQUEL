using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    public float speed;
    private Rigidbody2D rigidbodyComponent;
    public bool turnStarted;
    public Stats playerStats;
    public GameObject square;
    public GameObject up, down, left, right;
    public int orbsCollected, maxOrbs;
    public TextMeshProUGUI orbText;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = gameObject.GetComponent<Rigidbody2D>(); 
        Stats playerStats = gameObject.GetComponent<Stats>();
        Instantiate(playerStats.card, BattleManager.instance.playerCardSlot.position, Quaternion.identity);
        
        playerStats.card.SetActive(false);
        playerStats.collisionCheck = true;
        //CheckForPossibleMovement();
 
        Movement.instance.movesRemainingTxt.text = ("Moves remaining: " + (playerStats.maxMoves - playerStats.moves).ToString());
        GameManager.instance.orbText.text = maxOrbs.ToString();
        playerStats.SetValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnManager.instance.isPlayerTurn == true && turnStarted == false && playerStats.moveStarted == false && (playerStats.moves < playerStats.maxMoves) && GameManager.instance.stopped == false)
        {
            if (playerStats.collisionCheck)
            {
                playerStats.collisionCheck = false;
                Debug.Log("i should be shot in the head");
                CheckForPossibleMovement();
                //StartCoroutine(CollisionCheck());
            }
            else
            {
                PlayerTurn();
            }
        }
        //dev thingy
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BattleManager.instance.attackMade = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {   
            if (GameManager.instance.stopped == false)
            {
                playerStats.card.SetActive(true);
            }

            Movement.instance.movesRemainingTxt.gameObject.SetActive(true);

        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (GameManager.instance.stopped == false)
            {
                playerStats.card.SetActive(false);
            }

            Movement.instance.movesRemainingTxt.gameObject.SetActive(false);
        }
    }

    // IEnumerator CollisionCheck()
    // {
    //     yield return new WaitForSeconds(0.05f);
    //     playerStats.collisionCheck = false;
    // }

    public void CheckForPossibleMovement()
    {
        Debug.Log("12345 CHECK FOR POSSIBLE MOVEMTN PLAYER");
        Vector2 roundedVector = new Vector2((Mathf.Round(transform.position.x * 2) / 2f), (Mathf.Round(transform.position.y * 2) / 2f));
        if (up == null)
        {
            Debug.Log("instantiate up");
            up = Instantiate(square, roundedVector + new Vector2(0, 1), Quaternion.identity);
            up.GetComponent<CanMove>().player = gameObject;
        }
        if (down == null)
        {
            down = Instantiate(square, roundedVector + new Vector2(0, -1), Quaternion.identity);
            down.GetComponent<CanMove>().player = gameObject;
            down.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (right == null)
        {
            right = Instantiate(square, roundedVector + new Vector2(1, 0), Quaternion.identity);
            right.GetComponent<CanMove>().player = gameObject;
            right.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        if (left == null)
        {
            left = Instantiate(square, roundedVector + new Vector2(-1, 0), Quaternion.identity);
            left.GetComponent<CanMove>().player = gameObject;
            left.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    public void NukeDirections()
    {
        Debug.Log("nuke directions");
        Destroy(up);
        Destroy(down);
        Destroy(left);
        Destroy(right);
        up = null;
        down = null;
        left = null;
        right = null;
    }

    void FixedUpdate()
    {
       
    }

    void PlayerTurn()
    {
        Debug.Log("Player Turn");
        if (GameManager.instance.stopped == false)
        {
            if (Input.GetKeyDown(KeyCode.W) && up != null)
            {
                Movement.instance.MoveTile(1, gameObject, up, down, right, left);
                //playerStats.moveStarted = true;
                //playerStats.collisionCheck = true;
                //turnStarted = true;
            }
            if (Input.GetKeyDown(KeyCode.S) && down != null)
            {
                Movement.instance.MoveTile(2, gameObject, up, down, right, left);
                //playerStats.moveStarted = true;
                //playerStats.collisionCheck = true;
                //turnStarted = true;
            }
            if (Input.GetKeyDown(KeyCode.D ) && right != null)
            {
                Movement.instance.MoveTile(3, gameObject, up, down, right, left);
                //playerStats.moveStarted = true;
                //playerStats.collisionCheck = true;
                //turnStarted = true;
            }
            if (Input.GetKeyDown(KeyCode.A) && left != null)
            {
                Movement.instance.MoveTile(4, gameObject, up, down, right, left);
                //playerStats.moveStarted = true;
                //playerStats.collisionCheck = true;
                //turnStarted = true;
            }
                
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Exit")
        {
            Movement.instance.resetting = true;
            StartCoroutine(SetResetting());
            gameObject.transform.position = new Vector2(50.5f, 50.5f);
            gameObject.transform.rotation = Quaternion.identity;
            GameData.dungeonsCompleted++;
            //GameData.roomsCleared = 0;
            ProcGen.instance.GenerateDungeon();
        }

        if (collision.gameObject.tag == "Enemy")
        {
            //BattleManager.instance.Battle(gameObject, collision.gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Pickup")
        {
            collider.gameObject.GetComponent<Pickup>().Activate(gameObject);
        }

        if (collider.gameObject.tag == "Chest")
        {
            ChestManager.instance.Open();
        }
        if (collider.gameObject.tag == "Orb")
        {
            orbsCollected++;
            Destroy(collider.gameObject);
            if (orbsCollected >= maxOrbs)
            {
                Debug.Log("MAX ORBS");
                orbsCollected = 0;
                playerStats.maxMoves++;
                playerStats.maxMovesText.text = playerStats.maxMoves.ToString();
            }
            GameManager.instance.orbText.text = (maxOrbs - orbsCollected).ToString();
        }
    }

    IEnumerator SetResetting()
    {
        yield return new WaitForSeconds(0.5f);
        Movement.instance.resetting = false;
    }
    
}
