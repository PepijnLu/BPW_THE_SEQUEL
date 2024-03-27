using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public static ChestManager instance;
    public List<Transform> threeCardsTransforms, fiveCardTransforms, tenCardTransforms;
    public float cardSize3, cardSize5, cardSize10;
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
        else if (randomInt == 10) { GenerateCards(tenCardTransforms, cardSize10);  }
    }

    void GenerateCards(List<Transform> cardTransformList, float cardSize)
    {
        foreach (Transform cardTransform in cardTransformList)
        {
            GameObject newCard = Instantiate(card, cardTransform.position, Quaternion.identity);
            newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        }
        openingChest = false;
    }
}
