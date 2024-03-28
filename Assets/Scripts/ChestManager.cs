using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    public static ChestManager instance;
    public List<Transform> threeCardsTransforms, fiveCardTransforms, nineCardTransforms;
    public float cardSize3, cardSize5, cardSize9;
    public GameObject card;
    public bool openingChest;
    public Sprite icon3, icon5, icon9, card3, card5, card9;
    public Sprite currentIcon, currentCard;

    void Awake()
    {
        instance = this;
    }

    public void Open()
    {
        openingChest = true;
        GameManager.instance.stopped = true;
        int randomInt = Random.Range(1, 11);
        if (randomInt < 6) {    GenerateCards(threeCardsTransforms, cardSize3, card3, icon3);   }
        else if (randomInt < 10) {  GenerateCards(fiveCardTransforms, cardSize5, card5, icon5);   }
        else if (randomInt == 10) { GenerateCards(nineCardTransforms, cardSize9, card9, icon9);  }
    }

    void GenerateCards(List<Transform> cardTransformList, float cardSize, Sprite cardSprite, Sprite iconSprite)
    {
        currentCard = cardSprite;
        currentIcon = iconSprite;
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
        Vector3Int cellPosition = ProcGen.instance.chestMap.WorldToCell(GameManager.instance.player.transform.position);
        ProcGen.instance.chestMap.SetTile(cellPosition, null);
        foreach(Stats stats in BattleManager.instance.needToFinish)
        {
            StartCoroutine(Movement.instance.EndMove(stats));
        }
    }
}
