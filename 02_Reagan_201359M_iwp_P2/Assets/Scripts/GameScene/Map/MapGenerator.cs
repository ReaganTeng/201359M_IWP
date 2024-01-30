using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public compass CompassObject;
    //public GameObject manualMap;
    public GameObject gameManager;
    public GameObject referenceobject;
    public GameObject treasureChestPrefab;
    public GameObject EndZone;
    public GameObject unbreakableWall;
    public GameObject Player;
    public GameObject treasureParent;
    public GameObject questTriggerParent;
    public GameObject shopkeeperParent;
    public GameObject shopkeeperPrefab;
    public GameObject questTriggerPrefab;

    public GameObject musicPlayerPrefab;


    [HideInInspector]
    public List<Vector2> occupiedPositions;
    [HideInInspector]
    public Vector2 startingposition;
    [HideInInspector]
    public int roomdimension;
    [HideInInspector]
    public List<GameObject> listofShopkeepers = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> listofQuestTriggers = new List<GameObject>();
   

    //List <GameObject> player = new List<GameObject>();
    GameObject player;
    
    PlayerManager playermgt;
    EnemyManager enemymgt;
    GameObject endZone;


    List<GameObject> generatedRooms = new List<GameObject>();
    int NumberOfPaths;
    int PathsImplemented;
    int PathsImplementedChecker;
    int NumberOfShopKeepers;
    int NumberOfQuestTriggers;
    public List<GameObject> startingRoomtemplates;
    public List<GameObject> exitRoomtemplates;
    List<GameObject> roomTemplates = new List<GameObject>(); // Array of room templates.
    public List<GameObject> multiDirectionalPaths;
    public List<GameObject> OneDirectionalPaths;
    GameObject currentRoom;
    GameObject prevRoomInstantiated;
    GameObject startingRoom;
    Vector2 currentRoomPosition;
    //Vector2 shopKeeperPrevPos;
    
    //public Tilemap mainWallTilemap;
    //public Tilemap mainFloorTilemap;
    //public Tile wallTile;
    //THE LENGTH AND WIDTH OF EACH ROOM
    
    bool levelGenerated;
    bool exitRoomSpawned;
    Direction[] directionValues;
    GameObject exitroom;
    List<Vector2> pathsNextToEntrance = new List<Vector2>();
    float attemptTimer;
    bool pathsFinishedDiverging;
    bool extraroomsSpawned;
    bool allroomsSelected;
    bool paintedtotilemap;
    bool enemiesspawned;
    bool poolinitiated;
    bool objectsGenerated;

    //Vector2 exitroompos;
    //bool AllPathsGenerated;
    //Direction startingroomDir;
    //int reductionlevel;
    //List<GameObject> divergingpaths;
    //bool DivergingPathsGenerated;
    //int offset;


    //public List<GameObject> EnemyList = new List<GameObject>();
    //bool enemypositionchosen = false;


    //SET EACH ROOM AS CHILD OF THIS GAMEOBJECT, FOR ORGANIZATION PURPOSES IN INSPECTOR
    public GameObject mapParent;



    [HideInInspector]
    public List<GameObject> roomswithopenpaths = new List<GameObject>();

    [HideInInspector]
    public int maxenemynumbers;


    bool alltreasuresSpawned;

    void Awake()
    {
        //shopKeeperPrevPos = Vector2.zero;
        objectsGenerated = false;
        alltreasuresSpawned = false;

        poolinitiated = false;
        //offset = 2;
        //reductionlevel = 0;
        //DivergingPathsGenerated = false;
        //AllPathsGenerated = false;
        enemymgt = gameManager.GetComponent<EnemyManager>();
        //maxenemynumbers = 50;
        playermgt = gameManager.GetComponent<PlayerManager>();


        enemiesspawned = false;
        paintedtotilemap = false;
        attemptTimer = 0;

        allroomsSelected = false;


        NumberOfShopKeepers = 20;
        NumberOfQuestTriggers = 20;
        //TEMPORARILY TRUE
        //pathsFinishedDiverging = true;
        //extraroomsSpawned = true;

        pathsFinishedDiverging = false;
        extraroomsSpawned = false;


        directionValues = (Direction[])Enum.GetValues(typeof(Direction));
        exitRoomSpawned = false;
        levelGenerated = false;


        //NumberOfPaths = Random.Range(50, 81);
        NumberOfPaths = Random.Range(20, 30);
        //NumberOfPaths = 100;
        //NumberOfPaths = 50;
        //NumberOfPaths = 1;

        //UnityEngine.Debug.Log("PATHS DECIDED " + NumberOfPaths);
        //NumberOfPaths = 10;
        PathsImplemented = 0;
       
        occupiedPositions = new List<Vector2>();
        roomdimension = 16;

        for (int i = 0; i < multiDirectionalPaths.Count; i++)
        {
            multiDirectionalPaths[i].GetComponent<Room>().prefabId = i;
            roomTemplates.Add(multiDirectionalPaths[i]);
           
        }

        for (int i = 0; i < OneDirectionalPaths.Count; i++)
        {
            OneDirectionalPaths[i].GetComponent<Room>().prefabId = i + multiDirectionalPaths.Count;
            roomTemplates.Add(OneDirectionalPaths[i]);
        }

        spawnStartingRoom();

    }



    public void SpawnTreasure()
    {
        if (!alltreasuresSpawned)
        {
    
            float smallestX = occupiedPositions.Min(pos => pos.x);
            float largestX = occupiedPositions.Max(pos => pos.x);
            float smallestY = occupiedPositions.Min(pos => pos.y);
            float largestY = occupiedPositions.Max(pos => pos.y);

            for (int i = 0; i< 1000; i++)
            {
                int randomX = Random.Range((int)smallestX, (int)largestX + 1);
                int randomY = Random.Range((int)smallestY, (int)largestY + 1);
                Vector2 randomPosition = new Vector2(
                    randomX,
                   randomY
                );
                
                GameObject chest = 
                    Instantiate(treasureChestPrefab, randomPosition, Quaternion.identity);
                chest.transform.SetParent(treasureParent.transform);
            }
            alltreasuresSpawned = true;
        }
    }


    void SpawnTreasuresChests()
    {
        int TreasureChestsSpawned = 0;
        while (TreasureChestsSpawned < 250)
        {
            foreach (Vector2 position in occupiedPositions)
            {
                int chance = Random.Range(0, 2);

                if (chance == 0)
                {
                    int randomX = Random.Range(0, roomdimension + 1);
                    int randomY = Random.Range(0, roomdimension + 1);
                    Vector2 randomPosition = new Vector2(
                        position.x + randomX,
                       position.y + randomY
                    );
                    //if ((listofQuestTriggers.Count > 0
                    //    && IsPositionValidQuestTriggers(randomPosition))
                    //    || listofQuestTriggers.Count <= 0)
                    if (IsPositionValidforTreasureChest(randomPosition))
                    {
                        GameObject chest =
                        Instantiate(treasureChestPrefab, 
                        randomPosition, Quaternion.identity);
                        chest.transform.SetParent(treasureParent.transform);
                        TreasureChestsSpawned += 1;
                    }
                }
            }
        }
    }


    void Update()
    {
        if (prevRoomInstantiated != null)
        {
            referenceobject.transform.position = prevRoomInstantiated.transform.position;
        }
        if (!levelGenerated)
        {
            //StartCoroutine(
            GenerateLevel();
            //);
        }
        else
        {
            //aaa
            if (!objectsGenerated)
            {
                if (!gameManager.activeSelf)
                {
                    gameManager.SetActive(true);
                }

                //if(player == null)
                if (!playermgt.finishedSpawning)
                {
                    //for (int i = 0; i < 2; i++)
                    //{
                    //    player = Instantiate(Player, startingposition, Quaternion.identity);
                    //    player.GetComponent<SpriteRenderer>().color = 
                    //    new Color (
                    //        Random.Range(0, 1),
                    //        Random.Range(0, 1),
                    //        Random.Range(0, 1)
                    //        );
                    //    playermgt.players.Add(player);
                    playermgt.StartItself();
                    //}
                }

                if (endZone == null)
                {
                    endZone = Instantiate(EndZone, exitroom.transform.position, Quaternion.identity);
                    //Instantiate(EndZone, new Vector2 (10, 10), Quaternion.identity);

                }

                if (!enemiesspawned
                    && playermgt.finishedSpawning)
                {
                    enemymgt.StartItself();
                    enemiesspawned = true;
                }

                //SPAWN SHOPKEEPERS
                SpawnShopKeepers();

                //SPAWN QUEST NPC
                SpawnQuestTriggers();

                //SpawnTreasure();

                //SPAWN TREASURECHESTS
                SpawnTreasuresChests();


                FindAnyObjectByType<VolumeManager>().AdjustAllVolumes();

                CompassObject.StartItself();
                GameObject.FindGameObjectWithTag("LoadingScreen").SetActive(false);


                Instantiate(musicPlayerPrefab);
                objectsGenerated = true;
            }

        }


    }


    void SpawnShopKeepers()
    {
        int shopkeepersSpawned = 0;
        while (shopkeepersSpawned < NumberOfShopKeepers)
        {
            foreach (Vector2 position in occupiedPositions)
            {
                int chance = Random.Range(0, 2);

                if (chance == 0)
                {
                    int randomX = Random.Range(0, roomdimension + 1);
                    int randomY = Random.Range(0, roomdimension + 1);
                    Vector2 randomPosition = new Vector2(
                        position.x + randomX,
                       position.y + randomY
                    );
                    if ((listofShopkeepers.Count > 0 
                        && IsPositionValidForShopKeeper(randomPosition))
                        || listofShopkeepers.Count <= 0)
                    {
                        GameObject shopkeeper = Instantiate(shopkeeperPrefab,
                            randomPosition, Quaternion.identity);
                        listofShopkeepers.Add(shopkeeper);
                        shopkeeper.transform.SetParent(shopkeeperParent.transform);
                        shopkeepersSpawned += 1;
                    }
                }
            }
        }  
    }

    void SpawnQuestTriggers()
    {
        int questtriggersspawned = 0;
        while (questtriggersspawned < NumberOfQuestTriggers)
        {
            foreach (Vector2 position in occupiedPositions)
            {
                int chance = Random.Range(0, 2);

                if (chance == 0)
                {
                    int randomX = Random.Range(0, roomdimension + 1);
                    int randomY = Random.Range(0, roomdimension + 1);
                    Vector2 randomPosition = new Vector2(
                        position.x + randomX,
                       position.y + randomY
                    );
                    if ((listofQuestTriggers.Count > 0
                        && IsPositionValidQuestTriggers(randomPosition))
                        || listofQuestTriggers.Count <= 0)
                    {
                        GameObject questtrigger = Instantiate(questTriggerPrefab,
                            randomPosition, Quaternion.identity);
                        listofQuestTriggers.Add(questtrigger);
                        int idx = listofQuestTriggers.IndexOf(questtrigger);
                        questtrigger.GetComponent<QuestTrigger>().questGiverid = idx;
                        questtrigger.transform.SetParent(questTriggerParent.transform);
                        questtriggersspawned += 1;
                    }
                }
            }
        }
    }

    public bool IsPositionValidForShopKeeper(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f, LayerMask.GetMask("WallTilemap"));

        if (colliders.Length > 0)
        {
            // There is a wall tile at the position
            return false;
        }

        foreach (GameObject shopkeeper in listofShopkeepers)
        {
            float distance = Vector2.Distance(position, shopkeeper.transform.position);
            //float distance2 = Vector2.Distance(position, mapGenerator.startingposition);

            if (distance <= roomdimension
                )
            {
                // Too close to another enemy
                return false;
            }
        }

        return true;
    }



    public bool IsPositionValidQuestTriggers(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f, LayerMask.GetMask("WallTilemap"));

        if (colliders.Length > 0)
        {
            // There is a wall tile at the position
            return false;
        }

        List<GameObject> obstacles = new List<GameObject>();
        obstacles.AddRange(listofShopkeepers);
        obstacles.AddRange(listofQuestTriggers);

        foreach (GameObject obstacle in obstacles)
        {
            float distance = Vector2.Distance(position, obstacle.transform.position);
            if (distance <= roomdimension)
            {
                return false;
            }
        }

        return true;
    }



    public bool IsPositionValidforTreasureChest(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f, LayerMask.GetMask("WallTilemap"));

        if (colliders.Length > 0)
        {
            // There is a wall tile at the position
            return false;
        }

        List<GameObject> obstacles = new List<GameObject>();
        obstacles.AddRange(listofShopkeepers);
        obstacles.AddRange(listofQuestTriggers);

        foreach (GameObject obstacle in obstacles)
        {
            float distance = Vector2.Distance(position, obstacle.transform.position);
            if (distance <= roomdimension/2)
            {
                return false;
            }
        }

        return true;
    }


    void spawnExitRoom()
    {
        Direction directionChosen = Direction.UP;
        bool directionselected = false;
        Direction oppositeDirection = Direction.UP;
        Vector2 positionToGo = Vector2.zero;
        Vector2 prevroomPosition = prevRoomInstantiated.transform.position;
        bool roomchosen = false;

        while (!directionselected)
        {
            int directionindexChosen = UnityEngine.Random.Range(0, 4);
            if (prevRoomInstantiated.GetComponent<Room>().availableDirections.Find(info => info.dir == directionValues[directionindexChosen]) != null
                && !prevRoomInstantiated.GetComponent<Room>().availableDirections.Find(info => info.dir == directionValues[directionindexChosen]).unExposed)
            {
                //UnityEngine.Debug.Log("DIRECTION CHOSEN");
                directionChosen = directionValues[directionindexChosen];
                directionselected = true;
            }
        }

        if (directionselected)
        {
            if (!roomchosen)
            {
                switch (directionChosen)
                {
                    case Direction.UP:
                        {
                            positionToGo.y += roomdimension;
                            oppositeDirection = Direction.DOWN;
                            break;
                        }
                    case Direction.DOWN:
                        {
                            positionToGo.y -= roomdimension;
                            oppositeDirection = Direction.UP;

                            break;
                        }
                    case Direction.LEFT:
                        {
                            positionToGo.x -= roomdimension;
                            oppositeDirection = Direction.RIGHT;

                            break;
                        }
                    case Direction.RIGHT:
                        {
                            positionToGo.x += roomdimension;
                            oppositeDirection = Direction.LEFT;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                currentRoomPosition = prevroomPosition + positionToGo;

                int roomtemplatechosen = UnityEngine.Random.Range(0, exitRoomtemplates.Count);
                //INSTANTIATE ROOM WHEN ROOM IS CHOSEN
                if (exitRoomtemplates[roomtemplatechosen].GetComponent<Room>().availableDirections.Find(info => info.dir == oppositeDirection) != null)
                {
                    currentRoom = Instantiate(exitRoomtemplates[roomtemplatechosen], currentRoomPosition, Quaternion.identity);
                    CheckOccupiedPaths(ref currentRoom);
                    occupiedPositions.Add(currentRoomPosition);
                    CheckOccupiedPaths(ref prevRoomInstantiated);
                    generatedRooms.Add(currentRoom);
                    prevRoomInstantiated = currentRoom;
                    exitRoomSpawned = true;
                    currentRoom.name = "DIRECT EXIT";
                    exitroom = currentRoom;

                    //SET AS CHILD OF MAPPARENT
                    currentRoom.transform.SetParent(mapParent.transform);

                    //UnityEngine.Debug.Log("EXIT ROOM SPAWNED");
                    roomchosen = true;
                }
                
            }
        }
    }

    void spawnStartingRoom()
    {
        int roomChosen = Random.Range(0, exitRoomtemplates.Count);
        GameObject room = Instantiate(startingRoomtemplates[roomChosen], Vector2.zero, Quaternion.identity);
        generatedRooms.Add(room);
        currentRoom = room;
        currentRoomPosition = room.transform.position;
        occupiedPositions.Add(currentRoomPosition);
        room.name = "DIRECT ENTRANCE";
        prevRoomInstantiated = currentRoom;
        startingRoom = prevRoomInstantiated;
        startingposition = room.transform.position;

        //SET AS CHILD OF MAPPARENT
        currentRoom.transform.SetParent(mapParent.transform);
    }

    //LOGIC TO GENERATE ONE PATH
    void RandomlyGeneratePath(ref GameObject room, ref int pathimplemented)
    {
        //UnityEngine.Debug.Log("PREV " + prevRoomInstantiated.transform.position);
        Direction directionChosen = Direction.UP;
        bool directionselected = false;
        Direction oppositeDirection = Direction.UP;
        Vector2 positionToGo = Vector2.zero;
        Vector2 prevroomPosition = room.transform.position;
        bool roomchosen = false;

        if (!directionselected)
        {
            int directionindexChosen = Random.Range(0, 4);
            // UnityEngine.Debug.Log("DIRECTION CHOSEN " + directionValues[directionindexChosen]);
            if (room.GetComponent<Room>().availableDirections.Find(info => info.dir 
            == directionValues[directionindexChosen]) != null
                && !room.GetComponent<Room>().availableDirections.Find(info => info.dir 
                == directionValues[directionindexChosen]).unExposed)
            {
                //UnityEngine.Debug.Log("DIRECTION CHOSEN " + directionValues[directionindexChosen]);
                directionChosen = directionValues[directionindexChosen];
                directionselected = true;
            }
        }
        //else
        if (directionselected)
        {
            //UnityEngine.Debug.Log("SELECTING DIRECTION NOW");
            //attempts += 1;
            if (!roomchosen)
            {
                switch (directionChosen)
                {
                    case Direction.UP:
                        {
                            positionToGo.y += roomdimension;
                            oppositeDirection = Direction.DOWN;
                            break;
                        }
                    case Direction.DOWN:
                        {
                            positionToGo.y -= roomdimension;
                            oppositeDirection = Direction.UP;
                            break;
                        }
                    case Direction.LEFT:
                        {
                            positionToGo.x -= roomdimension;
                            oppositeDirection = Direction.RIGHT;
                            break;
                        }
                    case Direction.RIGHT:
                        {
                            positionToGo.x += roomdimension;
                            oppositeDirection = Direction.LEFT;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                currentRoomPosition = prevroomPosition + positionToGo;
                int roomtemplatechosen = Random.Range(0, multiDirectionalPaths.Count);
                GameObject rm = multiDirectionalPaths[roomtemplatechosen];
                //INSTANTIATE ROOM WHEN ROOM IS CHOSEN
                //IF RM HAS OPPOSITE DIRECTION
                if (rm.GetComponent<Room>().availableDirections.Find(info => info.dir == oppositeDirection) 
                    != null)
                {
                    //NEW VERSION - FIND WHETHER THE ROOM CHOSEN IS INSIDE THE POOL
                    //bool allDirectionsInTemplate = roompool.Find(template =>
                    //template.GetComponent<Room>().availableDirections
                    //.Select(directionInfo => directionInfo.dir)
                    //.SequenceEqual(rm.GetComponent<Room>().availableDirections
                    //.Select(directionInfo => directionInfo.dir)
                    //));
                    //bool allDirectionsInTemplate = roompool.Find(template =>
                    //    rm.GetComponent<Room>().availableDirections.All(rmDir =>
                    //        template.GetComponent<Room>().availableDirections.Any(templateDir =>
                    //            templateDir.dir == rmDir.dir
                    //        ))
                    //    &&
                    //    template.GetComponent<Room>().availableDirections.All(templateDir =>
                    //        rm.GetComponent<Room>().availableDirections.Any(rmDir =>
                    //            rmDir.dir == templateDir.dir
                    //        ))
                    //);

                    //bool allDirectionsInTemplate = roompool.Find(template =>
                    //template.GetComponent<Room>().availableDirections
                    //.All(info => info == rm.GetComponent<Room>().availableDirections)
                    //) ;

                    //REFERENCE
                    //allUnExposed
                    //= room.GetComponent<Room>().availableDirections.All(directionInfo
                    //=> directionInfo.unExposed);

                    currentRoom = roompool.Find(template =>
                    //allDirectionsInTemplate
                    template.GetComponent<Room>().prefabId == rm.GetComponent<Room>().prefabId
                    && template.GetComponent<Room>().availableDirections.Count == rm.GetComponent<Room>().availableDirections.Count
                    && !template.activeSelf);
                    if (currentRoom == null)
                    {
                        currentRoom = Instantiate(rm, currentRoomPosition, Quaternion.identity);
                        roompool.Add(currentRoom);
                        int roomidx = roompool.IndexOf(currentRoom);
                        roompool[roomidx].name = $"{roompool[roomidx].name} RECLONE";
                        roompool[roomidx].transform.SetParent(roompoolParent.transform);
                        Debug.Log("DIRECT PATH INSTANTIATED");
                    }
                    else
                    {
                        roompool.Remove(currentRoom);
                        currentRoom.SetActive(true);
                        currentRoom.transform.position = currentRoomPosition;
                        Debug.Log("DIRECT PATH POOLED");
                    }
                    CheckOccupiedPaths(ref currentRoom);
                    occupiedPositions.Add(currentRoomPosition);
                    currentRoom.name = $"DIRECT PATH {currentRoom.name}";
                    CheckOccupiedPaths(ref room);
                    generatedRooms.Add(currentRoom);
                    //PREVIOUS ROOM TURNS TO CURRENT ROOM
                    room = currentRoom;
                    //SET AS CHILD OF MAPPARENT
                    currentRoom.transform.SetParent(mapParent.transform);
                    pathimplemented += 1;
                    PathsImplementedChecker = pathimplemented;
                    //attempts = 0;
                    //attemptTimer += 1;

                    //if (IsPathFullyClosed(room)
                    //|| IsNextToOccupiedPositions(room))
                    //{
                    //    BackTrack(ref room, ref pathimplemented);
                    //    //attempts = 0;
                    //}
                    //else
                    //{
                    //    attempts += 1;
                    //}

                    roomchosen = true;

                    //OLD VERSION
                    //currentRoom = Instantiate(rm, currentRoomPosition, Quaternion.identity);
                    //CheckOccupiedPaths(ref currentRoom);
                    ////UnityEngine.Debug.Log("PATHS IMPLEMENTED " + PathsImplemented);
                    //occupiedPositions.Add(currentRoomPosition);
                    //currentRoom.name = "DIRECT PATH";
                    //CheckOccupiedPaths(ref room);
                    //generatedRooms.Add(currentRoom);
                    //room = currentRoom;
                    ////SET AS CHILD OF MAPPARENT
                    //currentRoom.transform.SetParent(mapParent.transform);
                    //pathimplemented += 1;
                    //attempts = 0;
                    //roomchosen = true;
                }
                //else
                //{
                //    attempts += 1;
                //}
            }


            //RECHECK WHICH IS UNOCCUPIED AND/OR OCCUPIED
            for (int i = 0; i < generatedRooms.Count; i++)
            {
                GameObject generatedRoom = generatedRooms[i];
                CheckUnOccupiedPaths(ref generatedRoom);
                CheckOccupiedPaths(ref generatedRoom);
            }

        }
    }

    void BackTrack(ref GameObject room, ref int path)
    {
        occupiedPositions.Remove(room.transform.position);
        generatedRooms.Remove(room);
        Vector2 posi = room.transform.position;
        if (pathsNextToEntrance.Any(pos => pos == posi))
        {
            pathsNextToEntrance.Remove(posi);
        }
        //OLD METHOD - DESTROY ROOMS
        //Destroy(room);

        //NEW METHOD- POOL ROOM
        foreach (var dir in room.GetComponent<Room>().availableDirections)
        {
            dir.unExposed = false;
        }
        roompool.Add(room);
        int poolidx = roompool.IndexOf(room);
        roompool[poolidx].transform.position = Vector2.zero;
        roompool[poolidx].name = $"{roompool[poolidx].name} RECLONE";
        roompool[poolidx].transform.SetParent(roompoolParent.transform);
        roompool[poolidx].SetActive(false);
        //Destroy(room);
        //if (roompool[poolidx] != null)
        //{
        //    Debug.Log("NOT NULL");
        //}
        //

        //BACKTRACK TO PREV ROOM
        int idx = generatedRooms.Count - 1;
        room = generatedRooms[(idx)];
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            GameObject generatedRoom = generatedRooms[i];
            CheckUnOccupiedPaths(ref generatedRoom);
            CheckOccupiedPaths(ref generatedRoom);
            //generatedRooms[i] = generatedRoom;
        }
        //attemptTimer = 0;
        path -= 1;
        //Debug.Log("BACKTRACKING " + path);
        //levelGenerated = true;
    }

    void BackTrackVer2(ref GameObject room, ref int path)
    {
       
        room.name = "BACKTRACKED";
        occupiedPositions.Remove(room.transform.position);
        generatedRooms.Remove(room);
        Vector2 posi = room.transform.position;
        if (pathsNextToEntrance.Any(pos => pos == posi))
        {
            pathsNextToEntrance.Remove(posi);
        }

        //OLD METHOD - DESTROY ROOMS
        //Destroy(room);

        //NEW METHOD- POOL ROOM
        foreach (var dir in room.GetComponent<Room>().availableDirections)
        {
            dir.unExposed = false;
        }
        roompool.Add(room);
        int poolidx = roompool.IndexOf(room);
        roompool[poolidx].transform.position = Vector2.zero;
        roompool[poolidx].name = $"{roompool[poolidx].name} RECLONE VER 2";
        roompool[poolidx].transform.SetParent(roompoolParent.transform);
        roompool[poolidx].SetActive(false);
        //Destroy(room);
        //if (roompool[poolidx] != null)
        //{
        //    Debug.Log("NOT NULL");
        //}
        //

        //BACKTRACK TO PREV ROOM
        int idx = generatedRooms.Count - 1;
        room = generatedRooms[(idx)];
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            GameObject generatedRoom = generatedRooms[i];
            CheckUnOccupiedPaths(ref generatedRoom);
            CheckOccupiedPaths(ref generatedRoom);
            //generatedRooms[i] = generatedRoom;
        }
        Debug.Log("B-TRACKING " + PathsImplemented);
        //attempts = 0;
        path -= 1;
        //levelGenerated = true;
    }

    //CHECK WHETHER THE PATHS ARE OCCUPIED BY OTHER ROOMS, AND THEN CLOSE THE PATHS
    void CheckOccupiedPaths(ref GameObject room)
    {
        Vector2 spawnedroomposition = room.transform.position;
        Room spawnedroomRoomScript = room.GetComponent<Room>();
        // Iterate through the available directions in the spawned room.
        foreach (DirectionInfo info in spawnedroomRoomScript.availableDirections)
        {
            // Determine the expected position if the direction was taken.
            Vector2 expectedPosition = spawnedroomposition;

            switch (info.dir)
            {
                case Direction.LEFT:
                    expectedPosition.x -= roomdimension;
                    break;
                case Direction.RIGHT:
                    expectedPosition.x += roomdimension;
                    break;
                case Direction.UP:
                    expectedPosition.y += roomdimension;
                    break;
                case Direction.DOWN:
                    expectedPosition.y -= roomdimension;
                    break;
            }

            // Check if the expected position matches any occupied positions.
            if (occupiedPositions.Any(pos => pos == expectedPosition))
            {
                info.unExposed = true;
                //UnityEngine.Debug.Log("CLOSE PATH " + info.dir.ToString().ToLower());
            }
        }
    }

    //CHECK WHETHER THE PATHS ARE OCCUPIED BY OTHER NEIGHBOURS, AND THEN CLOSE THE PATHS
    void CheckUnOccupiedPaths(ref GameObject room)
    {
        Vector2 spawnedroomposition = room.transform.position;
        Room spawnedroomRoomScript = room.GetComponent<Room>();
        // Iterate through the available directions in the spawned room.
        foreach (DirectionInfo info in spawnedroomRoomScript.availableDirections)
        {
            Vector2 expectedPosition = spawnedroomposition;

            switch (info.dir)
            {
                case Direction.LEFT:
                    expectedPosition.x -= roomdimension;
                    break;
                case Direction.RIGHT:
                    expectedPosition.x += roomdimension;
                    break;
                case Direction.UP:
                    expectedPosition.y += roomdimension;
                    break;
                case Direction.DOWN:
                    expectedPosition.y -= roomdimension;
                    break;
                default:
                    break;
            }

            if (!occupiedPositions.Any(pos => pos == expectedPosition))
            //if (!occupiedPositions.Any(pos => pos != expectedPosition))
            {
                    //UnityEngine.Debug.Log("UNOCCUPY " + info.dir);
                    info.unExposed = false;
            }
        }


    }



   
    //SPAWN ROOMS THAT WILL NOT DIRECT TO THE PATH
    //private IEnumerator SpawnExtraRooms()
    // ...
 
    void SpawnExtraRooms()
    {
        GameObject roomchosen = unbreakableWall;
        Debug.Log("SPAWNING EXTRA ROOMS");
        float smallestX = occupiedPositions.Min(pos => pos.x);
        float largestX = occupiedPositions.Max(pos => pos.x);
        float smallestY = occupiedPositions.Min(pos => pos.y);
        float largestY = occupiedPositions.Max(pos => pos.y);

        //float smallestX = -16 * 10;
        //float largestX = 16 * 10;
        //float smallestY = -16 * 10;
        //float largestY = 16 * 10;

        for (int x_cor = (int)smallestX; x_cor <= (int)largestX; x_cor += roomdimension)
        {
            for (int y_cor = (int)smallestY; y_cor <= (int)largestY; y_cor += roomdimension)
            {
                if (!occupiedPositions.Contains(new Vector2(x_cor, y_cor)))
                {
                    int templatechosen = Random.Range(0, roomTemplates.Count);
                    roomchosen = roomTemplates[templatechosen];

                    //UnityEngine.Debug.Log("SPAWNING EXTRA");
                    //OLD VERSION, INSTANTIATE ROOM
                    //GameObject room = Instantiate(roomchosen, new Vector2(x_cor, y_cor), Quaternion.identity);

                    //NEW VERSION - xxx
                    //bool allDirectionsInTemplate = roompool.Find(template =>
                    //template.GetComponent<Room>().availableDirections
                    //.Select(directionInfo => directionInfo.dir)
                    //.SequenceEqual(roomchosen.GetComponent<Room>().availableDirections
                    //.Select(directionInfo => directionInfo.dir)));
                    GameObject room = roompool.Find(template
                       => //allDirectionsInTemplate
                    template.GetComponent<Room>().prefabId == roomchosen.GetComponent<Room>().prefabId
                       && template.GetComponent<Room>().availableDirections.Count == roomchosen.GetComponent<Room>().availableDirections.Count
                       && !template.activeSelf);
                    if (room == null)
                    {
                        //currentRoom = Instantiate(roomchosen, new Vector2(x_cor, y_cor), Quaternion.identity);
                        //roompool.Add(currentRoom);
                        //int roomidx = roompool.IndexOf(currentRoom);
                        //roompool[roomidx].name = $"{roompool[roomidx].name} RECLONE";
                        //roompool[roomidx].transform.SetParent(roompoolParent.transform);

                        room = Instantiate(roomchosen, new Vector2(x_cor, y_cor), Quaternion.identity);
                        roompool.Add(room);
                        int roomidx = roompool.IndexOf(room);
                        roompool[roomidx].name = $"{roompool[roomidx].name} RECLONE";
                        roompool[roomidx].transform.SetParent(roompoolParent.transform);
                    }
                    else
                    {
                        //currentRoom = room;
                        //currentRoom.SetActive(true);
                        //roompool.Remove(currentRoom);
                        //currentRoom.transform.position = new Vector2(x_cor, y_cor);

                        room.SetActive(true);
                        room.transform.position = new Vector2(x_cor, y_cor);
                        roompool.Remove(room);
                        Debug.Log("PATH POOLED");
                    }
                    //

                    room.name = $"EXTRA ROOM {room.name}";
                    generatedRooms.Add(room);
                    occupiedPositions.Add(room.transform.position);
                    //SET AS CHILD OF MAPPARENT
                    room.transform.SetParent(mapParent.transform);
                    //yield return null;
                }
                else
                {
                    Debug.Log("OCCUPIED");
                }
            }
            //yield return null;
        }
        extraroomsSpawned = true;
        //yield return null;

        //// Wait until DivergePaths has finished before continuing
        ////while (divergePathsRunning)
        //while (!pathsFinishedDiverging)
        //{
        //    yield return null;
        //}

        //// Find the smallest and largest X and Y values
        //float smallestX = occupiedPositions.Min(pos => pos.x);
        //float largestX = occupiedPositions.Max(pos => pos.x);
        //float smallestY = occupiedPositions.Min(pos => pos.y);
        //float largestY = occupiedPositions.Max(pos => pos.y);

        //for (int x_cor = (int)smallestX; x_cor <= (int)largestX; x_cor += roomdimension)
        //{
        //    for (int y_cor = (int)smallestY; y_cor <= (int)largestY; y_cor += roomdimension)
        //    {
        //        int templatechosen = UnityEngine.Random.Range(0, roomTemplates.Count);
        //        GameObject roomchosen = roomTemplates[templatechosen];
        //        //!occupiedPositions.Any(pos => pos != expectedPosition)
        //        //!occupiedPositions.Any(pos => pos != expectedPosition)
        //        if (!occupiedPositions.Contains(new Vector2(x_cor, y_cor)))
        //        {
        //            //UnityEngine.Debug.Log("SPAWNING EXTRA");
        //            //new Vector2(x_cor, y_cor)
        //            GameObject room = Instantiate(roomchosen, new Vector2(x_cor, y_cor), Quaternion.identity);
        //            room.name = "EXTRA ROOM";
        //            generatedRooms.Add(room);
        //            occupiedPositions.Add(room.transform.position);
        //            //SET AS CHILD OF MAPPARENT
        //            room.transform.SetParent(mapParent.transform);
        //            //yield return null;

        //        }
        //    }
        //    //yield return null;
        //}

        //extraroomsSpawned = true;
        ////yield return null;
        }

        //private IEnumerator SpawnUnbreakableWalls()
   void SpawnUnbreakableWalls()
    {
        float smallestX = occupiedPositions.Min(pos => pos.x);
        float largestX = occupiedPositions.Max(pos => pos.x);
        float smallestY = occupiedPositions.Min(pos => pos.y);
        float largestY = occupiedPositions.Max(pos => pos.y);
        GameObject roomchosen = unbreakableWall;
        //SPAWN UNBREAKABLE WALLS
        for (int x_cor = (int)smallestX - roomdimension; x_cor <= (int)largestX + roomdimension; x_cor += roomdimension)
        {
            for (int y_cor = (int)smallestY - roomdimension; y_cor <= (int)largestY + roomdimension; y_cor += roomdimension)
            {
                int templatechosen = UnityEngine.Random.Range(0, roomTemplates.Count);
                //if (!occupiedPositions.Any(pos => pos == new Vector2(x_cor, y_cor)))
                if (!occupiedPositions.Contains(new Vector2(x_cor, y_cor)))
                {
                    //OLD VERSION 
                    GameObject room = Instantiate(roomchosen, new Vector2(x_cor, y_cor), Quaternion.identity);

                    //NEW VERSION
                    //bool notallDirectionsInTemplate = roompool.Find(template =>
                    //template.GetComponent<Room>().availableDirections
                    //.Select(directionInfo => directionInfo.dir)
                    //.SequenceEqual(roomchosen.GetComponent<Room>().availableDirections
                    //.Select(directionInfo => directionInfo.dir)));
                    //GameObject room = roompool.Find(template
                    //   => !notallDirectionsInTemplate
                    //   && template.GetComponent<Room>().availableDirections.Count
                    //   == roomchosen.GetComponent<Room>().availableDirections.Count
                    //   && !template.activeSelf);
                    //if (room == null)
                    //{
                    //    currentRoom = Instantiate(roomchosen, new Vector2(x_cor, y_cor), Quaternion.identity);
                    //    roompool.Add(currentRoom);
                    //    int roomidx = roompool.IndexOf(currentRoom);
                    //    roompool[roomidx].name = $"{roompool[roomidx].name} RECLONE";
                    //    roompool[roomidx].transform.SetParent(roompoolParent.transform);
                    //}
                    //else
                    //{
                    //    // Reuse the room
                    //    currentRoom = room;
                    //    roompool.Remove(currentRoom);
                    //    currentRoom.transform.position = new Vector2(x_cor, y_cor);
                    //    currentRoom.SetActive(true);
                    //    Debug.Log("PATH POOLED");
                    //}
                    //

                    room.transform.SetParent(mapParent.transform);
                    //yield return null;
                }
                //yield return null;
            }
            //yield return null;
        }
        //yield return null;

        paintedtotilemap = true;
        levelGenerated = true;
        //yield return null;
    }


    //OLD VERSION
    //IEnumerator DivergePaths()
    //{
    //Vector2 positionToGo = Vector2.zero;
    //List<GameObject> roomswithopenpaths = new List<GameObject>();
    //bool allUnExposed;
    //int chosenmin = 0;

    ////UnityEngine.Debug.Log("DIVERGING PATH");
    //if (!allroomsSelected)
    //{
    //    foreach (GameObject room in generatedRooms)
    //    {
    //        allUnExposed = room.GetComponent<Room>().availableDirections.All(directionInfo => directionInfo.unExposed);
    //        if (!allUnExposed
    //            && room != startingRoom
    //            && room != exitroom)
    //        {
    //            roomswithopenpaths.Add(room);
    //        }
    //    }
    //    allroomsSelected = true;
    //    yield return null;
    //}

    ////DIVERGE PATHS
    //if (allroomsSelected
    //    && exitRoomSpawned
    //    && !pathsFinishedDiverging)
    //{
    //    //UnityEngine.Debug.Log("DIVERGING");
    //    for (int i = 0; i < roomswithopenpaths.Count; i++)
    //    {
    //        //UnityEngine.Debug.Log("DIVERGING " + i);
    //        GameObject roomselected = roomswithopenpaths[i];
    //        //prevRoomInstantiated = roomswithopenpaths[i];
    //        int pathimp = 0;
    //        int randomPath = UnityEngine.Random.Range(chosenmin, (NumberOfPaths * 2) + 1);
    //        for (int x = 0; x < randomPath; x++)
    //        {
    //            if (//pathimp == NumberOfPaths - 1 ||
    //                !IsNextToOccupiedPositions(roomselected)
    //                && !IsPositionOccupied(roomselected)
    //                )
    //            {
    //                //UnityEngine.Debug.Log("DIVERGING " + x);
    //                RandomlyGeneratePath(ref roomselected, ref pathimp);

    //                if (roomselected != roomswithopenpaths[i])
    //                {
    //                    roomselected.name = "DIVERGED PATH";
    //                }
    //                //if (IsPositionOccupied(roomselected)
    //                //    && roomselected)
    //                //{
    //                //    BackTrack(ref roomselected, ref pathimp);
    //                //}
    //            }

    //            if (//pathimp == NumberOfPaths - 1 ||
    //                IsNextToOccupiedPositions(roomselected)
    //                || IsPositionOccupied(roomselected)
    //                )
    //            {
    //                roomselected = roomswithopenpaths[i];
    //                allUnExposed = roomselected.GetComponent<Room>().availableDirections.Any(directionInfo => directionInfo.unExposed);
    //                if (allUnExposed)
    //                {
    //                    break;
    //                }
    //                else
    //                {
    //                    roomselected = roomswithopenpaths[i];
    //                    x = 0;
    //                }
    //            }
    //            yield return null;
    //        }
    //        // Yield to the next frame
    //        yield return null;
    //        roomselected = roomswithopenpaths[i];
    //        allUnExposed = roomselected.GetComponent<Room>().availableDirections.Any(directionInfo => directionInfo.unExposed);
    //        if (!allUnExposed)
    //        {
    //            i++;
    //        }
    //    }
    //    //UnityEngine.Debug.Log("FINISHED DIVERGING");
    //    pathsFinishedDiverging = true;
    //    yield return null;
    //}
    //}

    [HideInInspector]
    public List<GameObject> roompool = new List<GameObject>();
    public GameObject roompoolParent;
    private void InitializeRoomPool()
    {
        //INITIALIZE 100 SIMILAR ROOM TEMPLATES
        for (int i = 0; i < roomTemplates.Count; i++)
        {
            for (int x = 0; x < 10; x++)
            {
                GameObject room = Instantiate(roomTemplates[i]);
                room.SetActive(false);
                roompool.Add(room);
                room.transform.SetParent(roompoolParent.transform);
            }
        }
        poolinitiated = true;
    }





    //NEW VERSION
    //IEnumerator DivergePaths()
    void DivergePaths()
    {
        Vector2 positionToGo = Vector2.zero;
        //List<GameObject> roomswithopenpaths = new List<GameObject>();
        bool allUnExposed;
        int chosenmin = 0;


        //int randomNumberPath = Random.Range(chosenmin, (NumberOfPaths * 2) + 1);
        //int randomNumberPath = Random.Range(chosenmin, (int)(NumberOfPaths * .4f));
        //TEMPORARY MAXIMUM
        int randomNumberPath = Random.Range(chosenmin, 6);

        if (!allroomsSelected)
        {
            foreach (GameObject room in generatedRooms)
            {
                //CHECK IF ALL PATHS ARE CLOSED OR NOT
                allUnExposed
                    = room.GetComponent<Room>().availableDirections.All(directionInfo
                    => directionInfo.unExposed);
                
                if (!allUnExposed && room != startingRoom && room != exitroom)
                {
                    roomswithopenpaths.Add(room);
                }
            }
            Debug.Log("FINISHED DIVERGE");
            allroomsSelected = true;
        }
        //yield return null;

        if (allroomsSelected
            //&& exitRoomSpawned 
            && !pathsFinishedDiverging)
        {
            int pathimp;
            //List<int> randomNumberOfPaths = new List<int>();
            //for (int i = 0; i < roomswithopenpaths.Count; i++)
            //{
            //    //GameObject roomselected = roomswithopenpaths[i];
            //    int randomPath = Random.Range(chosenmin, (NumberOfPaths * 2) + 1);
            //    //if(roomselected)
            //    randomNumberOfPaths.Add(randomPath);
            //}

            for (int i = 0; i < roomswithopenpaths.Count;
                i++
                )
                //foreach (GameObject roomselected in roomswithopenpaths)
            {
                GameObject roomselected = roomswithopenpaths[i];
                //int randomNumberPath = randomNumberOfPaths[i];
                randomNumberPath = Random.Range(chosenmin, (NumberOfPaths * 2) + 1);
                pathimp = 0;

                if (!IsNextToOccupiedPositions(roomselected)
                    //&& !IsPositionOccupied(roomselected)
                    && !IsPathFullyClosed(roomselected)
                    )
                {
                    for (int x = 0; x < randomNumberPath; x++)
                    //for (int x = 0; x < 5; x++)
                    {
                        RandomlyGeneratePath(ref roomselected, ref pathimp);
                        if (roomselected != roomswithopenpaths[i])
                        {
                            roomselected.name = "DIVERGED PATH";
                        }
                        if (IsNextToOccupiedPositions(roomselected) || IsPathFullyClosed(roomselected))
                        {
                            break;
                        }
                    }
                }
                //allUnExposed
                //    = roomselected.GetComponent<Room>().availableDirections.Any(directionInfo
                //    => directionInfo.unExposed);
                //if (!allUnExposed)
                //{
                //    i++;
                //}
            }

            pathsFinishedDiverging = true;
            //yield return null;
        }
    }

    //private IEnumerator RestGeneration()
    //{
    //    //DIVERGE PATHS
    //    if (!pathsFinishedDiverging
    //           && exitRoomSpawned)
    //    {
    //        yield return
    //        DivergePaths();
    //    }
    //    if (pathsFinishedDiverging
    //      && !extraroomsSpawned)
    //    {
    //        yield return 
    //        SpawnExtraRooms();
    //    }
    //    if (extraroomsSpawned
    //        && !paintedtotilemap)
    //    {
    //        yield return 
    //        SpawnUnbreakableWalls();
    //    }
    //    // Yield to the next frame
    //    yield return null;
    //}

    //CHECK IF ALL SIDES OF THE ROOMS ARE CLOSED
    bool IsPathFullyClosed(GameObject room)
    {

        Vector2 startpos = room.transform.position;
        bool allUnExposed = room.GetComponent<Room>().availableDirections.All(directionInfo => directionInfo.unExposed);
        if(allUnExposed
            //&& startpos != startingposition
            //&& !pathsNextToEntrance.Any(pos => pos == startpos)
            )
        {
            //TRUE IF ALL OPENINGS ARE OCCUPIED
            //UnityEngine.Debug.Log("RETURN TRUE");
            return true;
        }
        else
        {
            //UnityEngine.Debug.Log("RETURN FALSE");
            return false;
        }   
    }

    bool IsNextToOccupiedPositions(GameObject room)
    {
        bool nextToRoom = false;
        Vector2 spawnedroomposition = room.transform.position;
        Room spawnedroomRoomScript = room.GetComponent<Room>();
        //UnityEngine.Debug.Log("ROOM TYPE " + room.name);
        int occupiedpos = 0;
        //occupiedPositions.Any(pos => pos == expectedPosition)
        // IF IT IS NOT THE ROOM NEXT TO THE STARTING ROOM 
        //if (!pathsNextToEntrance.Any(pos => pos == spawnedroomposition))
        //{
            // Iterate through the available directions in the spawned room.
            for (int i = 0; i < 8; i++)
            {
                int offs = roomdimension;
                // Determine the expected position if the direction was taken.
                Vector2 neighbouringPosition = spawnedroomposition;

                switch (i)
                {
                    case 0:
                    //case 4:
                        neighbouringPosition.x -= offs;
                        break;
                    case 1:
                    //case 5:
                        neighbouringPosition.x += offs;
                        break;
                    case 2:
                    //case 6:
                        neighbouringPosition.y += offs;
                        break;
                    case 3:
                    //case 7:
                        neighbouringPosition.y -= offs;
                        break;
                    case 4:
                    //case 0:
                        neighbouringPosition.x -= offs;
                        neighbouringPosition.y -= offs;
                        break;
                    case 5:
                    //case 1:
                        neighbouringPosition.x += offs;
                        neighbouringPosition.y += offs;
                        break;
                    case 6:
                    //case 2:
                        neighbouringPosition.x -= offs;
                        neighbouringPosition.y += offs;
                        break;
                    case 7:
                    //case 3:
                        neighbouringPosition.x += offs;
                        neighbouringPosition.y -= offs;
                        break;
                    default:
                        break;
                }

                if (//spawnedroomposition != startingposition &&
                    occupiedPositions.Any(pos => pos == neighbouringPosition)
                    )
                {
                    //UnityEngine.Debug.Log("FOUND OCCUPIED POSITION");
                    occupiedpos += 1;
                }   
            }
        //}

        if (occupiedpos >= 7)
        {
            nextToRoom = true;
            Debug.Log("OCCUPIED POS " + occupiedpos);
            //occupiedpos = 0;
        }


        return nextToRoom;
    }





    //IEnumerator Pathgenerator()
    void Pathgenerator()
    {
        RandomlyGeneratePath(ref prevRoomInstantiated, ref PathsImplemented);
        //attempts += 1; 
        //* Time.deltaTime;

        //if (!IsPathFullyClosed(prevRoomInstantiated)
        //    || !IsNextToOccupiedPositions(prevRoomInstantiated))
        //{
        //    attempts += 1;
        //}

       


        // BACKTRACK WHEN ANY ROOMS HAS ALL THE PATHS CLOSED
        if (IsPathFullyClosed(prevRoomInstantiated)
        || IsNextToOccupiedPositions(prevRoomInstantiated))
        {
            BackTrack(ref prevRoomInstantiated, ref PathsImplemented);
        }
        //else
        //{
        //    attempts += 1
        //    * Time.deltaTime;
        //}

        // Yield after processing the current frame
        //yield return null;
        if (attemptTimer >= 1)
        {
            int no = PathsImplemented;
            for (int i = 0; i < no - 1; i++)
            {
                BackTrackVer2(ref prevRoomInstantiated, ref PathsImplemented);
                // Yield after each backtrack to spread the load across frames
                //yield return null;
            }
            attemptTimer = 0;
        }

        if (generatedRooms.Count == PathsImplementedChecker)

            //if (PathsImplemented == PathsImplementedChecker)
        {
            attemptTimer += 1
            * Time.deltaTime;
            Debug.Log($"ATTEMPS {attemptTimer} {PathsImplementedChecker} {PathsImplemented}");
        }
        else
        {
            attemptTimer = 0;
        }
    }

    void GenerateLevel()
    {
        if(!poolinitiated)
        {
            InitializeRoomPool();
        }
        //UnityEngine.Debug.Log("PATHS IMPLEMENTED " + PathsImplemented);
        if (PathsImplemented < NumberOfPaths
            && poolinitiated)
        {
            //yield return
            //StartCoroutine(
            Pathgenerator();
            //);
            //SpawnExtraRooms();

            //yield return null;
        }
        if (PathsImplemented >= NumberOfPaths)
        {
            if (!exitRoomSpawned)
            {
                spawnExitRoom();
            }

            //StartCoroutine(RestGeneration());

            if (!pathsFinishedDiverging
                && exitRoomSpawned
                //&& !extraroomsSpawned
                )
            {
                //StartCoroutine(
                DivergePaths();
                //pathsFinishedDiverging = true;
                //);
                //SpawnExtraRooms();
            }
            //zzz
            if (pathsFinishedDiverging
              && !extraroomsSpawned)
            {
                //Debug.Log($"TTIME {t}");

                //StartCoroutine(
                SpawnExtraRooms();
                //CA_SpawnExtraRooms();
                //) ;
            }
            if (extraroomsSpawned
                && !paintedtotilemap)
            {
                //    //StartCoroutine(
                SpawnUnbreakableWalls();
            //        //);
            }
        }

    }



    

    
    //
}



[System.Serializable]
public class RoomDirections
{
    public List<Direction> directions;

    public RoomDirections(List<Direction> dirs)
    {
        directions = dirs;
    }
}





/*

    //private IEnumerator DivergePathsCoroutine()
    //{
    //    if (allroomsSelected || pathsFinishedDiverging)
    //    {
    //        yield break;
    //    }

    //    List<GameObject> roomswithopenpaths = generatedRooms
    //        .Where(room => !room.GetComponent<Room>().availableDirections.All(directionInfo => directionInfo.unExposed)
    //                    && room != startingRoom
    //                    && room != exitroom)
    //        .ToList();

    //    allroomsSelected = true;

    //    if (roomswithopenpaths.Count == 0)
    //    {
    //        pathsFinishedDiverging = true;
    //        yield break;
    //    }

    //    foreach (GameObject roomselected in roomswithopenpaths)
    //    {
    //        int pathimp = 0;
    //        int randomPath = UnityEngine.Random.Range(0, (NumberOfPaths * 2) + 1);

    //        for (int x = 0; x < randomPath; x++)
    //        {
    //            if (pathimp == NumberOfPaths - 1
    //                || (!IsNextToOccupiedPositions(roomselected) && !IsPositionOccupied(roomselected)))
    //            {
    //                GameObject tempRoom = roomselected;
    //                RandomlyGeneratePath(ref tempRoom, ref pathimp);

    //                if (tempRoom != null)
    //                {
    //                    tempRoom.name = "DIVERGED PATH";
    //                    roomselected.name = "DIVERGED PATH";
    //                }
    //            }

    //            if (pathimp == NumberOfPaths - 1
    //                || IsNextToOccupiedPositions(roomselected)
    //                || IsPositionOccupied(roomselected))
    //            {
    //                if (!roomselected.GetComponent<Room>().availableDirections.Any(directionInfo => directionInfo.unExposed))
    //                {
    //                    UnityEngine.Debug.Log("Breaking due to unexposed directions");
    //                    break;
    //                }
    //                x = 0;
    //            }

    //            yield return null; // Pause execution for one frame
    //        }

    //        if (!roomselected.GetComponent<Room>().availableDirections.Any(directionInfo => directionInfo.unExposed))
    //        {
    //            UnityEngine.Debug.Log("Skipping due to no unexposed directions");
    //            continue;
    //        }

    //        yield return null; // Pause execution for one frame
    //    }

    //    pathsFinishedDiverging = true;
    //    UnityEngine.Debug.Log("Diverging paths completed");
    //}

    //private void DivergePaths2()
    //{
    //    //StartCoroutine(DivergePathsCoroutine());
    //    //StartCoroutine(DivergePathsCo());
    //    StartCoroutine(DivergePaths());

    //}


    //private IEnumerator DivergePathsCo()
    //{
    //    Vector2 positionToGo = Vector2.zero;
    //    List<GameObject> roomswithopenpaths = new List<GameObject>();
    //    bool allUnExposed;
    //    int chosenmin = 0;

    //    if (!allroomsSelected)
    //    {
    //        foreach (GameObject room in generatedRooms)
    //        {
    //            allUnExposed = room.GetComponent<Room>().availableDirections.All(directionInfo => directionInfo.unExposed);

    //            if (!allUnExposed
    //                && room != startingRoom
    //                && room != exitroom)
    //            {
    //                roomswithopenpaths.Add(room);
    //            }
    //        }
    //        allroomsSelected = true;
    //    }

    //    if (allroomsSelected && !pathsFinishedDiverging)
    //    {
    //        int i = 0;

    //        while (i < roomswithopenpaths.Count)
    //        {
    //            GameObject roomselected = roomswithopenpaths[i];
    //            int pathimp = 0;

    //            int randomPath = UnityEngine.Random.Range(chosenmin, (NumberOfPaths * 2) + 1);

    //            int x = 0;
    //            while (x < randomPath)
    //            {
    //                if (!IsNextToOccupiedPositions(roomselected) && !IsPositionOccupied(roomselected))
    //                {
    //                    RandomlyGeneratePath(ref roomselected, ref pathimp);

    //                    if (roomselected != roomswithopenpaths[i])
    //                    {
    //                        roomselected.name = "DIVERGED PATH";
    //                    }
    //                }

    //                if (IsNextToOccupiedPositions(roomselected) || IsPositionOccupied(roomselected))
    //                {
    //                    roomselected = roomswithopenpaths[i];
    //                    allUnExposed = roomselected.GetComponent<Room>().availableDirections.Any(directionInfo => directionInfo.unExposed);
    //                    if (allUnExposed)
    //                    {
    //                        break;
    //                    }
    //                    else
    //                    {
    //                        roomselected = roomswithopenpaths[i];
    //                        x = 0;
    //                    }
    //                }
    //                x++;
    //                //yield return null; // Pause execution for one frame
    //            }

    //            roomselected = roomswithopenpaths[i];
    //            allUnExposed = roomselected.GetComponent<Room>().availableDirections.Any(directionInfo => directionInfo.unExposed);
    //            if (!allUnExposed)
    //            {
    //                i++;
    //            }
    //            yield return null; // Pause execution for one frame
    //        }
    //        pathsFinishedDiverging = true;
    //    }
    //}







    //IEnumerator AddWalls()
    //{
    //    // ADD UNBREAKABLE WALLS
    //    // Paint walls on empty areas of mainWallTilemap
    //    BoundsInt bound = mainWallTilemap.cellBounds;

    //    // Define your scaling factor (e.g., 2 for double width)
    //    int scalingFactor = 2;

    //    // Calculate the new bounds manually
    //    BoundsInt scaledBounds = new BoundsInt(
    //        bound.position - new Vector2Int(bound.size.x * (scalingFactor - 1) / 2,
    //        bound.size.y * (scalingFactor - 1) / 2, 0),
    //        new Vector2Int(bound.size.x * scalingFactor, bound.size.y * scalingFactor, bound.size.z)
    //    );

    //    TileBase wallTileAtPosition;
    //    TileBase floorTileAtPosition;

    //    foreach (Vector2Int position in scaledBounds.allPositionsWithin)
    //    {
    //        wallTileAtPosition = mainWallTilemap.GetTile(position);
    //        floorTileAtPosition = mainFloorTilemap.GetTile(position);

    //        if (wallTileAtPosition == null && floorTileAtPosition == null)
    //        {
    //            mainWallTilemap.SetTile(position, wallTile);
    //        }

    //        yield return new WaitForSeconds(wallAddDelay);
    //    }

    //    levelGenerated = true;
    //}




    //IEnumerator PaintRoomsToGlobalTilemaps()
    //{

    //    int objectsPerFrame = 5; // Adjust this to a suitable value
    //    int count = 0;

    //    Tilemap roomWallTilemap = null;
    //    Tilemap roomFloorTilemap = null;
    //    Transform roomTransform = null;

    //    for (int i = 0; i < generatedRooms.Count; i++)
    //    {
    //        count++;

    //        roomTransform = generatedRooms[i].transform;
    //        roomWallTilemap = roomTransform.Find("Walls").GetComponent<Tilemap>();
    //        roomFloorTilemap = roomTransform.Find("Floors").GetComponent<Tilemap>();

    //        // Copy wall tiles from room to mainWallTilemap
    //        BoundsInt wallBounds = roomWallTilemap.cellBounds;
    //        Vector2 roomWorldPos = roomTransform.position;
    //        Vector2Int roomWorldPosInt = new Vector2Int(
    //            Mathf.FloorToInt(roomWorldPos.x),
    //            Mathf.FloorToInt(roomWorldPos.y),
    //            Mathf.FloorToInt(roomWorldPos.z)
    //        );
    //        foreach (var position in wallBounds.allPositionsWithin)
    //        {
    //            mainWallTilemap.SetTile(roomWorldPosInt + position - wallBounds.min, roomWallTilemap.GetTile(position));
    //        }

    //        // Copy floor tiles from room to mainFloorTilemap
    //        BoundsInt floorBounds = roomFloorTilemap.cellBounds;
    //        foreach (var position in floorBounds.allPositionsWithin)
    //        {
    //            mainFloorTilemap.SetTile(roomWorldPosInt + position - floorBounds.min, roomFloorTilemap.GetTile(position));
    //        }

    //        if (count >= objectsPerFrame)
    //        {
    //            count = 0;
    //            yield return null;
    //        }

    //        //Debug.Log("ROOM " + i + " PAINTED");
    //    }

    //    paintedtotilemap = true;
    //    //levelGenerated = true;

    //    // Start destroying rooms within PaintRoomsToGlobalTilemaps
    //    IEnumerator destroyCoroutine = Destroyrooms();
    //    while (destroyCoroutine.MoveNext())
    //    {
    //        yield return null;
    //    }
    //}


    //IEnumerator Destroyrooms()
    //{
    //    int objectsPerFrame = 5; // Adjust this to a suitable value
    //    int count = 0;

    //    for (int i = 0; i < generatedRooms.Count; i++)
    //    {
    //        Destroy(generatedRooms[i]);
    //        count++;

    //        if (count >= objectsPerFrame)
    //        {
    //            count = 0;
    //            yield return null; // Yield after every batch of objects
    //        }
    //    }

    //    levelGenerated = true;
    //}





    //CHECK IF SPAWNED ROOM IS NEXT TO STARTING PATH
    //bool IsNextToStartingPath(GameObject room)
    //{
    //    bool nextToStartingRoom = false;
    //    Vector2 spawnedroomposition = room.transform.position;
    //    Room spawnedroomRoomScript = room.GetComponent<Room>();

    //    //occupiedPositions.Any(pos => pos == expectedPosition)
    //    // IF IT IS NOT THE ROOM NEXT TO THE STARTING ROOM 
    //    //checks if there is no element in the pathsNextToEntrance list that is equal to spawnedroomposition
    //    if (!pathsNextToEntrance.Any(pos => pos == spawnedroomposition)
    //        && spawnedroomposition != startingposition)
    //    {
    //        int[] possibleOffsets = { roomdimension}; // Add more offsets if needed
    //        //int[] possibleOffsets = { roomdimension * offset};
    //        //int[] possibleOffsets = { roomdimension };

    //        // Iterate through the available directions in the spawned room.
    //        for (int i = 0; i < 8; i++)
    //        {
    //            for (int j = 0; j < possibleOffsets.Length; j++)
    //            {
    //                int offs = possibleOffsets[j];
    //                // Determine the expected position if the direction was taken.
    //                Vector2 neighbouringPosition = spawnedroomposition;

    //                //int factor = 1;

    //                // switch (directionValues[i])
    //                switch (i)
    //                {
    //                    case 0:
    //                        //case 4:
    //                        neighbouringPosition.x -= offs;
    //                        break;
    //                    case 1:
    //                        //case 5:
    //                        neighbouringPosition.x += offs;
    //                        break;
    //                    case 2:
    //                        //case 6:
    //                        neighbouringPosition.y += offs;
    //                        break;
    //                    case 3:
    //                        //case 7:
    //                        neighbouringPosition.y -= offs;
    //                        break;
    //                    case 4:
    //                        neighbouringPosition.x -= offs;
    //                        neighbouringPosition.y -= offs;
    //                        break;
    //                    case 5:
    //                        neighbouringPosition.x += offs;
    //                        neighbouringPosition.y += offs;
    //                        break;
    //                    case 6:
    //                        neighbouringPosition.x -= offs;
    //                        neighbouringPosition.y += offs;
    //                        break;
    //                    case 7:
    //                        neighbouringPosition.x += offs;
    //                        neighbouringPosition.y -= offs;
    //                        break;
    //                    default:
    //                        break;
    //                }

    //                if (neighbouringPosition == startingposition //||
    //                    //occupiedPositions.Any(pos => pos == neighbouringPosition)
    //                    )
    //                {
    //                    nextToStartingRoom = true;
    //                    break;
    //                }
    //            }
    //            if (nextToStartingRoom)
    //            {
    //                break; // No need to continue checking if it's already next to the starting room
    //            }
    //        }
    //    }

    //    return nextToStartingRoom;
    //}

    //bool caseDirections(Direction dirVal, ref GameObject room)
    //{
    //    bool offsetcorrect = false;
    //    Vector2 spawnedroomposition = room.transform.position;
    //    Room spawnedroomRoomScript = room.GetComponent<Room>();

    //    if (//!pathsNextToEntrance.Any(pos => pos == spawnedroomposition) &&
    //        spawnedroomposition != startingposition)
    //    {
    //        switch (dirVal)
    //        {
    //            case Direction.UP:
    //                if (room.transform.position.y <= startingposition.y)
    //                {
    //                    offsetcorrect = true;
    //                }
    //                break;
    //            case Direction.DOWN:
    //                if (room.transform.position.y >= startingposition.y)
    //                {
    //                    offsetcorrect = true;
    //                }
    //                break;
    //            case Direction.RIGHT:
    //                if (room.transform.position.x >= startingposition.x)
    //                {
    //                    offsetcorrect = true;
    //                }
    //                break;
    //            case Direction.LEFT:
    //                if (room.transform.position.x <= startingposition.x)
    //                {
    //                    offsetcorrect = true;
    //                }
    //                break;
    //            default:
    //                break;

    //        }
    //    }



    //    return offsetcorrect;
    //}

    //void Diverge()
    //{

    //    Direction directionChosen = Direction.UP;
    //    Vector2 positionToGo = Vector2.zero;
    //    Vector2 prevroomPosition = prevRoomInstantiated.transform.position;

    //    int randomRoomSelected = UnityEngine.Random.Range(0, generatedRooms.Count);

    //    for (int i = 0; i < UnityEngine.Random.Range(5, 11); i++)
    //    {
    //        bool roomSelected = false;
    //        if (!roomSelected)
    //        {
    //            prevRoomInstantiated = generatedRooms[randomRoomSelected];
    //            int directionindexChosen = UnityEngine.Random.Range(0, 4);

    //            if (prevRoomInstantiated.GetComponent<Room>().availableDirections.Find(info => info.dir == directionValues[directionindexChosen]) != null
    //                && !prevRoomInstantiated.GetComponent<Room>().availableDirections.Find(info => info.dir == directionValues[directionindexChosen]).unExposed
    //                && IsPositionOccupied(prevRoomInstantiated))
    //            {
    //                //UnityEngine.Debug.Log("DIRECTION CHOSEN");
    //                directionChosen = directionValues[directionindexChosen];
    //                roomSelected = true;
    //            }
    //            else
    //            {
    //                //UnityEngine.Debug.Log("DIRECTION NOT CHOSEN");
    //                roomSelected = false;
    //            }
    //        }

    //        if (roomSelected)
    //        {
    //            //INSTANTIATE ROOM BASED ON DIRECTIONS
    //            //if (directionselected)
    //            //{
    //            //    if (!roomchosen)
    //            //    {

    //            //        switch (directionChosen)
    //            //        {
    //            //            case Direction.UP:
    //            //                {
    //            //                    positionToGo.y += roomdimension;
    //            //                    oppositeDirection = Direction.DOWN;
    //            //                    break;
    //            //                }
    //            //            case Direction.DOWN:
    //            //                {
    //            //                    positionToGo.y -= roomdimension;
    //            //                    oppositeDirection = Direction.UP;

    //            //                    break;
    //            //                }
    //            //            case Direction.LEFT:
    //            //                {
    //            //                    positionToGo.x -= roomdimension;
    //            //                    oppositeDirection = Direction.RIGHT;

    //            //                    break;
    //            //                }
    //            //            case Direction.RIGHT:
    //            //                {
    //            //                    positionToGo.x += roomdimension;
    //            //                    oppositeDirection = Direction.LEFT;
    //            //                    break;
    //            //                }
    //            //            default:
    //            //                {
    //            //                    break;
    //            //                }
    //            //        }
    //            //        currentRoomPosition = prevroomPosition + positionToGo;

    //            //        int roomtemplatechosen = UnityEngine.Random.Range(0, multiDirectionalPaths.Count);
    //            //        //INSTANTIATE ROOM WHEN ROOM IS CHOSEN

    //            //        if (multiDirectionalPaths[roomtemplatechosen].GetComponent<Room>().availableDirections.Find(info => info.dir == oppositeDirection) != null)
    //            //        {
    //            //            GameObject spawnedroom = Instantiate(multiDirectionalPaths[roomtemplatechosen], currentRoomPosition, Quaternion.identity);
    //            //            CheckOccupiedPaths(currentRoomPosition, spawnedroom.GetComponent<Room>());
    //            //            UnityEngine.Debug.Log("PATHS IMPLEMENTED " + PathsImplemented);
    //            //            occupiedPositions.Add(currentRoomPosition);
    //            //            CheckOccupiedPaths(prevRoomInstantiated.transform.position, prevRoomInstantiated.GetComponent<Room>());
    //            //            generatedRooms.Add(spawnedroom);
    //            //            prevRoomInstantiated = spawnedroom;
    //            //            spawnedroom.name = "PATH";
    //            //            PathsImplemented += 1;
    //            //            roomchosen = true;
    //            //        }
    //            //        else
    //            //        {
    //            //            roomchosen = false;
    //            //        }
    //            //    }
    //            //}
    //        }
    //    }
    //}


    //public void RandomGeneratePath()
    //{
    //    //UnityEngine.Debug.Log("PREV " + prevRoomInstantiated.transform.position);
    //    Direction directionChosen = Direction.UP;
    //    bool directionselected = false;

    //    Direction oppositeDirection = Direction.UP;
    //    Vector2 positionToGo = Vector2.zero;
    //    Vector2 prevroomPosition = prevRoomInstantiated.transform.position;
    //    bool roomchosen = false;

    //    if (!directionselected)
    //    {
    //        int directionindexChosen = UnityEngine.Random.Range(0, 4);
    //        if (prevRoomInstantiated.GetComponent<Room>().availableDirections.Find(info => info.dir == directionValues[directionindexChosen]) != null
    //            && !prevRoomInstantiated.GetComponent<Room>().availableDirections.Find(info => info.dir == directionValues[directionindexChosen]).unExposed)
    //        {
    //            //UnityEngine.Debug.Log("DIRECTION CHOSEN");
    //            directionChosen = directionValues[directionindexChosen];

    //            if (prevRoomInstantiated == startingRoom)
    //            {
    //                startingroomDir = directionValues[directionindexChosen];
    //            }

    //            directionselected = true;
    //        }
    //        else
    //        {
    //            UnityEngine.Debug.Log("DIRECTION NOT CHOSEN");
    //            directionselected = false;
    //        }
    //    }

    //    if (directionselected)
    //    {
    //        if (!roomchosen)
    //        {
    //            switch (directionChosen)
    //            {
    //                case Direction.UP:
    //                    {
    //                        positionToGo.y += roomdimension;
    //                        oppositeDirection = Direction.DOWN;
    //                        break;
    //                    }
    //                case Direction.DOWN:
    //                    {
    //                        positionToGo.y -= roomdimension;
    //                        oppositeDirection = Direction.UP;

    //                        break;
    //                    }
    //                case Direction.LEFT:
    //                    {
    //                        positionToGo.x -= roomdimension;
    //                        oppositeDirection = Direction.RIGHT;

    //                        break;
    //                    }
    //                case Direction.RIGHT:
    //                    {
    //                        positionToGo.x += roomdimension;
    //                        oppositeDirection = Direction.LEFT;
    //                        break;
    //                    }
    //                default:
    //                    {
    //                        break;
    //                    }
    //            }
    //            currentRoomPosition = prevroomPosition + positionToGo;

    //            int roomtemplatechosen = UnityEngine.Random.Range(0, multiDirectionalPaths.Count);
    //            GameObject rm = multiDirectionalPaths[roomtemplatechosen];
    //            //INSTANTIATE ROOM WHEN ROOM IS CHOSEN

    //            if (rm.GetComponent<Room>().availableDirections.Find(info => info.dir == oppositeDirection) != null)
    //            {
    //                currentRoom = Instantiate(rm, currentRoomPosition, Quaternion.identity);
    //                CheckOccupiedPaths(ref currentRoom);
    //                //UnityEngine.Debug.Log("PATHS IMPLEMENTED " + PathsImplemented);
    //                occupiedPositions.Add(currentRoomPosition);
    //                //CHECK IF THE ROOM IS SPAWNED NEXT TO ENTRANCE
    //                //Vector2 pos = room.transform.position;
    //                //if (pos == startingposition)
    //                //{
    //                //    currentRoom.name = "PATHnta";
    //                //    pathsNextToEntrance.Add(currentRoom.transform.position);
    //                //}
    //                //else
    //                //{
    //                currentRoom.name = "PATH";
    //                //}
    //                CheckOccupiedPaths(ref prevRoomInstantiated);
    //                generatedRooms.Add(currentRoom);
    //                prevRoomInstantiated = currentRoom;
    //                PathsImplemented -= 1;
    //                roomchosen = true;
    //            }
    //            else
    //            {
    //                roomchosen = false;
    //            }
    //        }
    //    }
    //}
    //public IEnumerator PaintRoomsToGlobalTilemaps()
    //{
    //    for (int i = 0; i < generatedRooms.Count; i++)
    //    {
    //        Transform roomTransform = generatedRooms[i].transform;
    //        Tilemap roomWallTilemap = roomTransform.Find("Walls").GetComponent<Tilemap>();
    //        Tilemap roomFloorTilemap = roomTransform.Find("Floors").GetComponent<Tilemap>();

    //        Vector2 roomWorldPos = roomTransform.position;
    //        BoundsInt wallBounds = roomWallTilemap.cellBounds;
    //        BoundsInt floorBounds = roomFloorTilemap.cellBounds;

    //        // Copy wall tiles from room to mainWallTilemap
    //        foreach (var position in wallBounds.allPositionsWithin)
    //        {
    //            Vector2Int roomPositionInMainTilemap = mainWallTilemap.WorldToCell(roomWorldPos) + position - wallBounds.min;
    //            Vector2Int roomPosition = position - wallBounds.min;

    //            TileBase wallTile = roomWallTilemap.GetTile(roomPosition);
    //            if (wallTile != null)
    //            {
    //                mainWallTilemap.SetTile(roomPositionInMainTilemap, wallTile);
    //            }
    //        }

    //        // Copy floor tiles from room to mainFloorTilemap
    //        foreach (var position in floorBounds.allPositionsWithin)
    //        {
    //            Vector2Int roomPositionInMainTilemap = mainFloorTilemap.WorldToCell(roomWorldPos) + position - floorBounds.min;
    //            Vector2Int roomPosition = position - floorBounds.min;

    //            TileBase floorTile = roomFloorTilemap.GetTile(roomPosition);
    //            if (floorTile != null)
    //            {
    //                mainFloorTilemap.SetTile(roomPositionInMainTilemap, floorTile);
    //            }
    //        }

    //        // Yield after each room has been processed
    //        yield return null;
    //    }

    //    //for (int i = 0; i < generatedRooms.Count; i++)
    //    //        {
    //    //            Destroy(generatedRooms[i]);
    //    //        }

    //        paintedtotilemap = true;
    //    // Notify that all rooms have been painted
    //    UnityEngine.Debug.Log("All rooms painted");
    //}

    //IEnumerator AddWalls()
    //{
    //    //ADD UNBREAKABLE WALLS
    //    //Paint walls on empty areas of mainWallTilemap
    //    BoundsInt bound = mainWallTilemap.cellBounds;

    //    // Define your scaling factor (e.g., 2 for double width)
    //    int scalingFactor = 2;

    //    // Calculate the new bounds manually
    //    BoundsInt scaledBounds = new BoundsInt(
    //        bound.position - new Vector2Int(bound.size.x * (scalingFactor - 1) / 2,
    //        bound.size.y * (scalingFactor - 1) / 2, 0),
    //        new Vector2Int(bound.size.x * scalingFactor, bound.size.y * scalingFactor, bound.size.z)
    //    );

    //    TileBase wallTileAtPosition;
    //    TileBase floorTileAtPosition;

    //    foreach (Vector2Int position in scaledBounds.allPositionsWithin)
    //    {
    //        wallTileAtPosition = mainWallTilemap.GetTile(position);
    //        floorTileAtPosition = mainFloorTilemap.GetTile(position);

    //        if (wallTileAtPosition == null && floorTileAtPosition == null)
    //        {
    //            mainWallTilemap.SetTile(position, wallTile);
    //        }
    //        yield return null;

    //        //Debug.Log("WALL ADDED");
    //    }

    //    levelGenerated = true;
    //    //
    //}




    //UnityEngine.Debug.Log("ATTEMPTS " + attempts);
    //UnityEngine.Debug.Log("PATH " + PathsImplemented);
    //if(AllPathsGenerated)
    //{
    //    DivergePaths();
    //}
    //private void DivergePaths()
    //{
    //    if (pathsFinishedDiverging) return; // Early exit if paths are already finished diverging

    //    List<GameObject> roomsWithOpenPaths = new List<GameObject>();

    //    foreach (GameObject room in generatedRooms)
    //    {
    //        Room roomScript = room.GetComponent<Room>();
    //        bool allUnExposed = roomScript.availableDirections.All(directionInfo => directionInfo.unExposed);

    //        if (!allUnExposed && room != startingRoom && room != exitroom)
    //        {
    //            roomsWithOpenPaths.Add(room);
    //        }
    //    }

    //    if (roomsWithOpenPaths.Count == 0)
    //    {
    //        pathsFinishedDiverging = true;
    //        return; // No more rooms with open paths, exit
    //    }

    //    int chosenMin = 0;

    //    for (int i = 0; i < roomsWithOpenPaths.Count; i++)
    //    {
    //        GameObject roomSelected = roomsWithOpenPaths[i];
    //        int pathImp = 0;
    //        int randomPath = UnityEngine.Random.Range(chosenMin, (NumberOfPaths * 2) + 1);

    //        for (int x = 0; x < randomPath; x++)
    //        {
    //            if (IsNextToOccupiedPositions(roomSelected) || IsPositionOccupied(roomSelected))
    //            {
    //                // The room can't generate more paths, skip it
    //                break;
    //            }

    //            RandomlyGeneratePath(ref roomSelected, ref pathImp);
    //            roomSelected.name = "DIVERGED PATH";
    //        }
    //    }

    //    pathsFinishedDiverging = true;
    //}
}

*/