using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public TextMeshProUGUI damageText, healthText;
    int damage, health;
    public Image cardImage, iconImage;

    void Start()
    {
        Debug.Log("card void start");
    }

    public void Initialize(bool fromChest)
    {
        Debug.Log("card void init1");
        if (fromChest)
        {
            Debug.Log("card void init2");
            damage = Random.Range(1, GameData.roomsCleared);
            health = Random.Range(1, GameData.roomsCleared);
            damageText.text = ("+" + damage.ToString());
            healthText.text = ("+" + health.ToString());

            cardImage.sprite = ChestManager.instance.currentCard;
            iconImage.sprite = ChestManager.instance.currentIcon;
        }
        else
        {

        }
    }

    public void WhenClicked()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.audioSources["pickCardSFX"]);
        GameManager.instance.playerStats.health += health;
        GameManager.instance.playerStats.hpText.text = GameManager.instance.playerStats.health.ToString();
        GameManager.instance.playerStats.maxHealth += health;
        GameManager.instance.playerStats.damage += damage;
        GameManager.instance.playerStats.damageText.text = GameManager.instance.playerStats.damage.ToString();
        GameManager.instance.playerStats.hpText.text = GameManager.instance.playerStats.health.ToString();
        ChestManager.instance.CheckCards();
        Destroy(gameObject);
    }
    
    
}
