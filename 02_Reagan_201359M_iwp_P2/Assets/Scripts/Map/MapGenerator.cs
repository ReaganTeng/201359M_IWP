using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;


public class MapGenerator : MonoBehaviour
{

    public int maxRooms; // Maximum number of rooms.
    List<GameObject> generatedRooms = new List<GameObject>();
    int NumberOfPaths;
    int PathsImplemented;
    public List<GameObject> startingRoomtemplates;
    public List<GameObject> exitRoomtemplates;
    List<GameObject> roomTemplates = new List<GameObject>(); // Array of room templates.
    public List<GameObject> multiDirectionalPaths;
    public List<GameObject> OneDirectionalPaths;
    GameObject currentRoom;
    GameObject prevRoomInstantiated;
    GameObject startingRoom;
    Vector2 currentRoomPosition;
    List<Vector2> occupiedPositions;
    public Vector2 startingposition;
    public Tilemap mainWallTilemap;
    public Tilemap mainFloorTilemap;
    public Tile wallTile;
    //THE LENGTH AND WIDTH OF EACH ROOM
    int roomdimension;
    bool levelGenerated;
    bool exitRoomSpawned;
    Direction[] directionValues;
    GameObject exitroom;
    List<Vector2> pathsNextToEntrance = new List<Vector2>();
    int attempts;
    bool pathsFinishedDiverging;
    bool extraroomsSpawned;
    bool allroomsSelected;
    bool paintedtotilemap;
    public GameObject unbreakableWall;

    //bool AllPathsGenerated;
    //Direction startingroomDir;
    //int reductionlevel;
    //List<GameObject> divergingpaths;
    //bool DivergingPathsGenerated;
    //int offset;

    void Awake()
    {
        //offset = 2;
        //reductionlevel = 0;
        //DivergingPathsGenerated = false;
        //AllPathsGenerated = false;

        paintedtotilemap = false;
        attempts = 0;

        allroomsSelected = false;
        pathsFinishedDiverging = false;
        extraroomsSpawned = false;

        

        directionValues = (Direction[])Enum.GetValues(typeof(Direction));
        exitRoomSpawned = false;
        
        levelGenerated = false;
        //NumberOfPaths = UnityEngine.Random.Range(50, 81);
        NumberOfPaths = 100;
        //UnityEngine.Debug.Log("PATHS DECIDED " + NumberOfPaths);
        //NumberOfPaths = 10;
        PathsImplemented = 0;
       
        occupiedPositions = new List<Vector2>();
        roomdimension = 4;

        for (int i = 0; i < multiDirectionalPaths.Count; i++)
        {
            roomTemplates.Add(multiDirectionalPaths[i]);
        }

        for (int i = 0; i < OneDirectionalPaths.Count; i++)
        {
            roomTemplates.Add(OneDirectionalPaths[i]);
        }

        // Step 1: Implement exit room randomly
        spawnStartingRoom();
    }



