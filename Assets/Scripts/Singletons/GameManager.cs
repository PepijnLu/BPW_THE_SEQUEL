using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{ 
    public static GameManager instance;
    public bool stopped;
    public List<GameObject> enemies;
    public GameObject cards, player, settingsMenu;
    bool paused;
    public Stats playerStats;
    public Sprite arrow;
    public TextMeshProUGUI orbText;
    public bool generatingDungeon;
    void Awake()
    {
        instance = this;
        enemies = new List<GameObject>();
    }

    void Update()
    {
        //Open settings menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                paused = true;
                Time.timeScale = 0f;
                settingsMenu.SetActive(true);
            }
            else
            {
                paused = false;
                Time.timeScale = 1f;
                settingsMenu.SetActive(false);
            }
        }
    }
    public void EnemyDeath(int mainRoomInt, int extraRoomInt)
    {
        //Make sure the room opens when all enemies are dead
        if (!generatingDungeon)
        {
            if (mainRoomInt != 0)
            {
                ProcGen.instance.enemiesPerRoom["enemiesMainRoom" + mainRoomInt.ToString()]--;
                if (ProcGen.instance.enemiesPerRoom["enemiesMainRoom" + mainRoomInt.ToString()] == 0)
                {
                    if (mainRoomInt < ProcGen.instance.maxMainRooms)
                    {
                        Debug.Log("All enemies dead main room " + mainRoomInt);
                        OpenRoom(mainRoomInt);
                    }
                    else
                    {
                        OpenExit(mainRoomInt);
                    }
                }
            }
            else
            {
                ProcGen.instance.enemiesPerRoom["enemiesExtraRoom" + extraRoomInt.ToString()]--;
                if (ProcGen.instance.enemiesPerRoom["enemiesExtraRoom" + extraRoomInt.ToString()] == 0)
                {
                    Debug.Log("All enemies dead extra room " + extraRoomInt);
                    OpenChest(extraRoomInt);
                }
            }
        }
    }
    public void OpenChest(int room)
    {
        //OPEN THE CHEST
        int xPos = Mathf.RoundToInt(ProcGen.instance.extraRoomLocations["extraRoomLocation" + room.ToString()].x);
        int yPos = Mathf.RoundToInt(ProcGen.instance.extraRoomLocations["extraRoomLocation" + room.ToString()].y);
        ProcGen.instance.collisionMap.SetTile(new Vector3Int(xPos, yPos, 0), null);
        ProcGen.instance.decorationMap.SetTile(new Vector3Int(xPos, yPos, 0), null);
        ProcGen.instance.chestMap.SetTile(new Vector3Int(xPos, yPos, 0), ProcGen.instance.chest);
        AudioManager.instance.PlaySound(AudioManager.instance.audioSources["openRoomSFX"]);
    }

    public void OpenExit(int room)
    {
        Vector3Int exitLocation = ProcGen.instance.collisionMap.WorldToCell(ProcGen.instance.mainRoomLocations["mainRoomLocation" + room.ToString()]);
        ProcGen.instance.collisionMap.SetTile(exitLocation, null);
        ProcGen.instance.decorationMap.SetTile(exitLocation, null);
        ProcGen.instance.exitMap.SetTile(exitLocation, ProcGen.instance.exit);
        AudioManager.instance.PlaySound(AudioManager.instance.audioSources["openRoomSFX"]);
    }

    public void OpenRoom(int room)
    {
        Debug.Log("Open room " + room);
        float xPos = ProcGen.instance.mainRoomLocations["mainRoomLocation" + room.ToString()].x;
        float yPos = ProcGen.instance.mainRoomLocations["mainRoomLocation" + room.ToString()].y;
        float direction = ProcGen.instance.mainRoomLocations["mainRoomLocation" + room.ToString()].z;
        float length = ProcGen.instance.mainRoomLengthWidthSize["mainRoomLWS" + room.ToString()].x;
        float width = ProcGen.instance.mainRoomLengthWidthSize["mainRoomLWS" + room.ToString()].y;
        float roomSize = ProcGen.instance.mainRoomLengthWidthSize["mainRoomLWS" + room.ToString()].z;
        AudioManager.instance.PlaySound(AudioManager.instance.audioSources["openRoomSFX"]);
        GameData.roomsCleared++;

        if (direction == 1)
        {
            for (int x = 0; x < 500; x++)
            {
                for (int y = 0; y < 500; y++)
                {
                    if ( (y >= yPos + roomSize - width) && (y <= yPos + length + width))
                    {   

                        if ( (x > xPos - width) && (x < xPos + width) )
                        {
                            if (y == yPos + roomSize)
                            {
                                Debug.Log("Clear Tile: " + x + "," + y);
                                ProcGen.instance.collisionMap.SetTile(new Vector3Int(x, y, 0), null);
                                ProcGen.instance.decorationMap.SetTile(new Vector3Int(x, y, 0), null);
                                ProcGen.instance.tilemap.SetTile(new Vector3Int(x, y, 0), ProcGen.instance.tile);
                            }
                        }
                    }
                }
            }
        }
        if (direction == 2)
        {
            for (int x = 0; x < 500; x++)
            {
                for (int y = 0; y < 500; y++)
                {
                    if ( (x >= xPos + roomSize - width) && (x <= xPos + length + width))
                    {  
                        if ( (y > yPos - width) && (y < yPos + width) )
                        {
                            if (x == xPos + roomSize)
                            {   
                                Debug.Log("Clear Tile: " + x + "," + y);
                                ProcGen.instance.collisionMap.SetTile(new Vector3Int(x, y, 0), null);
                                ProcGen.instance.decorationMap.SetTile(new Vector3Int(x, y, 0), null);
                                ProcGen.instance.tilemap.SetTile(new Vector3Int(x, y, 0), ProcGen.instance.tile);
                            }
                        }
                    }
                }
            }
        }
    }
}
