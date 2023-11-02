using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Player : MonoBehaviour
{
    String[] tilemaptags;

    Transform playerTransform;

    TileBase closestTile;
    public TileBase wallTile;

    bool attacked;

    Camera playercam;

    float time;
    int idx;
    List<Vector3> listOfPositions;
    List<List<Vector3>> listOfListofPositions;

    public bool AIMode;


    private void Awake()
    {

        idx = 0;
        listOfPositions = new List<Vector3>();
        listOfListofPositions = new List<List<Vector3>>();

        playercam = GetComponentInChildren<Camera>();
        attacked = false;
        playerTransform = transform;


        //transform.position = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().startingposition;
        //Debug.Log("TRANSFORM POSITION " + GameObject.Find("MapGenerator").GetComponent<MapGenerator>().startingposition);


        //raycastOrigin = transform;

        //TAGS OF ROOM
        tilemaptags = new string[] { "WallTilemap", "FloorTilemap" };
    }





    private void Update()
    {


        if (!AIMode)
        {
            Movement();
            cameraMovement();
        }
        //Attack();

        //FindClosestTile(tilemaptags);
    }

    private void cameraMovement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            playercam.orthographicSize += 1f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            playercam.orthographicSize -= 1f * Time.deltaTime;
        }
    }

    private void Attack()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            if (!attacked)
            {
                Debug.Log("Attack ");
                Explode(transform.position, 1);
                attacked = true;
            }
        }
        else
        {
            //Debug.Log("NOT ATTACK");
            attacked = false;
        }
    }

    private void UpdateTargetingPosition()
    {
        time += Time.deltaTime;

        //if(time >= 3)
        //{
        //    if (listOfPositions[idx] != null)
        //    {
        //        listOfListofPositions[idx] = transform.position;
        //    }
        //    else
        //    {
        //        listOfListofPositions.Add(transform.position);
        //    }

        //    if(idx >= 5)
        //    {
        //        idx = 0;
        //    }
        //    time = 0;
        //}
    }
   

    


   private void Movement()
    {
        // Get input from the player
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement vector
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f).normalized * 5.0f * Time.deltaTime;

        // Calculate the player's potential new position
        Vector3 newPosition = transform.position + movement;

        // Calculate the rotation angle based on input (adjust the rotation speed as needed)
        float rotationAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg;

        // Apply rotation to the player
        transform.rotation = Quaternion.Euler(0f, 0f, -rotationAngle);

        // Apply movement
        transform.position = newPosition;
    }

    public void Explode(Vector3 explosionPosition, float explosionRadius)
    {
        // Find the GameObject with the "WallTilemap" tag.
        GameObject wallTilemapObject = GameObject.FindWithTag("WallTilemap");

        // Check if the GameObject with the tag was found.
        if (wallTilemapObject != null)
        {
            // Get the Tilemap component from the found GameObject.
            Tilemap wallTilemap = wallTilemapObject.GetComponent<Tilemap>();

            if (wallTilemap != null)
            {
                // Convert world position to tilemap cell position.
                Vector3Int cellPosition = wallTilemap.WorldToCell(explosionPosition);

                // Loop through tiles within the explosion radius.
                for (int x = -Mathf.FloorToInt(explosionRadius); x <= Mathf.FloorToInt(explosionRadius); x++)
                {
                    for (int y = -Mathf.FloorToInt(explosionRadius); y <= Mathf.FloorToInt(explosionRadius); y++)
                    {
                        // Calculate the position of the current cell.
                        Vector3Int currentCell = cellPosition + new Vector3Int(x, y, 0);

                        // Check if the cell contains a wall tile.
                        TileBase tile = wallTilemap.GetTile(currentCell);

                        // If it does, remove the wall tile.
                        if (tile != null)
                        {
                            wallTilemap.SetTile(currentCell, null);
                        }
                    }
                }
            }
        }
       
    }

    /*private void FindClosestTile(string[] tags)
    {
        Vector3 playerPosition = playerTransform.position;

        float closestDistance = float.MaxValue;
        TileBase closestTile = null;

        foreach (string tag in tags)
        {
            GameObject[] tilemapObjects = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject tilemapObject in tilemapObjects)
            {
                Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();

                foreach (Vector3Int cellPosition in tilemap.cellBounds.allPositionsWithin)
                {
                    TileBase tile = tilemap.GetTile(cellPosition);

                    if (tile != null)
                    {
                        Vector3 tileCenter = tilemap.GetCellCenterWorld(cellPosition);
                        float distance = Vector3.Distance(playerPosition, tileCenter);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTile = tile;
                        }
                    }
                }
            }
        }

        if (closestTile != null)
        {
            Debug.Log("Closest Tile: " + closestTile.name);
        }
        //else
        //{
        //    Debug.Log("No tiles found with the specified tags in the specified Tilemaps.");
        //}
    }*/







    private void FindClosestTile(string[] tags)
    {
        Vector3 playerPosition = playerTransform.position;
        Vector3 playerForward = playerTransform.forward; // Get the player's forward direction.

        float closestDistance = float.MaxValue;
        TileBase closestTile = null;

        foreach (string tag in tags)
        {
            GameObject[] tilemapObjects = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject tilemapObject in tilemapObjects)
            {
                Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();

                foreach (Vector3Int cellPosition in tilemap.cellBounds.allPositionsWithin)
                {
                    TileBase tile = tilemap.GetTile(cellPosition);

                    if (tile != null)
                    {
                        Vector3 tileCenter = tilemap.GetCellCenterWorld(cellPosition);
                        float distance = Vector3.Distance(playerPosition, tileCenter);

                        // Calculate the direction from the player to the tile.
                        Vector3 tileDirection = (tileCenter - playerPosition).normalized;

                        // Calculate the dot product between the player's forward direction and the tile direction.
                        float dotProduct = Vector3.Dot(playerForward, tileDirection);

                        // You can adjust the threshold for what is considered the "front" of the player.
                        // A dot product close to 1 means the tile is in front of the player.
                        float directionThreshold = 0.9f;

                        if (distance < closestDistance && dotProduct > directionThreshold)
                        {
                            closestDistance = distance;
                            closestTile = tile;
                        }
                    }
                }
            }
        }

        if (closestTile != null)
        {
            Debug.Log("Closest Tile: " + closestTile.name);
        }
        //else
        //{
        //    Debug.Log("No tiles found with the specified tags in the specified Tilemaps.");
        //}
    }


}
