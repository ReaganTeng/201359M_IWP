using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
//using static UnityEditor.Experimental.GraphView.GraphView;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Vector2 = UnityEngine.Vector2;
using static Projectile;
using static Item;
using TMPro;

public class Player : Character
{
    //public enum CharacterType
    //{
    //    JOE,
    //    PROFESSOR,
    //    VETERAN,
    //}

    public enum PlayerState
    {
        FOLLOW,
        ATTACK,
        HURT
    }

    public TextMeshProUGUI moneyearnerd;


    //public Slider healthbar;
    String[] tilemaptags;

    // Get input from the player
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;

    bool effectactive;
    public Transform playerTransform;
    TileBase closestTile;
    public TileBase wallTile;
    bool attacked;
    Camera playercam;
    float time;
    int idx;
    List<Vector3> listOfPositions;
    List<List<Vector3>> listOfListofPositions;
    [HideInInspector] public bool AIMode;
    public Image icon;
    //THE PLAYER THAT IS NOT IN AI MODE
    [HideInInspector] public GameObject leadingPlayer;
    Inventory playerInventory;

    public GameObject projectilePrefab;
    //public float immunity_timer;
    public GameObject itemPrefab;

    bool useditem;
    bool gotInput;


    //[HideInInspector]
    public CharacterUnlockManager.CharacterType characterType;

    [HideInInspector]
    public GameObject nearestEnemy;
    [HideInInspector]
    public Weapon playerWeapon;
    [HideInInspector]
    public GameObject[] listOfEnemies;

    protected override void Awake()
    {
        base.Awake();

        playerWeapon = GetComponentInChildren<Weapon>();
        gotInput = false;

        PlayerPrefs.SetFloat("MoneyEarned", 0);

        projectileDamage = 15;
        meleedamage = 15;

        useditem = false;
        playerInventory = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<Inventory>();
        icon = GetComponent<Image>();
        effectactive = false;
        health = 100 + PlayerPrefs.GetInt("HealthUpgradePercentage");
        //Debug.Log($"PLAYER HEALTH {health}");

        idx = 0;
        listOfPositions = new List<Vector3>();
        listOfListofPositions = new List<List<Vector3>>();
        playercam = GetComponentInChildren<Camera>();
        attacked = false;
        playerTransform = transform;

        moneyearnerd = GameObject.FindGameObjectWithTag("MoneyEarnedText").GetComponent<TextMeshProUGUI>();
    }



    public GameObject objectToSpawn;
    public float spawnRadius = 5f;
    public LayerMask occupiedLayer;



    void SpawnRandomObject()
    {
        //Vector2 randomPosition;
        //Collider2D[] colliders;

        //do
        //{
        //    // Generate a random position within the specified spawn radius
        //    randomPosition = new Vector2(
        //        transform.position.x + Random.Range(-spawnRadius, spawnRadius),
        //        transform.position.y + Random.Range(-spawnRadius, spawnRadius)
        //    );

        //    // Check if the random position is occupied by any colliders on the specified layer
        //    colliders = Physics2D.OverlapCircleAll(randomPosition, 0.1f, occupiedLayer);

        //} while (colliders.Length > 0);

        //// Instantiate the object at the non-occupied random position
        //Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
    }


    protected override void Update()
    {

        base.Update();

        //IF SAME LAYER AS LAYER TILE
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, LayerMask.GetMask("WallTilemap"));
        //Debug.Log("COLLIDERS " + colliders.Length);

        if (disabled)
        {
            // Put any logic specific to the disabled state here, or simply return
            return;
        }



        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        listOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");


        if (health > 0)
        {
            spriteRenderer.color = Color.white;

            
            if (!AIMode)
            {
                //if (Input.GetKeyDown(KeyCode.Space))
                //{
                //    health -= 10;
                //}

                if (verticalInput == 0
                    && horizontalInput == 0
                    && !playerWeapon.isRotating)
                {
                    currentAnimIdx = 0;
                }


                //emissionModule.enabled = true;
                //Debug.Log($"CURRENT HEALTH IS {health}");
                Movement();
                cameraMovement();
                UseItem();
            }
            else
            {
                AIMovement();
            }
        }
        //else
        //{
        //    spriteRenderer.color = Color.black;
        //}

        PlayAnimation(characterType, currentAnimIdx);



        //EFFECT TESTING
        //if (Input.GetKey(KeyCode.Space)
        //    && !effectactive)
        //{
        //    effectactive = true;
        //}

        //ApplyStatusEffect(StatusEffectType.POISON, 10);
        //ApplyPoisonEffectToPlayer(10);

        //WHEN IMMUNITY_TIMER IS >= 0, PLAYER CANNOT BE HURT
        //if (immunity_timer >= 0)
        //{
        //    if (immunity_timer < .3)
        //    {
        //        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //        rb.velocity = Vector2.zero;
        //    }

        //    immunity_timer -= Time.deltaTime;
        //}


        //Attack();

