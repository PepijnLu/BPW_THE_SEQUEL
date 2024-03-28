using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI tutorialText, tutorialCornerText;
    public Image minimapSquare;
    public List<Image> minimapSquares;
    public Transform minimapStart;
    [SerializeField] Vector3 minimapOffset;
    public GameObject canvas;
    public float offsetFloat;
    public float sizeFloat;
    public Sprite roomImg, hallwayImg, minimapImg;
    public int lastChangedImage;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        minimapSquares = new List<Image>(){};
    }

    public void StartShowTextRoutine(string text, float duration, TextMeshProUGUI textToWriteTo)
    {
        StartCoroutine(ShowTextForDuration(text, duration, textToWriteTo));
    }
    public IEnumerator ShowTextForDuration(string text, float duration, TextMeshProUGUI textToWriteTo)
    {
        Debug.Log("show text: " + text);
        GameManager.instance.stopped = true;
        textToWriteTo.gameObject.SetActive(true);
        textToWriteTo.text = text;
        yield return new WaitForSeconds(duration);
        textToWriteTo.text = "";
        textToWriteTo.gameObject.SetActive(false);
        tutorialCornerText.text = text;
        GameManager.instance.stopped = false;
    }

    public void GenerateMinimap()
    {
        // -7 is up
        // +7 is down
        // -1 is left
        // +1 is right
        foreach (Image img in minimapSquares)
        {
            Destroy(img.gameObject);
        }
        minimapSquares.Clear();
        float tiles = (ProcGen.instance.maxMainRooms + 1) * (ProcGen.instance.maxMainRooms + 1);
        minimapOffset = minimapStart.position;
        bool firstRow = true;
        int timesOffset = 0;
        int timesWentDown = 0;
        for (int i = 1; i <= tiles; i++)
        {
            if (firstRow)
            {
                if ((IsDivisible(i, ProcGen.instance.maxMainRooms + 2)) && timesOffset != 0)
                {
                    Debug.Log("first go down at: " + i);
                    minimapOffset += new Vector3(-((timesOffset - 1) * (offsetFloat / tiles)), -(offsetFloat / tiles), 0);
                    firstRow = false;
                    timesWentDown++;
                    timesOffset = 0;
                }
                else
                {
                    minimapOffset += new Vector3(offsetFloat / tiles, 0, 0);
                    timesOffset++;
                }
            }
            else
            {
                if (IsDivisible(i + timesWentDown, ProcGen.instance.maxMainRooms + 2) && timesOffset != 0)
                {
                    Debug.Log("go down at: " + timesWentDown);
                    minimapOffset += new Vector3(-((timesOffset) * (offsetFloat / tiles)), -(offsetFloat / tiles), 0);
                    timesOffset = 0;
                    timesWentDown++;
                }
                else
                {
                    minimapOffset += new Vector3(offsetFloat / tiles, 0, 0);
                    timesOffset++;
                }
            }
            GameObject newSquare = Instantiate(minimapSquare.gameObject, minimapOffset, Quaternion.identity);
            newSquare.transform.SetParent(canvas.transform);
            float size = sizeFloat / tiles;
            newSquare.transform.localScale = new Vector3(size, size, size);
            minimapSquares.Add(newSquare.GetComponent<Image>());
        }
    }

    public void ChangeImageSprite(int direction, Sprite sprite)
    {
        switch(direction)
        {
            case 0:
                lastChangedImage = ((ProcGen.instance.maxMainRooms + 1) * (ProcGen.instance.maxMainRooms + 1)) - (ProcGen.instance.maxMainRooms + 1);
                break;
            case 1:
                lastChangedImage -= 7;
                Debug.Log("minimap up");
                break;
            case 2:
                lastChangedImage += 7;
                Debug.Log("minimap down");
                break;
            case 3:
                lastChangedImage -= 1;
                Debug.Log("minimap left");
                break;
            case 4:
                lastChangedImage += 1;
                Debug.Log("minimap right");
                break;
        }
        Debug.Log("last image " + lastChangedImage);
        if (minimapSquares.Count < lastChangedImage)
        {
            minimapSquares[lastChangedImage].sprite = sprite;
        }
    }

    public void FillMinimapBackground()
    {
        foreach (Image img in minimapSquares)
        {
            if (img.sprite != roomImg && img.sprite != hallwayImg)
            {
                img.sprite = minimapImg;
            }
        }
    }
    bool IsDivisible(int number, int divisor)
    {
        return number % divisor == 0;
    }
}
