using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using static Projectile;
using static Item;
using TMPro;



[Serializable]
public class PlayerIcons
{
    public CharacterUnlockManager.CharacterType characterType;
    public Sprite spriteIcon;
}

public class Player : Character
{
    

    public enum PlayerState
    {
        FOLLOW,
        ATTACK,
        HURT
    }

    public TextMeshProUGUI pressEText;


    public TextMeshProUGUI moneyearnerd;

    public Upgrades upgrades;

    //public Slider healthbar;
    //String[] tilemaptags;

    // Get input from the player
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;

    bool effectactive;
    public Transform playerTransform;
    TileBase closestTile;
    public TileBase wallTile;
    bool attacked;
    Camera playercam;
    //float time;
    //int idx;
    //List<Vector3> listOfPositions;
    //List<List<Vector3>> listOfListofPositions;
    [HideInInspector] public bool AIMode;
    //THE PLAYER THAT IS NOT IN AI MODE
    [HideInInspector] public GameObject leadingPlayer;
    [HideInInspector] public Inventory playerInventory;

    
    //public GameObject projectilePrefab;
    //public float immunity_timer;
    public GameObject itemPrefab;

    bool useditem;
    //bool gotInput;


    //[HideInInspector]
    //public CharacterUnlockManager.CharacterType characterType;

    [HideInInspector]
    public GameObject nearestEnemy;
    [HideInInspector]
    public Weapon playerWeapon;
    [HideInInspector]
    public GameObject[] listOfEnemies;
    [HideInInspector]
    public Dictionary<ItemType, Action> itemActions = new Dictionary<ItemType, Action>();

    [HideInInspector]
    public string IDLE;
    [HideInInspector]
    public string WALK_FRONT;
    [HideInInspector]
    public string WALK_BACK;
    [HideInInspector]
    public string WALK_LEFT;
    [HideInInspector]
    public string WALK_RIGHT;
    [HideInInspector]
    public string ATTACK;
    [HideInInspector]
    public string HURT;
    [HideInInspector]
    public string DEATH;
    [HideInInspector]
    public PlayerState currentstate;
    [HideInInspector]
    public TextMeshProUGUI activeEffectsText;
    [HideInInspector]
    public Sprite icon;
    [HideInInspector]
    public PowerUpNotificationUI PowerUpNotificationUI;

    public List<PlayerIcons> playericon = new List<PlayerIcons>();


   

    protected override void Awake()
    {
        base.Awake();

        PowerUpNotificationUI = GameManager.GetComponent<MenuManager>().powerupNotificationPanel.GetComponent<PowerUpNotificationUI>();

        pressEText.enabled = false;

        activeEffectsText = GameObject.FindGameObjectWithTag("ActiveEffectsText").GetComponent<TextMeshProUGUI>(); ;
        activeEffectsText.text = "";

        IDLE = "player_idle";
        WALK_BACK = "player_WalkBack";
        WALK_FRONT = "player_WalkFront";
        WALK_LEFT = "player_WalkLeft";
        WALK_RIGHT = "player_WalkRight";
        ATTACK = "player_Attack";
        HURT = "player_Hurt";
        currentAnimState = IDLE;

        // INITIALISE THE EFFECTS AND FUNCTIONS FOR EACH ITEM
        itemActions = new Dictionary<ItemType, Action>
        {
            { ItemType.RED_GEM, () =>
            ShootProjectiles(ProjectileType.RED_GEM,
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            transform.position) },
            { ItemType.GREEN_GEM, () =>
            ShootProjectiles(ProjectileType.GREEN_GEM,
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            transform.position) },
            { ItemType.BLUE_GEM, ()
            => ApplyEffect(EffectType.SHIELD) },

            //POWERUPS
            { ItemType.ONE_HIT, ()
            => PowerUpUsed(EffectType.ONE_HIT)
            },
            { ItemType.MINER_SENSE, ()
             => PowerUpUsed(EffectType.MINER_SENSE)
            },
            { ItemType.GHOST, ()
             => PowerUpUsed(EffectType.GHOST)
            },
            { ItemType.SPIRIT_FIRE, ()
             => PowerUpUsed(EffectType.SPIRIT_FIRE) },
            { ItemType.GEM_WISDOM, ()
             => PowerUpUsed(EffectType.GEM_WISDOM) },
            //


            //EQUIPMENT
             { ItemType.BOMB, () =>
            ShootProjectiles(ProjectileType.BOMB,
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            transform.position) },
              { ItemType.POTION, () =>
               health += 99
              },
            { ItemType.BULLET, () =>
            ShootProjectiles(ProjectileType.NORMAL,
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            transform.position) },
             //
        };

        playerWeapon = GetComponentInChildren<Weapon>();
        //gotInput = false;

        PlayerPrefs.SetFloat("MoneyEarned", 0);

        projectileDamage = 15;
        projectileDamage += projectileDamage * (upgrades.DamageBuff / 100);
        meleedamage = 15;
        meleedamage += meleedamage * (upgrades.DamageBuff / 100);


        useditem = false;
        playerInventory = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<Inventory>();
        effectactive = false;
        health = 100;
        health += health * (upgrades.HealthBuff / 100);
        Debug.Log($"PLAYER HEALTH IS {health}");

        playercam = GetComponentInChildren<Camera>();
        attacked = false;
        playerTransform = transform;

        moneyearnerd = GameObject.FindGameObjectWithTag("MoneyEarnedText").GetComponent<TextMeshProUGUI>();
        moneyearnerd.text = "$0";
    }

