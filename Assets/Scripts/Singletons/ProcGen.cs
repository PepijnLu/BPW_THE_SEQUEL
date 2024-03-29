using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProcGen : MonoBehaviour
{
    public PlayerController playerController;
    public static ProcGen instance;
    public Tilemap tilemap, collisionMap, exitMap, chestMap, decorationMap, rubbleMap;
    public Tilemap tutorialGround, tutorialCollision, tutorialChest, tutorialExit;
    public List<Tilemap> normalTilemaps, tutorialTilemaps;
    public Tile tile, wall, exit, block, chest, lockTile, doorTile;
    public List<Tile> rubbleTiles;
    public GameObject enemyPrefab, orbPrefab;
    private int[,] map;
    int x, y;
    //NOTE: EXTRA ROOMS ARE GENERATED BACK TO FRONT. LAST EXTRA ROOM IS EXTRA ROOM 1
    [SerializeField] int mainRoomAmount, extraRoomAmount, hallwayAmount;
    public int maximumMainRooms, maximumExtraRooms;
    [HideInInspector] public int maxMainRooms, maxExtraRooms;
    public int minRoomSize, maxRoomSize, minHallwayLength, maxHallwayLength, minHallwayWidth, maxHallwayWidth, enemySpawnChance, blockSpawnChance, orbSpawnChance;
    int previousX, previousY, previousSize;
    bool occupied, clearedRooms;
    public GameObject[] cards;
    public Dictionary<string, int> enemiesPerRoom = new Dictionary<string, int>();
    public Dictionary<string, Vector3> mainRoomLocations = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector3> mainRoomLengthWidthSize = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector3> extraRoomLocations = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector3> extraRoomLengthWidthSize = new Dictionary<string, Vector3>();

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        normalTilemaps = new List<Tilemap>(){tilemap, collisionMap, exitMap, chestMap, decorationMap, rubbleMap};
        tutorialTilemaps = new List<Tilemap>(){tutorialGround, tutorialCollision, tutorialChest, tutorialExit};
        //GetRoomNumbers();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyUp(KeyCode.L))
        // {
        //     GenerateDungeon();
        // }
    }

    public void GenerateDungeon()
    {
        maxMainRooms = Random.Range(3, maximumMainRooms);
        maxExtraRooms = Random.Range(3, maximumExtraRooms);
        Debug.Log("maxMainRooms: " + maxMainRooms + "maxExtraRooms " + maxExtraRooms);
        TurnManager.instance.isPlayerTurn = true;
        playerController.turnStarted = false;
        GameManager.instance.playerStats.moves = 0;
        GameManager.instance.playerStats.turnDone = false;
        GameManager.instance.generatingDungeon = true;
        previousSize = 0;
        previousX = 0;
        previousY = 0;
        mainRoomAmount = 0;
        extraRoomAmount = 0;
        hallwayAmount = 0;
        enemiesPerRoom.Clear();
        mainRoomLocations.Clear();
        mainRoomLengthWidthSize.Clear();
        extraRoomLocations.Clear();
        extraRoomLengthWidthSize.Clear();
        foreach (Tilemap tmp in normalTilemaps)
        {
            tmp.ClearAllTiles();
        }
        clearedRooms = false;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
        foreach (GameObject pickup in pickups)
        {
            Destroy(pickup);
        }
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        foreach (GameObject orb in orbs)
        {
            Destroy(orb);
        }
        GetRoomNumbers();
        playerController.gameObject.transform.position = new Vector3(50.5f, 50.5f, 0);
        playerController.NukeDirections();
        playerController.CheckForPossibleMovement();
        cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in cards)
        {
            card.SetActive(false);
        }
        OpenEmptyRooms();
        GameManager.instance.generatingDungeon = false;
    }
    void GetRoomNumbers()
    {
        int size = Random.Range(minRoomSize, maxRoomSize);
        int xLocation = 50;
        int yLocation = 50;

        float distanceBetweenCircles = Vector2.Distance(new Vector2(xLocation, yLocation), new Vector2(previousX, previousY));

        float sumOfCircles = size + previousSize;

        if (distanceBetweenCircles <= sumOfCircles)
        {
            GetRoomNumbers();
        }
        else
        {
            previousSize = size;
            previousX = xLocation;
            previousY = yLocation;

            GenerateRoom(xLocation, yLocation, size, true, 0);
        }
    }

    public void OpenEmptyRooms()
    {
        foreach (KeyValuePair<string, int> pair in enemiesPerRoom)
        {
            int enemies = pair.Value;
            if ( (enemies <= 0) && (pair.Key.Contains("MainRoom")))
            {
                string numericPart = pair.Key.Substring("enemiesMainRoom".Length);
                if (int.TryParse(numericPart, out int roomNumber))
                {
                    if (roomNumber == maxMainRooms)
                    {
                        GameManager.instance.OpenExit(roomNumber);
                    }
                    else
                    {   
                        GameManager.instance.OpenRoom(roomNumber);
                    }
                }
            }
        }
    }
    
    public void GenerateRoom(int xLoc, int yLoc, int size, bool makeExtra, int prevDir)
    {
        int enemiesInRoom = 0;
        
        if (makeExtra == false)
        {
            extraRoomAmount++;
        }
        else
        {   
            mainRoomAmount++;
        }

        for (int x = xLoc - size; x < 500; x++)
        {
            for (int y = yLoc - size; y < 500; y++)
            {
                //Bool for if a tile already has something on it
                occupied = false;
                //How far away the current tile is from the center of the room
                Vector2 difference = new Vector2(x, y) - new Vector2(xLoc, yLoc);
                //Place a wall tile if the distance to the room is equal to the size
                if ( (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y) ) ) ) == size)
                {   
                    collisionMap.SetTile(new Vector3Int(x, y, 0), wall);
                    occupied = true;
                } 

                //Check if you're currently inside the room
                if ( (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y) ) ) ) < size)
                {   
                    //Set a ground tile
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);

                    //Generate a random integer to decide whether or not to spawn an obstacle
                    int randomizer = Random.Range(1, blockSpawnChance);
                    if ( (randomizer == 1) && (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y)) ) > 2 ) && ((x != xLoc) && (y != yLoc)) )
                    {
                        int randomRubble = Random.Range(0, rubbleTiles.Count);
                        rubbleMap.SetTile(new Vector3Int(x, y, 0), rubbleTiles[randomRubble]);
                        occupied = true;
                    }

                    //Generate a random integer to decide whether or not to spawn an enemy
                    int enemyRandom = Random.Range(1, enemySpawnChance);
                    //Make sure no enemies spawn near walls
                    if ( (!occupied) && (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y))) > 2 ))
                    {
                        if (enemyRandom == 1)
                        {
                            //Spawn the enemy
                            GameObject spawnedEnemy = Instantiate(enemyPrefab, new Vector2(x + 0.5f, y + 0.5f), transform.rotation);
                            Stats spawnedEnemyStats = spawnedEnemy.GetComponent<Stats>();
                            occupied = true;
                            enemiesInRoom++;
                            //Let the enemy know it's in a main room
                            if (makeExtra)
                            {
                                spawnedEnemyStats.mainRoomInt = mainRoomAmount;
                            }
                            //Let the enemy know it's in an extra room
                            else
                            {
                                spawnedEnemyStats.extraRoomInt = extraRoomAmount;
                            }
                            //Set the values of the spawned enemy (damage, health, sprites etc.)
                            spawnedEnemyStats.SetValues();
                        }
                    }

                    //Generate a random integer to decide whether or not to spawn an orb pickup
                    if (!occupied)
                    {
                        int orbRandom = Random.Range(1, orbSpawnChance);
                        if (orbRandom == 1)
                        {
                            Instantiate(orbPrefab, new Vector2(x + 0.5f, y + 0.5f), transform.rotation);
                        }
                    }
                }   
            }
        }

        int direction = Random.Range(1, 3);
        int newSize = Random.Range(minRoomSize, maxRoomSize);
        int length = Random.Range(minHallwayLength, maxHallwayLength) + newSize + size;
        int width = Random.Range(minHallwayWidth, maxHallwayWidth);

        if (makeExtra)
        {
            enemiesPerRoom.Add(("enemiesMainRoom" + mainRoomAmount.ToString()), enemiesInRoom);

            mainRoomLocations.Add(("mainRoomLocation" + mainRoomAmount.ToString()), new Vector3(xLoc, yLoc, direction));
            mainRoomLengthWidthSize.Add(("mainRoomLWS" + mainRoomAmount.ToString()), new Vector3(length, width, size));
            Debug.Log("mainRoomLocation" + mainRoomAmount.ToString());

            if (mainRoomAmount == maxMainRooms)
            {
                decorationMap.SetTile(new Vector3Int(xLoc, yLoc, 0), lockTile);
                collisionMap.SetTile(new Vector3Int(xLoc, yLoc, 0), exit);
            }
            
        }
        else
        {   
            enemiesPerRoom.Add(("enemiesExtraRoom" + extraRoomAmount.ToString()), enemiesInRoom);
            extraRoomLocations.Add(("extraRoomLocation" + extraRoomAmount.ToString()), new Vector3(xLoc, yLoc));
            extraRoomLengthWidthSize.Add(("extraRoomLWS" + extraRoomAmount.ToString()), new Vector3(length, width, size));

            //LOCK THE CHEST
            decorationMap.SetTile(new Vector3Int(xLoc, yLoc, 0), lockTile);
            collisionMap.SetTile(new Vector3Int(xLoc, yLoc, 0), chest);

            if (enemiesPerRoom["enemiesExtraRoom" + extraRoomAmount.ToString()] <= 0)
            {
                GameManager.instance.OpenChest(extraRoomAmount);
            }
        }

        if ( (mainRoomAmount + 1 <= maxMainRooms) )
        {
            GenerateHallway(direction, width, xLoc, yLoc, size, makeExtra, prevDir, newSize, length);
        }
    }
        

    public void GenerateHallway(int direction, int width, int xPos, int yPos, int roomSize, bool makeExtra, int prevDir, int newSize, int length)
    {
        //Random integer to decide whether or not to spawn an extra room
        int extraRoom = Random.Range(1, 3);
            switch(direction)
            {
                //Generate main room up
                case 1:
                    hallwayAmount++;
                    GenerateRoom(xPos, yPos + length + newSize, newSize, true, direction);
                    for (int x = 0; x < 500; x++)
                    {
                        for (int y = 0; y < 500; y++)
                        {
                            if ( (y >= yPos + roomSize - width) && (y <= yPos + length + width))
                            {
                                if ( (x == xPos - width) || (x == xPos + width) )
                                {
                                    collisionMap.SetTile(new Vector3Int(x, y, 0), wall);
                                }   

                                if ( (x > xPos - width) && (x < xPos + width) )
                                {
                                    if (y == yPos + roomSize)
                                    {
                                        collisionMap.SetTile(new Vector3Int(x, y, 0), doorTile);
                                        decorationMap.SetTile(new Vector3Int(x, y, 0), lockTile);
                                    }
                                    else
                                    {
                                        collisionMap.SetTile(new Vector3Int(x, y, 0), null);
                                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                                    }
                                }
                            }
                        }
                    }
                    break;

                //Generate main room right
                case 2:
                hallwayAmount++;
                    GenerateRoom(xPos + length + newSize, yPos, newSize, true, direction);

                    for (int x = 0; x < 500; x++)
                    {
                        for (int y = 0; y < 500; y++)
                        {
                            if ( (x >= xPos + roomSize - width) && (x <= xPos + length + width))
                            {
                                if ( (y == yPos - width) || (y == yPos + width) )
                                {
                                    collisionMap.SetTile(new Vector3Int(x, y, 0), wall);
                                }   

                                if ( (y > yPos - width) && (y < yPos + width) )
                                {
                                    if (x == xPos + roomSize)
                                    {   
                                        collisionMap.SetTile(new Vector3Int(x, y, 0), doorTile);
                                        decorationMap.SetTile(new Vector3Int(x, y, 0), lockTile);
                                    }
                                    else
                                    {
                                        collisionMap.SetTile(new Vector3Int(x, y, 0), null);
                                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        

        if ((makeExtra == true) && (extraRoomAmount + 1 <= maxExtraRooms))
        {
            Debug.Log(prevDir);
            switch(prevDir)
            {
                case 1:
                    hallwayAmount++;
                    //extra room left
                    if ((extraRoom == 1))
                    {
                        GenerateRoom(xPos - length - newSize, yPos, newSize, false, 4);
                        for (int x = 0; x < 500; x++)
                        {
                            for (int y = 0; y < 500; y++)
                            {
                                if ( (x <= xPos - roomSize + width) && (x >= xPos - length - width))
                                {
                                    if ( (y == yPos - width) || (y == yPos + width) )
                                    {
                                        collisionMap.SetTile(new Vector3Int(x, y, 0), wall);
                                    }   

                                    if ( (y > yPos - width) && (y < yPos + width) )
                                    {
                                        collisionMap.SetTile(new Vector3Int(x, y, 0), null);
                                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 2:
                    hallwayAmount++;
                    //extra room down
                    if ((extraRoom == 1))
                    {
                        GenerateRoom(xPos, yPos - length - newSize, newSize, false, 3);
                        for (int x = 0; x < 500; x++)
                        {
                            for (int y = 0; y < 500; y++)
                            {
                                if ( (y <= yPos - roomSize + width) && (y >= yPos - length - width))
                                {
                                    if ( (x == xPos - width) || (x == xPos + width) )
                                    {
                                        collisionMap.SetTile(new Vector3Int(x, y, 0), wall);
                                    }   

                                    if ( (x > xPos - width) && (x < xPos + width) )
                                    {
                                        collisionMap.SetTile(new Vector3Int(x, y, 0), null);
                                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                                    }
                                }
                            }
                        }
                    }
                    break;
            } 
        }
    }
}
