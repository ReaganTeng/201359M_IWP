using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    String[] tilemaptags;


    TileBase closestTile;
    public TileBase wallTile;

    bool attacked;

    float time;
    int idx;
    List<Vector3> listOfPositions;
    //List<List<Vector3>> listOfListofPositions;

    GameObject player;
    Vector3 playerpos;


    protected float distance;
    public float stoppingdistance;
    public float distanceToIdle;

    private void Awake()
    {
        idx = 0;
        listOfPositions = new List<Vector3>();

        attacked = false;
        player = GameObject.FindGameObjectWithTag("Player");

        //transform.position = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().startingposition;
        //Debug.Log("TRANSFORM POSITION " + GameObject.Find("MapGenerator").GetComponent<MapGenerator>().startingposition);
        //raycastOrigin = transform;

        listOfPositions.Add(player.transform.position);

        //TAGS OF ROOM
        tilemaptags = new string[] { "WallTilemap", "FloorTilemap" };
    }

    protected void FollowPlayer()
    {
        //Vector3 targetpos = UpdateTargetingPosition();
        Vector3 targetpos = UpdateTargetingPosition();
        float speed = 1.0f;
        // Calculate the direction from the follower to the target
        Vector3 direction = targetpos - transform.position;
        // Normalize the direction vector (optional, keeps the movement consistent)
        direction.Normalize();
        // Update the position of the follower toward the target
        transform.position += direction * speed * Time.deltaTime;
        if (transform.position == targetpos)
        {
            listOfPositions.Remove(targetpos);
        }

    }

    private Vector3 UpdateTargetingPosition()
    {
        time += Time.deltaTime;

        if (time >= 0.5)
        {
            if (listOfPositions.Count < 5)
            {
                listOfPositions.Add(playerpos);
                Debug.Log("POSITION ADDED");
            }
            time = 0;
        }

        int count = listOfPositions.Count - 1;

        if (count < 0)
        {
            count = 0;
        }

        if (listOfPositions.Count > 0)
        {
            //Debug.Log(listOfPositions[count].ToString());
            return listOfPositions[count];
        }
        else
        {
            // If the list is empty, return the player's current position as a fallback.
            return transform.position;
        }
    }







    public enum EnemyState
    {
        IDLE,
        CHASE,
        ABOUT_TO_ATTACK,
        ATTACK,
        HURT
    }

    public EnemyState currentState;

    protected virtual void Update()
    {
        //Debug.Log("REFERENCING FROM");

        playerpos = player.transform.position;

        //THE CURRENT DISTANCE BETWEEN PLAYER AND ENEMY
        distance = Vector2.Distance(playerpos, transform.position);

        
        if(distance >= distanceToIdle)
        {
            if(currentState != EnemyState.IDLE)
            {
                Debug.Log("SET TO IDLE");
                currentState = EnemyState.IDLE;
            }
        }
        else
        {
            if (currentState == EnemyState.IDLE)
            {
                Debug.Log("SET TO CHASE");
                currentState = EnemyState.CHASE;
            }
        }

        // Implement state-specific behavior in derived classes
        switch (currentState)
        {
            //case EnemyState.IDLE:

            //    break;
            case EnemyState.CHASE:
                if (distance >= stoppingdistance)
                {
                    FollowPlayer();
                }
                break;
            case EnemyState.ABOUT_TO_ATTACK:
                break;
            case EnemyState.ATTACK:
                break;
            case EnemyState.HURT:
                break;
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
        //Vector3 playerPosition = playerTransform.position;
        //Vector3 playerForward = playerTransform.forward; // Get the player's forward direction.

        //float closestDistance = float.MaxValue;
        //TileBase closestTile = null;

        //foreach (string tag in tags)
        //{
        //    GameObject[] tilemapObjects = GameObject.FindGameObjectsWithTag(tag);

        //    foreach (GameObject tilemapObject in tilemapObjects)
        //    {
        //        Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();

        //        foreach (Vector3Int cellPosition in tilemap.cellBounds.allPositionsWithin)
        //        {
        //            TileBase tile = tilemap.GetTile(cellPosition);

        //            if (tile != null)
        //            {
        //                Vector3 tileCenter = tilemap.GetCellCenterWorld(cellPosition);
        //                float distance = Vector3.Distance(playerPosition, tileCenter);

        //                // Calculate the direction from the player to the tile.
        //                Vector3 tileDirection = (tileCenter - playerPosition).normalized;

        //                // Calculate the dot product between the player's forward direction and the tile direction.
        //                float dotProduct = Vector3.Dot(playerForward, tileDirection);

        //                // You can adjust the threshold for what is considered the "front" of the player.
        //                // A dot product close to 1 means the tile is in front of the player.
        //                float directionThreshold = 0.9f;

        //                if (distance < closestDistance && dotProduct > directionThreshold)
        //                {
        //                    closestDistance = distance;
        //                    closestTile = tile;
        //                }
        //            }
        //        }
        //    }
        //}

        //if (closestTile != null)
        //{
        //    Debug.Log("Closest Tile: " + closestTile.name);
        //}
        //else
        //{
        //    Debug.Log("No tiles found with the specified tags in the specified Tilemaps.");
        //}
    }
}
