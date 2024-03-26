using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int pickupID;
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
        switch(pickupID)
        {
            case 1:
                cards.SetActive(true);
                playerStats.card.SetActive(true);
                PickupManager.instance.InitializeCards(gameObject);
                break;
        }
    }


}
