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
    [SerializeField] int mainRoomAmount, extraRoomAmount;
    public int maxMainRooms, maxExtraRooms;
    public int minRoomSize, maxRoomSize, minHallwayLength, maxHallwayLength, minHallwayWidth, maxHallwayWidth, enemySpawnChance, blockSpawnChance, orbSpawnChance;
    int previousX, previousY, previousSize;
    bool occupied, clearedRooms;
    public GameObject[] cards;
    public Dictionary<string, int> enemiesPerRoom = new Dictionary<string, int>();
    public Dictionary<string, Vector3> mainRoomLocations = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector3> mainRoomLengthWidthSize = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector2> extraRoomLocations = new Dictionary<string, Vector2>();

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
        if (Input.GetKeyUp(KeyCode.L))
        {
            GenerateDungeon();
        }
    }

    public void GenerateDungeon()
    {
        previousSize = 0;
        previousX = 0;
        previousY = 0;
        mainRoomAmount = 0;
        extraRoomAmount = 0;
        enemiesPerRoom.Clear();
        mainRoomLocations.Clear();
        mainRoomLengthWidthSize.Clear();
        extraRoomLocations.Clear();
        // collisionMap.ClearAllTiles();
        // tilemap.ClearAllTiles();
        // exitMap.ClearAllTiles();
        // decorationMap.ClearAllTiles();
        // chestMap.ClearAllTiles();
        // rubbleMap.ClearAllTiles();
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
        UIManager.instance.GenerateMinimap();
        GetRoomNumbers();
        TurnManager.instance.isPlayerTurn = true;
        playerController.turnStarted = false;

        cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in cards)
        {
            card.SetActive(false);
        }
    }
    void GetRoomNumbers()
    {
        int size = Random.Range(minRoomSize, maxRoomSize);
        int xLocation = 50;
        int yLocation = 50;

        float distanceBetweenCircles = Vector2.Distance(new Vector2(xLocation, yLocation), new Vector2(previousX, previousY));

        // Calculate the sum of the radii
        float sumOfRadii = size + previousSize;

        // Check if the circles are overlapping
        if (distanceBetweenCircles <= sumOfRadii)
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
                occupied = false;

                Vector2 difference = new Vector2(x, y) - new Vector2(xLoc, yLoc);
                if ( (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y) ) ) ) == size)
                {   
                    collisionMap.SetTile(new Vector3Int(x, y, 0), wall);
                    occupied = true;
                } 
                if ( (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y) ) ) ) < size)
                {   
                    int randomizer = Random.Range(1, blockSpawnChance);
                    if ( (randomizer == 1) && (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y)) ) > 2 ) && ((x != xLoc) && (y != yLoc)) )
                    {
                        int randomRubble = Random.Range(0, rubbleTiles.Count);
                        rubbleMap.SetTile(new Vector3Int(x, y, 0), rubbleTiles[randomRubble]);
                        occupied = true;
                    }
                } 
                // if ( (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y) ) ) ) == size)
                // {   
                // //     collisionMap.SetTile(new Vector3Int(x, y, 0), wall);
                // } 
                if ( (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y) ) ) ) < size)
                {   
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    int enemyRandom = Random.Range(1, enemySpawnChance);
                    if ( (occupied == false) && (( (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y) ) ) ) < size)) && (Mathf.RoundToInt( (Mathf.Abs(difference.x) + Mathf.Abs(difference.y)) ) > 2 ))
                    {
                        if (enemyRandom == 1)
                        {
                            GameObject spawnedEnemy = Instantiate(enemyPrefab, new Vector2(x + 0.5f, y + 0.5f), transform.rotation);
                            Stats spawnedEnemyStats = spawnedEnemy.GetComponent<Stats>();
                            occupied = true;
                            enemiesInRoom++;

                            if (makeExtra)
                            {
                                spawnedEnemyStats.mainRoomInt = mainRoomAmount;
                            }
                            else
                            {
                                spawnedEnemyStats.extraRoomInt = extraRoomAmount;
                            }
                            spawnedEnemyStats.SetValues();
                        }
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

            // if (enemiesPerRoom["enemiesMainRoom" + mainRoomAmount.ToString()] <= 0)
            // {
            //     // Debug.Log("open main room with 0 enemies");
            //     // Debug.Log("main room amount = " + mainRoomAmount);
            //     GameManager.instance.OpenRoom(mainRoomAmount);
            // }

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

        if ( (extraRoomAmount == maxExtraRooms) && (!clearedRooms))
        {
            clearedRooms = true;
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
    
    }
        

    public void GenerateHallway(int direction, int width, int xPos, int yPos, int roomSize, bool makeExtra, int prevDir, int newSize, int length)
    {
        //int newSize = Random.Range(minRoomSize, maxRoomSize);
        //int length = oldLength + newSize + roomSize;
        //int extraRoom = Random.Range(1, 4);
        int extraRoom = 1;
            switch(direction)
            {
                //up
                case 1:
                    GenerateRoom(xPos, yPos + length + newSize, newSize, true, direction);
                    //up, 10 long, 3 wide
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

                //right
                case 2:
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
                    //extra room left
                    if ((extraRoom == 1))
                    {
                        GenerateRoom(xPos - length - newSize, yPos, newSize, false, 4);

                        //up, 10 long, 3 wide
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
                    //extra room down
                    if ((extraRoom == 1))
                    {
                        GenerateRoom(xPos, yPos - length - newSize, newSize, false, 3);

                        //up, 10 long, 3 wide
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
