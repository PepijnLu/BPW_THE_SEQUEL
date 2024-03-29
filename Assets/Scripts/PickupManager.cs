using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupManager : MonoBehaviour
{
    public GameObject cards;
    public GameObject playerCard;
    GameObject currentCards;
    public static PickupManager instance;
    public TextMeshProUGUI playerHP, playerDamage;
    public Stats playerStats;
    public bool selecting;

    int greenCardHealth, greenCardDamage;
    int blueCardHealth, blueCardDamage;
    int redCardHealth, redCardDamage;

    public TextMeshProUGUI greenCardHealthText, greenCardDamageText;
    public TextMeshProUGUI blueCardHealthText, blueCardDamageText;
    public TextMeshProUGUI redCardHealthText, redCardDamageText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerHP = GameManager.instance.playerStats.hpText;
        playerDamage = GameManager.instance.playerStats.damageText;
        
    }

    public void InitializeCards(GameObject cardObject)
    {
        Debug.Log("roomsCleared = " + GameData.roomsCleared);
        selecting = true;
        currentCards = cardObject;
        playerCard = GameManager.instance.playerStats.card;
    
        greenCardHealth = Random.Range(1, GameData.roomsCleared);
        greenCardDamage = 0;
        blueCardHealth = 0;
        blueCardDamage = 0;
        redCardHealth = 0;
        redCardDamage = Random.Range(1, GameData.roomsCleared);

        greenCardHealthText.text = ("+" + greenCardHealth.ToString());
        greenCardDamageText.text = ("+" + greenCardDamage.ToString());
        redCardHealthText.text = ("+" + redCardHealth.ToString());
        redCardDamageText.text = ("+" + redCardDamage.ToString());
    }

    public void GreenCard()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.audioSources["pickCardSFX"]);
        playerStats.health += greenCardHealth;
        playerHP.text = playerStats.health.ToString();
        playerStats.maxHealth += greenCardHealth;
        playerStats.damage += greenCardDamage;
        playerDamage.text = playerStats.damage.ToString();
        cards.SetActive(false);
        StartCoroutine(ClosePlayerCard());
        Destroy(currentCards);
    }
    public void BlueCard()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.audioSources["pickCardSFX"]);
        playerStats.health = playerStats.maxHealth;
        playerHP.text = playerStats.health.ToString();
        cards.SetActive(false);
        StartCoroutine(ClosePlayerCard());
        Destroy(currentCards);
    }
    public void RedCard()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.audioSources["pickCardSFX"]);
        playerHP.text = (playerStats.health + redCardHealth).ToString();
        playerStats.health += redCardHealth;
        playerStats.maxHealth += redCardHealth;
        playerDamage.text = (playerStats.damage + redCardDamage).ToString();
        playerStats.damage += redCardDamage;
        cards.SetActive(false);
        StartCoroutine(ClosePlayerCard());
        Destroy(currentCards);
    }

    public void StartHoverGreen()
    {
        playerHP.text = (playerStats.health + greenCardHealth).ToString();
    }
    public void StopHoverGreen()
    {
        playerHP.text = playerStats.health.ToString();
    }
    public void StartHoverBlue()
    {
        playerHP.text = playerStats.maxHealth.ToString();
    }
    public void StopHoverBlue()
    {
        playerHP.text = playerStats.health.ToString();
    }
    public void StartHoverRed()
    {
        playerDamage.text = (playerStats.damage + redCardDamage).ToString();
    }
    public void StopHoverRed()
    {
        playerDamage.text = playerStats.damage.ToString();
    }

    IEnumerator ClosePlayerCard()
    {
        yield return new WaitForSeconds(1);
        playerCard.SetActive(false);
        GameManager.instance.stopped = false;
        selecting = false;
        //playerHP.fontSize = (1 / playerHP.text.Length) * playerHP.fontSize;
        //playerDamage.fontSize = (1 / playerHP.text.Length) * playerHP.fontSize;
        StartCoroutine(Movement.instance.EndMove(GameManager.instance.playerStats));
        foreach(Stats stats in BattleManager.instance.needToFinish)
        {
            StartCoroutine(Movement.instance.EndMove(stats));
        }
        
        // if (playerStats.moves < playerStats.maxMoves)
        // {
        //     //playerStats.gameObject.GetComponent<PlayerController>().CheckForPossibleMovement();
        // }
        // else
        // {
        //     TurnManager.instance.CheckActions(GameManager.instance.player);
        // }
    }

}
