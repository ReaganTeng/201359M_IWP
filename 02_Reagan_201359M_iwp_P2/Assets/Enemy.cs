using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Item;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    String[] tilemaptags;

    QuestManager questManager;

    TileBase closestTile;
    public TileBase wallTile;

    bool attacked;

    float time;

    public float attackcooldown;
    int idx;
    List<Vector3> listOfPositions;
    //List<List<Vector3>> listOfListofPositions;


    protected GameObject[] playerlist;

    protected GameObject player;
    Vector3 playerpos;

    protected float distance;
    public float stoppingdistance;
    public float distanceToIdle;


    protected Animator animator;


    GameObject positonGameObject;
    float raycastDistance = 10.0f;
    int raycastDirections = 100;
    //LayerMask wallLayerMask = LayerMask.GetMask("WallTilemap");  // Define the WallTilemap layer mask


    public GameObject itemPrefab;

    //START TO INTEGRATE ENEMIES INTO THE MAZE

    List<Vector3> potentialDestinations = new List<Vector3>();



    public GameObject positionGO;

    public float immunity_timer;
    public float hurt_timer;

    public Rigidbody2D enemyrb;

    EnemyManager enemymgt;

    protected override void Awake()
    {
        base.Awake();


        questManager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<QuestManager>();

        attackcooldown = 0;
        enemyrb = GetComponent<Rigidbody2D>();
        damage = 15;
        hurt_timer = 0;
        immunity_timer = 0;
        idx = 0;
        listOfPositions = new List<Vector3>();
        //health = UnityEngine.Random.Range(100, 200);
        health = 100;

        //Debug.Log("HEALTH SET " + health);
        attacked = false;



        playerlist = GameObject.FindGameObjectsWithTag("Player");
        
        for(int i = 0; i< playerlist.Length; i++)
        {
            if (!playerlist[i].GetComponent<Player>().AIMode)
            {
                player = playerlist[i];
                break;
            }
        }


        enemymgt = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<EnemyManager>();
        //transform.position = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().startingposition;
        //Debug.Log("TRANSFORM POSITION " + GameObject.Find("MapGenerator").GetComponent<MapGenerator>().startingposition);
        //raycastOrigin = transform;

        listOfPositions.Add(player.transform.position);
        animator = GetComponent<Animator>();
        //FOR POSITION TESTING
        if (positonGameObject == null)
        {
            positonGameObject = Instantiate(positionGO, transform.position, Quaternion.identity);
            Color color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            positonGameObject.GetComponent<SpriteRenderer>().color = Color.clear;
            //GetComponent<SpriteRenderer>().color = color;
        }
        //
        //TAGS OF ROOM
        tilemaptags = new string[] { "WallTilemap", "FloorTilemap" };
    }

    protected void FollowPlayer()
    {
        //Vector3 targetpos = UpdateTargetingPosition();
        //Vector3 targetpos = CalculatePath();
        Vector3 targetpos = FindPath();
       positonGameObject.transform.position = targetpos;



        float speed = 2.0f;
        // Calculate the direction from the follower to the target
        Vector3 direction = targetpos - transform.position;
        // Normalize the direction vector (optional, keeps the movement consistent)
        direction.Normalize();
        // Update the position of the follower toward the target
        transform.position += direction * speed * Time.deltaTime;

        //float distance = Vector2.Distance(transform.position, targetpos);

        //if (distance <= 0.5f)
        //{
        //    listOfPositions.Remove(targetpos);
        //}

    }
    Vector3 FindPath()
    {
        Vector2 playerPosition = playerpos;
        Vector2 enemyPosition = transform.position;
        Vector2 directionToPlayer = playerPosition - enemyPosition;
        directionToPlayer.Normalize();

        //DRAW RAYS
        for (float angle = 0; angle < 360; angle += 1)
        {
            float x = 1.0f;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            direction.Normalize();
            Debug.DrawRay(enemyPosition, direction * raycastDistance * x, Color.red);
            Debug.DrawRay(playerPosition, direction * raycastDistance * x, Color.magenta);
        }
        Debug.DrawRay(enemyPosition, directionToPlayer * raycastDistance, Color.blue);
        //

        RaycastHit2D hitwall = Physics2D.Raycast(enemyPosition, directionToPlayer, raycastDistance, LayerMask.GetMask("WallTilemap"));

        if (hitwall.collider != null)
        {
            // Scenario 1: There is an obstacle between the enemy and the player
            // Initialize variables for finding the closest intersection
            List<Vector3> lineOfSightPoints = new List<Vector3>();
            float minDistance = float.MaxValue;
            Vector3 closestIntersection = Vector3.zero;

            for (float angle = 0; angle < 360; angle+= 1)
            {
                Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                Vector2 endPoint = enemyPosition + direction * raycastDistance;

                List<Vector3> pointsInDirection = BresenhamLine(enemyPosition, endPoint);
                lineOfSightPoints.AddRange(pointsInDirection);
            }

            
    //        foreach (var point in lineOfSightPoints)
    //        {
    ////if(!Physics2D.Linecast(enemyPosition, point, LayerMask.GetMask("WallTilemap")) &&
    ////!Physics2D.Linecast(playerPosition, point, LayerMask.GetMask("WallTilemap"))

    //            // Check if this path is less obstructed by the "WallTilemap" layer
    //            bool enemyLinecastClear = !Physics2D.Linecast(enemyPosition, point, LayerMask.GetMask("WallTilemap"));
    //            bool playerLinecastClear = !Physics2D.Linecast(playerPosition, point, LayerMask.GetMask("WallTilemap"));

    //            if (enemyLinecastClear 
    //                && playerLinecastClear)
    //            {
    //                float distance = Vector3.Distance(playerPosition, point);
    //                if (distance < minDistance)
    //                {
    //                    closestIntersection = point;
    //                    minDistance = distance;
    //                }
    //            }
    //        }


            foreach (var point in lineOfSightPoints)
            {
                bool enemyLinecastClear = !Physics2D.Linecast(enemyPosition, point, LayerMask.GetMask("WallTilemap"));
                bool playerLinecastClear = !Physics2D.Linecast(playerPosition, point, LayerMask.GetMask("WallTilemap"));

                // Filter out points that are too close to the enemy or player
                float minDistanceFromEntities = 0.1f; // Adjust this value as needed
                if (enemyLinecastClear && playerLinecastClear &&
                    Vector3.Distance(point, enemyPosition) > minDistanceFromEntities &&
                    Vector3.Distance(point, playerPosition) > minDistanceFromEntities)
                {
                    float distance = Vector3.Distance(playerPosition, point);
                    if (distance < minDistance)
                    {
                        closestIntersection = point;
                        minDistance = distance;
                    }
                }
            }


            lineOfSightPoints.Clear();

            //if (minDistance != float.MaxValue)
            //{
            //    Debug.Log("MINDIST GOT SOMETHING");
            //}

            //if (minDistance != float.MaxValue)
            //{
            return closestIntersection;
            //}
        }
        else
        {
            //Debug.Log("THERE'S NO OBSTACLE");
            //Debug.Log("PLAYER'S POSITION " + playerPosition);
            // Scenario 2: There is no obstacle between the enemy and the player
            return playerPosition;
        }
    }
    public List<Vector3> BresenhamLine(Vector2 start, Vector2 end)
    {
        List<Vector3> linePoints = new List<Vector3>();

        int x0 = Mathf.RoundToInt(start.x);
        int y0 = Mathf.RoundToInt(start.y);
        int x1 = Mathf.RoundToInt(end.x);
        int y1 = Mathf.RoundToInt(end.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            linePoints.Add(new Vector3(x0, y0));

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err = err - dy;
                x0 = x0 + sx;
            }
            if (e2 < dx)
            {
                err = err + dx;
                y0 = y0 + sy;
            }
        }

        return linePoints;
    }

    //int CountWallHits(Vector3 start, Vector3 end)
    //{
    //    // Count how many obstacles were hit by a ray from start to end
    //    int wallHitCount = 0;
    //    RaycastHit[] hits = Physics.RaycastAll(start, end - start, Vector3.Distance(start, end), LayerMask.GetMask("WallTilemap"));
    //    foreach (RaycastHit hit in hits)
    //    {
    //        if (hit.transform.CompareTag("WallTilemap"))
    //        {
    //            wallHitCount++;
    //        }
    //    }
    //    return wallHitCount;
    //}
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
            //Debug.Log("USE LAST POS");
            //Debug.Log(listOfPositions[count].ToString());
            return listOfPositions[count];
        }
        else
        {
            //Debug.Log("USE PLAYER POS");
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

    protected override void Update()
    {
        base.Update();


        for (int i = 0; i < playerlist.Length; i++)
        {
            if (!playerlist[i].GetComponent<Player>().AIMode)
            {
                player = playerlist[i];
                break;
            }
        }

        //if (!GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AIMode)
        //{
        //    player = GameObject.FindGameObjectWithTag("Player");
        //}


        //Debug.Log("REFERENCING FROM");
        if (player != null)
        {
            playerpos = player.transform.position;
        }
        //THE CURRENT DISTANCE BETWEEN PLAYER AND ENEMY
        distance = Vector2.Distance(playerpos, transform.position);

        if(health <= 0)
        {
            //DEATH
            Die();
        }


        //WHEN IMMUNITY_TIMER IS >= 0, ENEMY CANNOT BE HURT
        if (immunity_timer >= 0)
        {
            immunity_timer -= Time.deltaTime;
        }
        if (immunity_timer < .3)
        {
            enemyrb.velocity = Vector2.zero;
        }

        if(attackcooldown >= 0)
        {
            attackcooldown -= Time.deltaTime;
        }



        if (distance >= distanceToIdle)
        {
            if (//currentState != EnemyState.IDLE
            //    && currentState != EnemyState.ABOUT_TO_ATTACK
            //    && currentState != EnemyState.ATTACK
            //    && currentState != EnemyState.HURT
                currentState == EnemyState.CHASE
                )
            {
                //Debug.Log("SET TO IDLE");
                currentState = EnemyState.IDLE;
            }
        }
        else
        {
            if (currentState == EnemyState.IDLE)
            {
                //Debug.Log("SET TO CHASE");
                currentState = EnemyState.CHASE;
            }
        }

        //// Implement state-specific behavior in derived classes
        switch (currentState)
        {
            case EnemyState.IDLE:
                spriteRenderer.color = Color.white;
                break;
            case EnemyState.CHASE:
                spriteRenderer.color = Color.yellow;
                if (distance >= stoppingdistance)
                {
                    FollowPlayer();
                }
                break;
            case EnemyState.HURT:
                {
                    Debug.Log("OUCH");
                    spriteRenderer.color = Color.black;
                    hurt_timer += 1 * Time.deltaTime;
                    if (hurt_timer >= 1.0f)
                    {
                        hurt_timer = 0;
                        currentState = EnemyState.IDLE;
                    }
                    break;
                }
        }
    }

    void Die()
    {
        //drop 10 random items
        for(int i = 0; i < 10; i++)
        {
            int enumLength = Enum.GetValues(typeof(ItemType)).Length - 1;
            ItemType itemchosen = (ItemType)(Random.Range(0, enumLength));
            
            GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            item.GetComponent<Item>().SetItem(itemchosen, 99);
        }

        enemymgt.enemyList.Remove(gameObject);

        questManager.UpdateQuestProgress("Killing Monster");
        Destroy(gameObject);

        //THINGS TO ADD - DYING ANIMATION
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


//Vector3 CalculatePath()
//{
//    Vector3 enemypos = transform.position;

//    Vector3 direction = playerpos - enemypos;
//    RaycastHit2D hitPlayer = Physics2D.Raycast(playerpos, -direction, direction.magnitude);
//    RaycastHit2D hitEnemy = Physics2D.Raycast(enemypos, direction, direction.magnitude, LayerMask.GetMask("WallTilemap"));

//    Vector3 positionWithSightToPlayer = hitPlayer.collider != null ? hitPlayer.point : Vector3.zero;
//    Vector3 positionWithSightToEnemy = hitEnemy.collider != null ? hitEnemy.point : Vector3.zero;

//    float distanceToPlayer = Vector3.Distance(playerpos, positionWithSightToPlayer);
//    float distanceToEnemy = Vector3.Distance(playerpos, positionWithSightToEnemy);

//    // Check if both positions have line of sight and choose the closer one
//    if (hitPlayer.collider != null && hitEnemy.collider != null)
//    {
//        if (distanceToPlayer < distanceToEnemy)
//        {
//            return positionWithSightToPlayer;
//        }
//        else
//        {
//            return positionWithSightToEnemy;
//        }
//    }
//    else if (hitPlayer.collider != null)
//    {
//        return positionWithSightToPlayer;
//    }
//    else if (hitEnemy.collider != null)
//    {
//        return positionWithSightToEnemy;
//    }
//    else
//    {
//        return playerpos; // No line of sight, move directly towards the player
//    }
//}






//private float CalculateBlockedPercentage(RaycastHit2D[] hits1, RaycastHit2D[] hits2)
//{
//    // Calculate the percentage of blocked rays
//    int totalRays = hits1.Length + hits2.Length;
//    int blockedRays = 0;

//    foreach (var hit in hits1)
//    {
//        if (hit.collider.CompareTag("WallTilemap"))
//        {
//            blockedRays++;
//        }
//    }

//    foreach (var hit in hits2)
//    {
//        if (hit.collider.CompareTag("WallTilemap"))
//        {
//            blockedRays++;
//        }
//    }

//    return (float)blockedRays / totalRays;
//}

//private Vector2 CalculateIntersectionPoint(RaycastHit2D[] hits1, RaycastHit2D[] hits2)
//{
//    // Calculate the average intersection point
//    Vector2 intersectionPoint = Vector2.zero;

//    foreach (var hit in hits1)
//    {
//        intersectionPoint += hit.point;
//    }

//    foreach (var hit in hits2)
//    {
//        intersectionPoint += hit.point;
//    }

//    intersectionPoint /= (hits1.Length + hits2.Length);

//    return intersectionPoint;
//}