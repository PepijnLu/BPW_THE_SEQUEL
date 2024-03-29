using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Stats : MonoBehaviour
{
    public int health, maxHealth, damage, maxMoves;
    public int moves, enemyInt;
    public GameObject cardPrefab;
    public GameObject card;
    public TextMeshProUGUI hpText, damageText, nameText, maxMovesText;
    public GameObject icon;
    public Sprite attack, defend, attacker, defender, laserBoi, snail, nosferatu, slime;
    public Image sword;
    public Transform cardSlot;
    public bool turnDone, fired;
    public bool inBattle;
    public bool firstStrike;
    public bool inList;
    public bool doneMoving, tryingToEnd;
    public int mainRoomInt, extraRoomInt;
    public bool moveStarted, collisionCheck;
    public int powerLevel;
    public bool attackingFirst;

    void Awake()
    {

        if (gameObject.tag == "Player")
        {
            card = Instantiate(cardPrefab, BattleManager.instance.playerCardSlot.position, Quaternion.identity);
            card.transform.SetParent(GameObject.Find("Cards").transform);
            card.transform.position = BattleManager.instance.playerCardSlot.position;
            card.SetActive(false);
        }
        if (gameObject.tag == "Enemy")
        {
            card = Instantiate(cardPrefab, BattleManager.instance.enemyCardSlot.position, Quaternion.identity);
            card.transform.SetParent(GameObject.Find("Cards").transform);
            card.transform.position = BattleManager.instance.enemyCardSlot.position;
            card.SetActive(false);
        }

        Transform childHpText = card.transform.Find("hpText");
        Transform childDamageText = card.transform.Find("damageText");
        Transform childNameText = card.transform.Find("nameText");
        Transform childIcon = card.transform.Find("Icon");
        Transform childMaxMovesText = card.transform.Find("MaxMovesText");

        hpText = childHpText.gameObject.GetComponent<TextMeshProUGUI>();
        damageText = childDamageText.gameObject.GetComponent<TextMeshProUGUI>();
        maxMovesText = childMaxMovesText.gameObject.GetComponent<TextMeshProUGUI>();
        //nameText = childNameText.gameObject.GetComponent<TextMeshProUGUI>();
        icon = childIcon.gameObject;
    }

    void Start()
    {
        if (gameObject.tag == "Player")
        {
            cardSlot = GameObject.Find("playerCardSlot").transform;
            sword = BattleManager.instance.playerSword;
        }
        if (gameObject.tag == "Enemy")
        {
            cardSlot = GameObject.Find("enemyCardSlot").transform;
            sword = BattleManager.instance.enemySword;
        }
        if (gameObject.tag == "Enemy")
        {
            SetValues();
        }
    }

    void FixedUpdate()
    {
        if ((gameObject.tag == "Enemy") && (!TurnManager.instance.isPlayerTurn))
        {
            if ((moves >= maxMoves) && (!GameManager.instance.stopped) && turnDone == false && doneMoving && !tryingToEnd)
            {
                tryingToEnd = true;
                StartCoroutine(Movement.instance.EndMove(this));
            }
        }
    }

    public void SetValues()
    {
        if (gameObject.tag == "Enemy")
        {
            if (Tutorial.instance != null)
            {
                switch(Tutorial.instance.tutorialEnemiesSpawned)
                {
                    case 1:
                        powerLevel = 1;
                        enemyInt = 2;
                        break;
                    case 2:
                        powerLevel = 2;
                        enemyInt = 1;
                        break;
                    case 3:
                        powerLevel = 2;
                        enemyInt = 1;
                        break;
                    case 4:
                        powerLevel = 1;
                        enemyInt = 2;
                        break;
                    case 5:
                        powerLevel = 3;
                        enemyInt = 1;
                        break;
                    case 6:
                        powerLevel = 1;
                        enemyInt = 3;
                        break;
                }

            }
            else if (mainRoomInt != 0)
            {
                powerLevel = Mathf.FloorToInt(((mainRoomInt) * 1.5f) + ((GameData.dungeonsCompleted * ProcGen.instance.maxMainRooms) * 1.5f));
            }
            else
            {
                powerLevel = Mathf.FloorToInt(((ProcGen.instance.maxExtraRooms + 1 - extraRoomInt) * 1.5f) + ((GameData.dungeonsCompleted * ProcGen.instance.maxMainRooms) * 1.5f));
            }
            maxHealth = Random.Range(1, powerLevel + 1);
            damage = Random.Range(1, powerLevel + 1);
            if (Tutorial.instance == null)
            {
                enemyInt = Random.Range(1, 7);
            }
            Image enemyIcon = icon.GetComponent<Image>();
            SpriteRenderer enemySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            switch(enemyInt)
            {
                case 1:
                    enemyIcon.sprite = attacker;
                    enemySpriteRenderer.sprite = attacker;
                    maxMoves = 2;
                    break;
                case 2:
                    enemyIcon.sprite = defender;
                    enemySpriteRenderer.sprite = defender;
                    maxMoves = 1;
                    break;
                case 3:
                    enemyIcon.sprite = laserBoi;
                    enemySpriteRenderer.sprite = laserBoi;
                    maxMoves = 1;
                    maxHealth = 1;
                    health = 1;
                    damage = 1;
                    break;
                case 4:
                    enemyIcon.sprite = slime;
                    enemySpriteRenderer.sprite = slime;
                    maxMoves = 1;
                    break;
                case 5:
                    enemyIcon.sprite = snail;
                    enemySpriteRenderer.sprite = snail;
                    maxMoves = 1;
                    break;
                case 6:
                    enemyIcon.sprite = nosferatu;
                    enemySpriteRenderer.sprite = nosferatu;
                    maxMoves = 3;
                    break;
            }
        }
        
        health = maxHealth;
        maxMoves += GameData.dungeonsCompleted;
        maxMovesText.text = maxMoves.ToString();
        hpText.text = health.ToString();
        damageText.text = damage.ToString();
    }

    // void Update()
    // {
    //     if (gameObject.tag == "Player")
    //     {
    //         Debug.Log(turnDone);
    //     }
    // }
}
