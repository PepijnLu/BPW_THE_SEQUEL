using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public TextMeshProUGUI damageText, healthText, wordText;
    int damage, health;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (ChestManager.instance.openingChest)
        {
            damage = Random.Range(1, GameData.roomsCleared);
            health = Random.Range(1, GameData.roomsCleared);
            damageText.text = ("+" + damage.ToString());
            healthText.text = ("+" + health.ToString());
        }
    }

    public void WhenClicked()
    {

    }
    
}
