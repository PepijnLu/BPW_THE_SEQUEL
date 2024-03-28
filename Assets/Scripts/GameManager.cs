using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{ 
    /*
        TO DO:

        fix:  
        -no enemies turn doesnt pass
        -laser enemies sometimes dont fire

        add:
        -MAKE HALLWAYS INTERESTING

        -TUTORIAL:
            -explain ranged enemies

            -explain opening exits

        IMPROVE:
        -CODE   
            -cards dropped by enemies
            -procedural generation        
        -difficulty scaling/balancing


        probably fixed:
        -something sometimes breaks the turns
        -lock the exit (EASY FIX)
        -4: fix cards and battles happening at the same time (might be a non issue)
        -2: fix broken enemy movement (has a bandaid fix)
        -1: make it so that multiple enemies can not be on the same square
        -enemies spawning in you (easy fix)
        -multiple move enemies will break when they cant move in every direction (think its fixed?)
        -enemies can theoretically get completely stuck in blocks (uhhhhh)
        -combat with multiple enemies at once breaks   
    */
    public static GameManager instance;
    public bool stopped;
    public GameObject cards;
    public List<GameObject> enemies;
    public GameObject player, settingsMenu;
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
        // int xPos = Mathf.RoundToInt(ProcGen.instance.mainRoomLocations["mainRoomLocation" + room.ToString()].x);
        // int yPos = Mathf.RoundToInt(ProcGen.instance.mainRoomLocations["mainRoomLocation" + room.ToString()].y);
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