    public void PowerUpUsed(EffectType effect)
    {
        int currentslot = playerInventory.selectedSlot;
        Sprite sprite = playerInventory.slots[currentslot].slotImage.sprite;
      
        //PowerUpNotificationUI.powerupName.text
        //                = powerupName;
        PowerUpNotificationUI.GetComponent<PowerUpNotificationUI>()
               .NotifyPowerUp(effect, sprite);
        ApplyEffect(effect);
    }

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
        PlayAnimation(currentAnimState);

        //IF SAME LAYER AS LAYER TILE
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, LayerMask.GetMask("WallTilemap"));
        //Debug.Log("COLLIDERS " + colliders.Length);

        if (disabled)
        {
            // Put any logic specific to the disabled state here, or simply return
            return;
        }
        base.Update();

        //PlayAnimation("player_Attack");

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        listOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (health <= 0)
        {
            return;
        }
        //spriteRenderer.color = Color.white;

        //CONTROLS WHAT HAPPENS IF PLAYER IF IS IN PLAYER MODE OR AI MODE
        ControlManager();
        UpdateActiveEffectsUI();
    }

    //DISPLAY WHAT ARE THE CURRENT EFFECTS THE PLAYER HAS
    void UpdateActiveEffectsUI()
    {
        // Check if there is a change in the active effects
        //if (!IsEffectListEqual(previousActiveEffects, activeEffects))
        if(activeEffects.Count > 0)
        {
            string effectName = "";
            foreach (Effect effects in activeEffects)
            {
                effectName += $"{effects.name} {(int)effects.Duration}\n";
            }
            activeEffectsText.text = effectName;
            Debug.Log("EFFECTS UPDATED");
        }
    }

   public void InteractableInteraction()
    {
        Interactables nearestInteractable = null;
        float nearestDistance = float.MaxValue;

        // Find all objects with the Interactable.cs script
        Interactables[] interactables = GameObject.FindObjectsOfType<Interactables>();

        foreach (Interactables interactable in interactables)
        {
            // Calculate the distance to the current interactable
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (interactable.gameObject.GetComponent<SpriteRenderer>() != null)
            {
                interactable.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }

            
            // Check if the distance is within the search radius and closer than the current nearest
            if (distance < 2.0f && distance < nearestDistance)
            {

                nearestInteractable = interactable;
                if (nearestInteractable.gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    nearestInteractable.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
                nearestDistance = distance;
            }
            //else
            //{
            //    pressEText.enabled = false;
            //}
        }

        // Check if a nearest interactable was found
        if (nearestInteractable != null)
        {
            pressEText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                nearestInteractable.Interact();
            }
        }
        else
        {
            pressEText.enabled = false;
        }
        //else
        //{

        //}
        //else
        //{
        //    Debug.Log("No interactables found within the search radius.");
        //}


    }


    public void ControlManager()
    {
        if (!AIMode)
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    health -= 10;
            //}
            InteractableInteraction();
            if (Input.GetKey(KeyCode.Space))
            {
                if (activeEffects.Count <= 0)
                {
                    //Debug.Log("EFFECT APPLIED");
                    ApplyEffect(EffectType.GHOST);
                    //ApplyEffect(EffectType.BURN);
                }
            }

            if (verticalInput == 0
                && horizontalInput == 0
                && !playerWeapon.isRotating)
            {
                currentAnimState = IDLE;
                //currentAnimIdx = 0;
            }

            if (playerWeapon.isRotating)
            {
                currentAnimState = ATTACK;
            }
            //emissionModule.enabled = true;
            //Debug.Log($"CURRENT HEALTH IS {health}");
            Movement();
            //cameraMovement();
            UseItem();
        }
        else
        {
            FSManager();
        }
    }


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

    void FollowPlayer()
    {
        //Vector3 targetpos = UpdateTargetingPosition();
        //Vector3 targetpos = CalculatePath();
        Vector2 playerPosition = leadingPlayer.transform.position;
        Vector2 Position = transform.position;
        Vector3 targetpos = FindPath(playerPosition, Position);

        //positonGameObject.transform.position = targetpos;

        //float speed = 5.0f;
        // Calculate the direction from the follower to the target
        Vector3 direction = targetpos - transform.position;
        // Normalize the direction vector (optional, keeps the movement consistent)
        direction.Normalize();
        // Update the position of the follower toward the target
        transform.position += direction * 6.0f * speed * Time.deltaTime;

        if (transform.position.x < leadingPlayer.transform.position.x)
        {
            currentAnimState = WALK_RIGHT;
        }
        else
        {
            currentAnimState = WALK_LEFT;
        }
        
    }
   
    

    void FSManager()
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
            //case PlayerState.ATTACK:
            {
                if (distance >= 2.0)
                {
                    FollowPlayer();
                }
                else
                {
                    currentAnimState = WALK_FRONT;
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
                        //ATTACK THE ENEMY WHEN NEAR
                        if (distanceBetweenEnemy < 2.0f)
                        {
                            currentAnimState = ATTACK;
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

    // This method is called when a collision occurs
    
    GameObject FindNearestEnemy(Vector3 position)
    {
       


        GameObject nearestEnemy = null;
        float minDistance = 10;
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
        ItemType itemchosen = ItemType.NOTHING;
        int currentslot = playerInventory.selectedSlot;

        // Check if the item type is in the dictionary, then perform the associated action
        if (itemActions.ContainsKey(playerInventory.slots[currentslot].itemtype))
        {
            itemchosen = playerInventory.slots[currentslot].itemtype;
            itemActions[playerInventory.slots[currentslot].itemtype].Invoke();
        }
        //REMOVE THE ITEM FROM THE SLOT
        playerInventory.slots[currentslot].RemoveItem();
        //

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
            currentAnimState = WALK_FRONT;
        }
        else if (verticalInput > 0)
        {
            currentAnimState = WALK_BACK;
        }

        if (horizontalInput < 0)
        {
            currentAnimState = WALK_LEFT;
        }
        else if (horizontalInput > 0)
        {
            currentAnimState = WALK_RIGHT;
        }
        //}

        // Calculate the movement vector
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f).normalized * 6.0f * speed * Time.deltaTime;
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







    //private void FindClosestTile(string[] tags)
    //{
    //    Vector3 playerPosition = playerTransform.position;
    //    Vector3 playerForward = playerTransform.forward; // Get the player's forward direction.

    //    float closestDistance = float.MaxValue;
    //    TileBase closestTile = null;

    //    foreach (string tag in tags)
    //    {
    //        GameObject[] tilemapObjects = GameObject.FindGameObjectsWithTag(tag);

    //        foreach (GameObject tilemapObject in tilemapObjects)
    //        {
    //            Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();

    //            foreach (Vector3Int cellPosition in tilemap.cellBounds.allPositionsWithin)
    //            {
    //                TileBase tile = tilemap.GetTile(cellPosition);

    //                if (tile != null)
    //                {
    //                    Vector3 tileCenter = tilemap.GetCellCenterWorld(cellPosition);
    //                    float distance = Vector3.Distance(playerPosition, tileCenter);

    //                    // Calculate the direction from the player to the tile.
    //                    Vector3 tileDirection = (tileCenter - playerPosition).normalized;

    //                    // Calculate the dot product between the player's forward direction and the tile direction.
    //                    float dotProduct = Vector3.Dot(playerForward, tileDirection);

    //                    // You can adjust the threshold for what is considered the "front" of the player.
    //                    // A dot product close to 1 means the tile is in front of the player.
    //                    float directionThreshold = 0.9f;

    //                    if (distance < closestDistance && dotProduct > directionThreshold)
    //                    {
    //                        closestDistance = distance;
    //                        closestTile = tile;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    if (closestTile != null)
    //    {
    //        Debug.Log("Closest Tile: " + closestTile.name);
    //    }
    //    //else
    //    //{
    //    //    Debug.Log("No tiles found with the specified tags in the specified Tilemaps.");
    //    //}
    //}


}






