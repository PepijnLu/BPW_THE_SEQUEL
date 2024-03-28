using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public static ChestManager instance;
    public List<Transform> threeCardsTransforms, fiveCardTransforms, nineCardTransforms;
    public float cardSize3, cardSize5, cardSize9;
    public GameObject card;
    public bool openingChest;

    void Awake()
    {
        instance = this;
    }

    public void Open()
    {
        openingChest = true;
        GameManager.instance.stopped = true;
        int randomInt = Random.Range(1, 11);
        if (randomInt < 6) {    GenerateCards(threeCardsTransforms, cardSize3);   }
        else if (randomInt < 10) {  GenerateCards(fiveCardTransforms, cardSize5);   }
        else if (randomInt == 10) { GenerateCards(nineCardTransforms, cardSize9);  }
    }

    void GenerateCards(List<Transform> cardTransformList, float cardSize)
    {
        GameManager.instance.playerStats.card.SetActive(true);
        GameManager.instance.stopped = true;
        foreach (Transform cardTransform in cardTransformList)
        {
            GameObject newCard = Instantiate(card, cardTransform.position, Quaternion.identity);
            newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
            newCard.transform.SetParent(UIManager.instance.canvas.transform);
            newCard.GetComponent<Card>().Initialize(true);
        }
        openingChest = false;
    }

    public void CheckCards()
    {
        GameObject[] chestSpawnedCards = GameObject.FindGameObjectsWithTag("ChestCard");
        if (chestSpawnedCards.Length == 1)
        {
            StartCoroutine(Close());
        }
    }

    IEnumerator Close()
    {
        yield return new WaitForSeconds(1);
        GameManager.instance.stopped = false;
        GameManager.instance.playerStats.card.SetActive(false);
        
        foreach(Stats stats in BattleManager.instance.needToFinish)
        {
            Movement.instance.EndMove(stats);
        }
    }
}