    private void Update()
    {

        if (!levelGenerated)
        {
            GenerateLevel();
        }
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
            //else
            //{
            //    UnityEngine.Debug.Log("DIRECTION NOT CHOSEN");
            //    directionselected = false;
            //}
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
                    currentRoom.name = "EXIT";
                    exitroom = currentRoom;
                    //UnityEngine.Debug.Log("EXIT ROOM SPAWNED");
                    roomchosen = true;
                }
                
            }
        }
    }


    void spawnStartingRoom()
    {
        int roomChosen = UnityEngine.Random.Range(0, exitRoomtemplates.Count);
        GameObject room = Instantiate(startingRoomtemplates[roomChosen], Vector3.zero, Quaternion.identity);
        generatedRooms.Add(room);
        currentRoom = room;
        currentRoomPosition = room.transform.position;
        occupiedPositions.Add(currentRoomPosition);
        room.name = "ENTRANCE";
        prevRoomInstantiated = currentRoom;
        startingRoom = prevRoomInstantiated;
        startingposition = room.transform.position;
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
            int directionindexChosen = UnityEngine.Random.Range(0, 4);
           // UnityEngine.Debug.Log("DIRECTION CHOSEN " + directionValues[directionindexChosen]);

            if (room.GetComponent<Room>().availableDirections.Find(info => info.dir == directionValues[directionindexChosen]) != null
                && !room.GetComponent<Room>().availableDirections.Find(info => info.dir == directionValues[directionindexChosen]).unExposed)
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

                int roomtemplatechosen = UnityEngine.Random.Range(0, multiDirectionalPaths.Count);
                GameObject rm = multiDirectionalPaths[roomtemplatechosen];
                //INSTANTIATE ROOM WHEN ROOM IS CHOSEN

                if (rm.GetComponent<Room>().availableDirections.Find(info => info.dir == oppositeDirection) != null)
                {
                    currentRoom = Instantiate(rm, currentRoomPosition, Quaternion.identity);
                    CheckOccupiedPaths(ref currentRoom);
                    //UnityEngine.Debug.Log("PATHS IMPLEMENTED " + PathsImplemented);
                    occupiedPositions.Add(currentRoomPosition);

                    //CHECK IF THE ROOM IS SPAWNED NEXT TO ENTRANCE AT A CERTAIN DISTANCE
                    //Vector2 pos = room.transform.position;
                    //Vector2 offset = pos - startingposition;
                    //float distance = offset.magnitude; // Calculate the distance
                    //// Define the maximum allowed distance from the starting position
                    //float maxDistance = roomdimension * 1;
                    //if (distance <= maxDistance)
                    //{
                    //    currentRoom.name = "PATHnta";
                    //    pathsNextToEntrance.Add(currentRoom.transform.position);
                    //}
                    //else
                    //{
                        currentRoom.name = "DIRECT PATH";
                    //}

                    CheckOccupiedPaths(ref room);
                    generatedRooms.Add(currentRoom);
                    room = currentRoom;
                    pathimplemented += 1;
                    attempts = 0;
                    roomchosen = true;
                }
            }
           
        }
    }

    void BackTrack(ref GameObject room, ref int path)
    {
        //UnityEngine.Debug.Log("REDUCTION LEVEL " + reductionlevel);
        //room.name = "BACKTRACKED";
        //int loop = 1;
        //if(PathsImplemented > 1)
        //{
        //    loop = 2;
        //}

        //for (int i = 0; i < 1; i++)
        //{
            occupiedPositions.Remove(room.transform.position);
            generatedRooms.Remove(room);

            Vector2 posi = room.transform.position;
            if (pathsNextToEntrance.Any(pos => pos == posi))
            {
                pathsNextToEntrance.Remove(posi);
            }

            Destroy(room);
            //room = generatedRooms[(generatedRooms.Count - 1) - reductionLevel];

            //int idx = 0;
            //if(generatedRooms.Count - 1 >= 0)
            //{
            //    idx = generatedRooms.Count - 1;
            //}
            int idx = generatedRooms.Count - 1;
            room = generatedRooms[(idx)];
        //CheckUnOccupiedPaths(room);
        //}

        for (int i = 0; i < generatedRooms.Count; i++)
        {
            GameObject generatedRoom = generatedRooms[i];
            CheckUnOccupiedPaths(ref generatedRoom);
            generatedRooms[i] = generatedRoom;
        }

        //UnityEngine.Debug.Log("B-TRACKING " + PathsImplemented);
        attempts = 0;
        path -= 1;
        //UnityEngine.Debug.Log("BACKTRACKING " + path);
    }

    void BackTrackVer2(ref GameObject room, ref int path)
    {
        //UnityEngine.Debug.Log("REDUCTION LEVEL " + reductionlevel);
        room.name = "BACKTRACKED";
        
        occupiedPositions.Remove(room.transform.position);
        generatedRooms.Remove(room);
        Vector2 posi = room.transform.position;
        if (pathsNextToEntrance.Any(pos => pos == posi))
        {
            pathsNextToEntrance.Remove(posi);
        }
        Destroy(room);
        
        int idx = generatedRooms.Count - 1;
        room = generatedRooms[(idx)];

        for (int i = 0; i < generatedRooms.Count; i++)
        {
            GameObject generatedRoom = generatedRooms[i];
            CheckUnOccupiedPaths(ref generatedRoom);
            generatedRooms[i] = generatedRoom;
        }

        UnityEngine.Debug.Log("B-TRACKING " + PathsImplemented);
        path -= 1;

    }

    //CHECK WHETHER THE PATHS ARE OCCUPIED BY OTHER NEIGHBOURS, AND THEN CLOSE THE PATHS
    private void CheckOccupiedPaths(ref GameObject room)
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
    private void CheckUnOccupiedPaths(ref GameObject room)
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

            if (!occupiedPositions.Any(pos => pos != expectedPosition))
            {
                //UnityEngine.Debug.Log("UNOCCUPY " + info.dir);
                info.unExposed = false;
            }
        }
    }

   
    //SPAWN ROOMS THAT WILL NOT DIRECT TO THE PATH
    private IEnumerator SpawnExtraRooms()
    {

        // Find the smallest and largest X and Y values
        float smallestX = occupiedPositions.Min(pos => pos.x);
        float largestX = occupiedPositions.Max(pos => pos.x);
        float smallestY = occupiedPositions.Min(pos => pos.y);
        float largestY = occupiedPositions.Max(pos => pos.y);

        

        for (int x_cor = (int)smallestX; x_cor <= (int)largestX; x_cor += roomdimension)
        {
            for (int y_cor = (int)smallestY; y_cor <= (int)largestY; y_cor += roomdimension)
            {
                int templatechosen = UnityEngine.Random.Range(0, roomTemplates.Count);
                GameObject roomchosen = roomTemplates[templatechosen];

                //!occupiedPositions.Any(pos => pos != expectedPosition)
                //!occupiedPositions.Any(pos => pos != expectedPosition)
                if (!occupiedPositions.Any(pos => pos == new Vector2(x_cor, y_cor)))
                {
                    //UnityEngine.Debug.Log("SPAWNING EXTRA");

                    //new Vector2(x_cor, y_cor)
                    GameObject room = Instantiate(roomchosen, new Vector2(x_cor, y_cor), Quaternion.identity);
                    room.name = "EXTRA ROOM";
                    generatedRooms.Add(room);
                    occupiedPositions.Add(room.transform.position);
                    yield return null;

                }
            }
        }

        extraroomsSpawned = true;
    }

    void SpawnUnbreakableWalls()
    {
        // Find the smallest and largest X and Y values
        float smallestX = occupiedPositions.Min(pos => pos.x);
        float largestX = occupiedPositions.Max(pos => pos.x);
        float smallestY = occupiedPositions.Min(pos => pos.y);
        float largestY = occupiedPositions.Max(pos => pos.y);
        GameObject roomchosen = unbreakableWall;

        for (int x_cor = (int)smallestX - roomdimension; x_cor <= (int)largestX + roomdimension; x_cor += roomdimension)
        {
            for (int y_cor = (int)smallestY - roomdimension; y_cor <= (int)largestY + roomdimension; y_cor += roomdimension)
            {
                int templatechosen = UnityEngine.Random.Range(0, roomTemplates.Count);
                if (!occupiedPositions.Any(pos => pos == new Vector2(x_cor, y_cor)))
                {
                    GameObject room = Instantiate(roomchosen, new Vector2(x_cor, y_cor), Quaternion.identity);
                }
            }
            //yield return null;
        }

        paintedtotilemap = true;
        levelGenerated = true;
    }






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




    private void DivergePaths()
    {
        Vector2 positionToGo = Vector2.zero;

        List<GameObject> roomswithopenpaths = new List<GameObject>();
        bool allUnExposed;
        int chosenmin = 0;

        //UnityEngine.Debug.Log("DIVERGING PATH");


        if (!allroomsSelected)
        {
            foreach (GameObject room in generatedRooms)
            {
                allUnExposed = room.GetComponent<Room>().availableDirections.All(directionInfo => directionInfo.unExposed);

                if (!allUnExposed
                    && room != startingRoom
                    && room != exitroom

                    )
                {
                    roomswithopenpaths.Add(room);
                }
            }
            allroomsSelected = true;
        }
        //allroomsSelected = roomswithopenpaths.Count == generatedRooms.Count;

        if (allroomsSelected
            && !pathsFinishedDiverging)
        {
            //UnityEngine.Debug.Log("DIVERGING");
            for (int i = 0; i < roomswithopenpaths.Count; i++)
            {
                //UnityEngine.Debug.Log("DIVERGING " + i);
                GameObject roomselected = roomswithopenpaths[i];
                //prevRoomInstantiated = roomswithopenpaths[i];
                int pathimp = 0;

                int randomPath = UnityEngine.Random.Range(chosenmin, (NumberOfPaths * 2) + 1);

                for (int x = 0; x < randomPath; x++)
                {

                    if (//pathimp == NumberOfPaths - 1 ||
                        !IsNextToOccupiedPositions(roomselected)
                        && !IsPositionOccupied(roomselected)
                        )
                    {
                        //UnityEngine.Debug.Log("DIVERGING " + x);
                        RandomlyGeneratePath(ref roomselected, ref pathimp);

                        if (roomselected != roomswithopenpaths[i])
                        {
                            roomselected.name = "DIVERGED PATH";
                        }
                        //if (IsPositionOccupied(roomselected)
                        //    && roomselected)
                        //{
                        //    BackTrack(ref roomselected, ref pathimp);
                        //}
                    }

                    if (//pathimp == NumberOfPaths - 1 ||
                        IsNextToOccupiedPositions(roomselected)
                        || IsPositionOccupied(roomselected)
                        )
                    {
                        roomselected = roomswithopenpaths[i];
                        allUnExposed = roomselected.GetComponent<Room>().availableDirections.Any(directionInfo => directionInfo.unExposed);
                        if (allUnExposed)
                        {
                            break;
                        }
                        else
                        {
                            roomselected = roomswithopenpaths[i];
                            x = 0;
                        }
                    }
                }

                // Yield to the next frame
                //yield return null;

                roomselected = roomswithopenpaths[i];
                allUnExposed = roomselected.GetComponent<Room>().availableDirections.Any(directionInfo => directionInfo.unExposed);
                if (!allUnExposed)
                {
                    i++;
                }
            }

            //UnityEngine.Debug.Log("FINISHED DIVERGING");
            pathsFinishedDiverging = true;
        }

        // Yield to the next frame
        //yield return null;
        //UnityEngine.Debug.Log("DIVERGING PATH");
    }

    //CHECK IF ALL SIDES OF THE ROOMS ARE CLOSED
    bool IsPositionOccupied(GameObject room)
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
        bool nextToStartingRoom = false;
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

        if (occupiedpos >= 8)
        {
            nextToStartingRoom = true;
            UnityEngine.Debug.Log("OCCUPIED POS " + occupiedpos);
            //occupiedpos = 0;
        }


        return nextToStartingRoom;
    }

    void GenerateLevel()
    {
        //UnityEngine.Debug.Log("PATHS IMPLEMENTED " + PathsImplemented);
        if (PathsImplemented < NumberOfPaths)
        {
            RandomlyGeneratePath(ref prevRoomInstantiated, ref PathsImplemented);
            attempts += 1;

            //BACKTRACK WHEN ANY ROOMS HAS ALL THE PATHS CLOSED
            if (
             IsPositionOccupied(prevRoomInstantiated)
             || IsNextToOccupiedPositions(prevRoomInstantiated)
             )
            {
                //UnityEngine.Debug.Log("BACKTRACK");
                BackTrack(ref prevRoomInstantiated, ref PathsImplemented);
            }


            if (attempts >= NumberOfPaths)
            {
                int no = PathsImplemented;
                //if (no % 2 != 0)
                //{
                //    no -= 1;
                //}
                for (int i = 0; i < no - 1; i++)
                {
                    //UnityEngine.Debug.Log("BACKTRACK VER 2");
                    BackTrackVer2(ref prevRoomInstantiated, ref PathsImplemented);
                }
                attempts = 0;
            }

            //}
        }
        else
        {
            if (!exitRoomSpawned)
            {
                spawnExitRoom();
            }

            if (exitRoomSpawned
                && !pathsFinishedDiverging)
            {
                UnityEngine.Debug.Log("SPAWNING DIVERGING PATHS");
                //StartCoroutine(DivergePaths());
                DivergePaths();
            }



            //if (pathsFinishedDiverging
            //    && !extraroomsSpawned)
            //{
            //    UnityEngine.Debug.Log("SPAWNING EXTRA ROOMS");

            //    StartCoroutine(SpawnExtraRooms());
            //    //levelGenerated = true;
            //}

            //if (extraroomsSpawned
            //    && !paintedtotilemap
            //    )
            //{
            //    UnityEngine.Debug.Log("SPAWNING PAINT");
            //    SpawnUnbreakableWalls();

            //    //PaintRoomsToGlobalTilemaps();
            //}













            //if (extraroomsSpawned
            //    //&& !paintedtotilemap
            //    )
            //{
            //    UnityEngine.Debug.Log("SPAWNING PAINT");
            //   // StartCoroutine(PaintRoomsToGlobalTilemaps());

            //    //PaintRoomsToGlobalTilemaps();
            //}


            //if (paintedtotilemap)
            //{
            //    UnityEngine.Debug.Log("SPAWNING UNBREAKABLE WALLS");
            //    StartCoroutine(Destroyrooms());
            //}
        }

        //UnityEngine.Debug.Log("ATTEMPTS " + attempts);

        //UnityEngine.Debug.Log("PATH " + PathsImplemented);

        //if(AllPathsGenerated)
        //{
        //    DivergePaths();
        //}

    }
















    //IEnumerator AddWalls()
    //{
    //    // ADD UNBREAKABLE WALLS
    //    // Paint walls on empty areas of mainWallTilemap
    //    BoundsInt bound = mainWallTilemap.cellBounds;

    //    // Define your scaling factor (e.g., 2 for double width)
    //    int scalingFactor = 2;

    //    // Calculate the new bounds manually
    //    BoundsInt scaledBounds = new BoundsInt(
    //        bound.position - new Vector3Int(bound.size.x * (scalingFactor - 1) / 2,
    //        bound.size.y * (scalingFactor - 1) / 2, 0),
    //        new Vector3Int(bound.size.x * scalingFactor, bound.size.y * scalingFactor, bound.size.z)
    //    );

    //    TileBase wallTileAtPosition;
    //    TileBase floorTileAtPosition;

    //    foreach (Vector3Int position in scaledBounds.allPositionsWithin)
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
    //        Vector3 roomWorldPos = roomTransform.position;
    //        Vector3Int roomWorldPosInt = new Vector3Int(
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
}
//public IEnumerator PaintRoomsToGlobalTilemaps()
//{
//    for (int i = 0; i < generatedRooms.Count; i++)
//    {
//        Transform roomTransform = generatedRooms[i].transform;
//        Tilemap roomWallTilemap = roomTransform.Find("Walls").GetComponent<Tilemap>();
//        Tilemap roomFloorTilemap = roomTransform.Find("Floors").GetComponent<Tilemap>();

