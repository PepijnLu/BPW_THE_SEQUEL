using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject cards;
    //1 = cards
    void Start()
    {
        cards = GameManager.instance.cards;
    }
    public void Activate(GameObject player)
    {
        GameManager.instance.stopped = true;
        Stats playerStats = player.GetComponent<Stats>();
        cards.SetActive(true);
        playerStats.card.SetActive(true);
        PickupManager.instance.InitializeCards(gameObject);
    }
}
