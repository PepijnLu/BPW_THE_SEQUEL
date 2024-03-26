using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stats : MonoBehaviour
{
    public int health, maxHealth, damage, maxMoves;
    public int moves, enemyInt;
    public GameObject cardPrefab;
    public GameObject card;
    public TextMeshProUGUI hpText, damageText, nameText, maxMovesText;
    public GameObject icon;
    public Sprite attack, defend, attacker, defender;
    public Image sword;
    public Transform cardSlot;
    public bool turnDone, fired;
    public bool inBattle;
    public bool firstStrike;
    public bool inList;
    public int mainRoomInt, extraRoomInt;
    public bool moveStarted, collisionCheck;
    public int powerLevel;
    public bool attackingFirst;

    void Awake()
    {

        if (gameObject.tag == "Player")
        {
            card = Instantiate(cardPrefab, BattleManager.instance.playerCardSlot.position, Quaternion.identity);
            card.transform.SetParent(GameObject.Find("Canvas").transform);
            card.transform.position = BattleManager.instance.playerCardSlot.position;
            card.SetActive(false);
        }
        if (gameObject.tag == "Enemy")
        {
            card = Instantiate(cardPrefab, BattleManager.instance.enemyCardSlot.position, Quaternion.identity);
            card.transform.SetParent(GameObject.Find("Canvas").transform);
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
    }


    public void SetValues()
    {
        if (gameObject.tag == "Enemy")
        {
            if (mainRoomInt != 0)
            {
                powerLevel = Mathf.FloorToInt(((mainRoomInt) * 1.5f) + ((GameData.dungeonsCompleted * ProcGen.instance.maxMainRooms) * 1.5f));
            }
            else
            {
                powerLevel = Mathf.FloorToInt(((ProcGen.instance.maxExtraRooms + 1 - extraRoomInt) * 1.5f) + ((GameData.dungeonsCompleted * ProcGen.instance.maxMainRooms) * 1.5f));
            }
            maxHealth = Random.Range(1, powerLevel + 1);
            damage = Random.Range(1, powerLevel + 1);
            if (damage > health)
            {
                firstStrike = true;
                icon.GetComponent<Image>().sprite = attacker;
                gameObject.GetComponent<SpriteRenderer>().sprite = attacker;
                maxMoves = 2;
            }
            else
            {
                firstStrike = false;
                icon.GetComponent<Image>().sprite = defender;
                gameObject.GetComponent<SpriteRenderer>().sprite = defender;
            }
            
        }
        
        health = maxHealth;
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