//        Vector3 roomWorldPos = roomTransform.position;
//        BoundsInt wallBounds = roomWallTilemap.cellBounds;
//        BoundsInt floorBounds = roomFloorTilemap.cellBounds;

//        // Copy wall tiles from room to mainWallTilemap
//        foreach (var position in wallBounds.allPositionsWithin)
//        {
//            Vector3Int roomPositionInMainTilemap = mainWallTilemap.WorldToCell(roomWorldPos) + position - wallBounds.min;
//            Vector3Int roomPosition = position - wallBounds.min;

//            TileBase wallTile = roomWallTilemap.GetTile(roomPosition);
//            if (wallTile != null)
//            {
//                mainWallTilemap.SetTile(roomPositionInMainTilemap, wallTile);
//            }
//        }

//        // Copy floor tiles from room to mainFloorTilemap
//        foreach (var position in floorBounds.allPositionsWithin)
//        {
//            Vector3Int roomPositionInMainTilemap = mainFloorTilemap.WorldToCell(roomWorldPos) + position - floorBounds.min;
//            Vector3Int roomPosition = position - floorBounds.min;

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
//        bound.position - new Vector3Int(bound.size.x * (scalingFactor - 1) / 2,
//        bound.size.y * (scalingFactor - 1) / 2, 0),
//        new Vector3Int(bound.size.x * scalingFactor, bound.size.y * scalingFactor, bound.size.z)
//    );

//    TileBase wallTileAtPosition;
//    TileBase floorTileAtPosition;

//    foreach (Vector3Int position in scaledBounds.allPositionsWithin)
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