        //FindClosestTile(tilemaptags);
    }


    public PlayerState currentstate;


    //HANDLES THE USAGE OF ITEMS
    void UseItem()
    {
        int currentslot = playerInventory.selectedSlot;

        //switch ()

        if (Input.GetMouseButtonDown(1)
            && !useditem
            && playerInventory.slots[currentslot].Quantity > 0
            && playerInventory.slots[currentslot].itemtype != ItemType.NOTHING)
        {
            ItemUsage();
            useditem = true;
        }
        if (!Input.GetMouseButtonDown(1))
        {
            useditem = false;
        }
    }

    protected void FollowPlayer()
    {
        //Vector3 targetpos = UpdateTargetingPosition();
        //Vector3 targetpos = CalculatePath();
        Vector3 targetpos = FindPath();
        //positonGameObject.transform.position = targetpos;

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
        Vector2 playerPosition = leadingPlayer.transform.position;
        Vector2 enemyPosition = transform.position;
        Vector2 directionToPlayer = playerPosition - enemyPosition;
        directionToPlayer.Normalize();
        float raycastDistance = 10.0f;

        //DRAW RAYS
        for (float angle = 0; angle < 360; angle += 1)
        {
            float x = 1.0f;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            direction.Normalize();
            //Debug.DrawRay(enemyPosition, direction * raycastDistance * x, Color.red);
            //Debug.DrawRay(playerPosition, direction * raycastDistance * x, Color.magenta);
        }
        //Debug.DrawRay(enemyPosition, directionToPlayer * raycastDistance, Color.blue);
        //

        RaycastHit2D hitwall = Physics2D.Raycast(enemyPosition, directionToPlayer, raycastDistance, LayerMask.GetMask("WallTilemap"));

        if (hitwall.collider != null)
        {
            // Scenario 1: There is an obstacle between the enemy and the player
            // Initialize variables for finding the closest intersection
            List<Vector3> lineOfSightPoints = new List<Vector3>();
            float minDistance = float.MaxValue;
            Vector3 closestIntersection = Vector3.zero;

            for (float angle = 0; angle < 360; angle += 1)
            {
                Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                Vector2 endPoint = enemyPosition + direction * raycastDistance;

                List<Vector3> pointsInDirection = BresenhamLine(enemyPosition, endPoint);
                lineOfSightPoints.AddRange(pointsInDirection);
            }

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
            return closestIntersection;
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




    private void AIMovement()
    {

        float distance = Vector3.Distance(transform.position, leadingPlayer.transform.position);
        if(distance >= 10)
        {
            currentstate = PlayerState.FOLLOW;
        }
        //public enum PlayerState
        //{
        //    FOLLOW,
        //    ATTACK,
        //    HURT
        //}

        switch (currentstate)
        {
            case PlayerState.FOLLOW:
            {
                if (distance >= 5)
                {
                    FollowPlayer();
                }
                nearestEnemy = FindNearestEnemy(transform.position);
                break;
            }
            case PlayerState.ATTACK:
            {
                //FOLLOW THE NEAREST ENEMY
                if (nearestEnemy != null)
                {
                    float distanceBetweenEnemy = Vector2.Distance(nearestEnemy.transform.position, transform.position);
                    if (distanceBetweenEnemy < 2.0f)
                    {
                        Enemy enemyScript = nearestEnemy.gameObject.GetComponent<Enemy>();
                        // Calculate the direction from this object to the enemy
                        Vector2 directionToEnemy =
                            (nearestEnemy.transform.position - transform.position).normalized;
                        // Set a force to launch the object in the opposite direction
                        float launchForce = .1f * Time.deltaTime; // Adjust the force as needed
                        enemyScript.enemyrb.AddForce(directionToEnemy * launchForce, ForceMode2D.Impulse);
                        //DAMAGE ENEMY
                        int playerDamage = meleedamage;
                        //enemyScript.health -= 1000000;
                        enemyScript.health -= playerDamage;
                        //SET ENEMY TO HURT STATE
                        enemyScript.currentState = Enemy.EnemyState.HURT;
                        enemyScript.immunity_timer = .5f;
                        enemyScript.hurt_timer = 0.0f;
                        currentstate = PlayerState.FOLLOW;
                    }
                    else
                    {
                        MoveTowardsNearestEnemy();
                    }
                }

                break;
            }
            case PlayerState.HURT:
            {
                break;
            }
            default:
            {
                break;
            }
        }
    }

    void MoveTowardsNearestEnemy()
    {
        // Calculate the direction towards the nearest enemy
        Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
        // Move the player in that direction
        transform.Translate(direction * 10 * Time.deltaTime);
    }

    GameObject FindNearestEnemy(Vector3 position)
    {
        GameObject nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (GameObject enemy in listOfEnemies)
        {
            // Calculate the distance between the position and the enemy's position
            float distance = Vector3.Distance(position, enemy.transform.position);

            // Check if the current enemy is closer than the previous nearest enemy
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    //public void ApplyPoisonEffectToPlayer(float duration)
    //{
    //    ApplyEffect(new StatusEffect(StatusEffectType.POISON, duration, 1));
    //}

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

    
    

    //CHOOSE WHICH ITEM TO USE
    void ItemUsage()
    {
        //FOR THE SUBTRACTION OF MONEY
        ItemType itemchosen = ItemType.NOTHING;
        int currentslot = playerInventory.selectedSlot;
        ProjectileType pt = ProjectileType.NORMAL;

        switch (playerInventory.slots[currentslot].itemtype)
        {
            case ItemType.RED_GEM:
                {
                    pt = ProjectileType.RED_GEM;
                    itemchosen = ItemType.RED_GEM;
                    projectilePrefab.GetComponent<Projectile>().projectiletype = pt;
                    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    if (projectile != null)
                    {
                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector3 direction = (mousePosition - transform.position).normalized;
                        projectile.GetComponent<Projectile>().setdata(projectileDamage, 10,
                            direction, gameObject);
                    }
                    break;
                }
            case ItemType.GREEN_GEM:
                {
                    pt = ProjectileType.GREEN_GEM;
                    itemchosen = ItemType.GREEN_GEM;
                    projectilePrefab.GetComponent<Projectile>().projectiletype = pt;
                    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    if (projectile != null)
                    {
                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector3 direction = (mousePosition - transform.position).normalized;
                        projectile.GetComponent<Projectile>().setdata(projectileDamage, 10,
                            direction, gameObject);
                    }
                    break;
                }
            case ItemType.BLUE_GEM:
                {
                    itemchosen = ItemType.BLUE_GEM;
                    ApplyEffect(EffectType.SHIELD);
                    break;
                }
            default:
                {
                    break;
                }
        }
        playerInventory.slots[currentslot].RemoveItem();

        //USE FOR SUBTRACTION OF MONEY
        GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        item.GetComponent<Item>().SetItem(itemchosen, 5);
        PlayerPrefs.SetFloat("MoneyEarned", PlayerPrefs.GetFloat("MoneyEarned") - item.GetComponent<Item>().money);

        if(PlayerPrefs.GetFloat("MoneyEarned") < 0)
        {
            PlayerPrefs.SetFloat("MoneyEarned", 0);
        }

        if (moneyearnerd != null)
        {
            moneyearnerd.text = $"{PlayerPrefs.GetFloat("MoneyEarned")}";
        }
        Destroy(item);
        //


    }


    private void Movement()
    {
        

        //if (currentAnimIdx <= 0)
        //{
        if (verticalInput < 0)
        {
            currentAnimIdx = 1;
        }
        else if (verticalInput > 0)
        {
            currentAnimIdx = 2;
        }

        if (horizontalInput < 0)
        {
            currentAnimIdx = 3;
        }
        else if (horizontalInput > 0)
        {
            currentAnimIdx = 4;
        }
        //}

        

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







    //private void Attack()
    //{
    //    if (Input.GetKey(KeyCode.Space))
    //    {
    //        if (!attacked)
    //        {
    //            Debug.Log("Attack ");
    //            //Explode(transform.position, 1);
    //            attacked = true;
    //        }
    //    }
    //    else
    //    {
    //        //Debug.Log("NOT ATTACK");
    //        attacked = false;
    //    }
    //}

    //private void UpdateTargetingPosition()
    //{
    //    time += Time.deltaTime;

    //    //if(time >= 3)
    //    //{
    //    //    if (listOfPositions[idx] != null)
    //    //    {
    //    //        listOfListofPositions[idx] = transform.position;
    //    //    }
    //    //    else
    //    //    {
    //    //        listOfListofPositions.Add(transform.position);
    //    //    }

    //    //    if(idx >= 5)
    //    //    {
    //    //        idx = 0;
    //    //    }
    //    //    time = 0;
    //    //}
    //}







    //public void Explode(Vector3 explosionPosition, float explosionRadius)
    //{
    //    // Find the GameObject with the "WallTilemap" tag.
    //    GameObject wallTilemapObject = GameObject.FindWithTag("WallTilemap");

    //    // Check if the GameObject with the tag was found.
    //    if (wallTilemapObject != null)
    //    {
    //        // Get the Tilemap component from the found GameObject.
    //        Tilemap wallTilemap = wallTilemapObject.GetComponent<Tilemap>();

    //        if (wallTilemap != null)
    //        {
    //            // Convert world position to tilemap cell position.
    //            Vector3Int cellPosition = wallTilemap.WorldToCell(explosionPosition);

    //            // Loop through tiles within the explosion radius.
    //            for (int x = -Mathf.FloorToInt(explosionRadius); x <= Mathf.FloorToInt(explosionRadius); x++)
    //            {
    //                for (int y = -Mathf.FloorToInt(explosionRadius); y <= Mathf.FloorToInt(explosionRadius); y++)
    //                {
    //                    // Calculate the position of the current cell.
    //                    Vector3Int currentCell = cellPosition + new Vector3Int(x, y, 0);

    //                    // Check if the cell contains a wall tile.
    //                    TileBase tile = wallTilemap.GetTile(currentCell);

    //                    // If it does, remove the wall tile.
    //                    if (tile != null)
    //                    {
    //                        wallTilemap.SetTile(currentCell, null);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //}

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






