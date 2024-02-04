using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UnityEngine;
using static Item;
using static Projectile;

// Base class for player effects
public abstract class Effect
{
    public string name { get; protected set; }
    public EffectType Type { get; protected set; }
    public float Duration { get; set; }
    public bool IsExpired { get; protected set; }
    public Character TargetCharacter { get; protected set; }

    public float maxDuration;

    bool maxdurationset;

    //public Effect()
    //{
    //    maxdurationset = false;
    //    maxDuration = Duration;
    //}
    //public virtual void Awake()
    //{
    //    maxDuration = Duration;
    //}
    public virtual void UpdateEffect(float deltaTime)
    {
        
        Duration -= deltaTime;
        if (Duration < 0)
        {
            IsExpired = true;

            if(IsExpired)
            {
                Player playerScript = TargetCharacter.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.activeEffectsText.text = "";
                }
            }
            //return;
        }

        //if (!maxdurationset)
        //{
        //    maxDuration = Duration;
        //    maxdurationset = !maxdurationset;
        //}

        //Debug.Log("UPDATING EFFECT");
    }
}

// Poison effect
public class PoisonEffect : Effect
{
    private float interval;
    private int damage;
    private float intervalTimer;

    public PoisonEffect(float duration, float interval, float slowFactor, Character targetCharacter)
    {
        name = "POISON";
        intervalTimer = 0;
        Type = EffectType.POISON;
        Duration = duration;
        this.interval = interval;
        this.damage = 10; // Adjust the damage amount as needed
        TargetCharacter = targetCharacter;
        targetCharacter.speed = .6f;
    }

    public override void UpdateEffect(float deltaTime)
    {
        base.UpdateEffect(deltaTime);

        Debug.Log("EFFECT POISON " + interval);
        intervalTimer += Time.deltaTime;



        // Apply poison damage at intervals
        if (intervalTimer >= interval)
        {
            ApplyDamage();
            intervalTimer = 0;
        }

        // Apply speed reduction
        ApplySpeedReduction();
    }

    private void ApplyDamage()
    {
        TargetCharacter.health -= damage;
        Debug.Log($"Poison: Player takes {damage} damage.");
    }

    private void ApplySpeedReduction()
    {
        // Implement logic to slow down the player
        Debug.Log("Poison: Player is slowed down.");
    }
}

// Shield effect
public class ShieldEffect : Effect
{
    public ShieldEffect(float duration, Character targetCharacter)
    {
        name = "SHIELD";
        Type = EffectType.SHIELD;
        Duration = duration;
        TargetCharacter = targetCharacter;
    }



    public override void UpdateEffect(float deltaTime)
    {
        base.UpdateEffect(deltaTime);

        if(Duration <= 0)
        {
            TargetCharacter.playerShield.shieldActive = false;
        }
        else
        {
            TargetCharacter.playerShield.shieldActive = true;
        }


        // Implement logic for the shield effect
        //Debug.Log("Shield: Player is protected.");
    }
}

// Burn effect
public class BurnEffect : Effect
{
    private float interval;
    private int damage;
    private float intervalTimer;

    public BurnEffect(float duration, float interval, Character targetCharacter)
    {
        name = "BURNING";
        intervalTimer = 0;
        Type = EffectType.BURN;
        Duration = duration;
        this.interval = interval;
        damage = 15; // Adjust the damage amount as needed
        TargetCharacter = targetCharacter;
    }

    public override void UpdateEffect(float deltaTime)
    {
        base.UpdateEffect(deltaTime);

        Debug.Log("UPDATING BURN EFFECT");

        // Apply burn damage at intervals
        intervalTimer += Time.deltaTime;


        if (Duration <= 0)
        {
            TargetCharacter.speed = 1.0f;
        }

        if (intervalTimer >= interval)
        {
            ApplyDamage();
            intervalTimer = 0;
        }
    }

    private void ApplyDamage()
    {
        TargetCharacter.health -= damage;
        Debug.Log($"Burn: Player takes {damage} damage.");
    }
}




//POWERUPS
// ONE_HIT effect
public class OneHitEffect : Effect
{
    //private int meleeDamageIncrease;
    private int OriginalMeleeDamage;

    public OneHitEffect(float duration, int originalMeleeDamage, Character targetCharacter)
    {
        Debug.Log($"DAMAGE ORIGINAL {targetCharacter.meleedamage}");

        name = "POWER UP: ONE HIT";
        Type = EffectType.ONE_HIT;
        Duration = duration;
        OriginalMeleeDamage = originalMeleeDamage;
        TargetCharacter = targetCharacter;
        //originalMeleeDamage = targetCharacter.meleedamage;

        TargetCharacter.meleedamage = 1000;
    }

    public override void UpdateEffect(float deltaTime)
    {
        base.UpdateEffect(deltaTime);

        if(Duration <= 0)
        {
            TargetCharacter.meleedamage = OriginalMeleeDamage;
            Debug.Log($"DAMAGE SWITCH {TargetCharacter.meleedamage}");
        }
        // Apply melee damage increase
        //Debug.Log($"OneHit: Melee damage increased to {TargetCharacter.meleedamage}.");
    }

    //public override void OnExpired()
    //{
    //    // Revert melee damage to original value when the effect expires
    //    TargetCharacter.meleedamage = originalMeleeDamage;
    //    Debug.Log("OneHit: Melee damage reverted to original value.");
    //}
}


// SPIRIT_FIRE effect
public class SpiritFireEffect : Effect
{
    public GameObject projectilePrefab;
    //private ProjectileSpawner projectileSpawner;
    bool projectileshot;
    public SpiritFireEffect(float duration, Character targetCharacter)
    {
        name = "POWER UP: SPIRIT FIRE";
        projectileshot = false;
        //Type = effectType;
        Duration = duration;
        //this.projectileSpawner = projectileSpawner;
        TargetCharacter = targetCharacter;
    }

    public override void UpdateEffect(float deltaTime)
    {
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0)
            && !projectileshot)
        {
            //ADD ALL FOUR DIRECTION
            Vector3[] targetpos = {
            new Vector3(
                TargetCharacter.transform.position.x,
                TargetCharacter.transform.position.y + 1,
                TargetCharacter.transform.position.z)
            ,new Vector3(
                TargetCharacter.transform.position.x,
                TargetCharacter.transform.position.y - 1,
                TargetCharacter.transform.position.z)
             ,new Vector3(
                TargetCharacter.transform.position.x + 1,
                TargetCharacter.transform.position.y,
                TargetCharacter.transform.position.z)
              ,new Vector3(
                TargetCharacter.transform.position.x - 1,
                TargetCharacter.transform.position.y,
                TargetCharacter.transform.position.z)
               /*,new Vector3(
                TargetCharacter.transform.position.x - 1,
                TargetCharacter.transform.position.y - 1,
                TargetCharacter.transform.position.z)
                ,new Vector3(
                TargetCharacter.transform.position.x - 1,
                TargetCharacter.transform.position.y + 1,
                TargetCharacter.transform.position.z)
                 ,new Vector3(
                TargetCharacter.transform.position.x + 1,
                TargetCharacter.transform.position.y - 1,
                TargetCharacter.transform.position.z)
                  ,new Vector3(
                TargetCharacter.transform.position.x + 1,
                TargetCharacter.transform.position.y + 1,
                TargetCharacter.transform.position.z)*/
            };
            for (int i = 0; i < targetpos.Length; i ++)
            {
                //Quaternion rotation = Quaternion.Euler(0, 0, i);
                TargetCharacter.ShootProjectiles(
                    ProjectileType.NORMAL,
                    targetpos[i],
                    TargetCharacter.transform.position//,
                    //rotation
                );
            }
            projectileshot = true;
        }
        if (!Input.GetMouseButtonDown(0))
        {
            projectileshot = false;
        }

        base.UpdateEffect(deltaTime);

        // Implement logic to shoot projectiles at four different directions
    }

}


// GEM_WISDOM effect
public class GemWisdomEffect : Effect
{
    //private int projectileDamageIncrease;
    int originalProjectileDamage;
    //bool projectileshot;
    Player playerScript;

    public GemWisdomEffect(float duration, Character targetCharacter)
    {
        name = "POWER UP: GEM WISDOM";
        //projectileshot =false;
        Type = EffectType.GEM_WISDOM;
        Duration = duration;
        //this.projectileDamageIncrease = projectileDamageIncrease;
        TargetCharacter = targetCharacter;
        originalProjectileDamage = targetCharacter.projectileDamage;
        playerScript = TargetCharacter.GetComponentInChildren<Player>();
    }

    public override void UpdateEffect(float deltaTime)
    {
        if (playerScript == null)
        {
            return; 
        }
        base.UpdateEffect(deltaTime);

        if (Duration <= 0)
        {
            TargetCharacter.projectileDamage = originalProjectileDamage;
            Debug.Log($"DAMAGE RESET TO {TargetCharacter.projectileDamage}");
        }
        else
        {
            //CHECK THE CURRENT ITEMTYPE SELECTED IN THE SLOT
            Inventory inventory = playerScript.playerInventory;
            int currentslot = inventory.selectedSlot;
            ItemType type = inventory.slots[currentslot].itemtype;
            switch (type)
            {
                //IF ITS A RED GEM OR GREEN GEM DOUBLE IT'S DAMAGE
                case ItemType.RED_GEM:
                case ItemType.GREEN_GEM:
                    {
                        TargetCharacter.projectileDamage = originalProjectileDamage * 2;
                        Debug.Log($"DAMAGE {TargetCharacter.projectileDamage}");
                        break;
                    }
                //ELSE THE PROJECTILEDAMAGE IS NORMAL
                default:
                    {
                        TargetCharacter.projectileDamage = originalProjectileDamage;
                        Debug.Log($"DAMAGE {TargetCharacter.projectileDamage}");
                        break;
                    }
            }
             //
        }
        //


        // Apply projectile damage increase
        //TargetCharacter.projectileDamage += projectileDamageIncrease;
        //Debug.Log($"GemWisdom: Projectile damage increased to {TargetCharacter.projectileDamage}.");
    }

    //public override void OnExpired()
    //{
    //    // Revert projectile damage to original value when the effect expires
    //    TargetCharacter.projectileDamage = originalProjectileDamage;
    //    Debug.Log("GemWisdom: Projectile damage reverted to original value.");
    //}
}


// MINER_SENSE effect
public class MinerSenseEffect : Effect
{
    int originalSortOrder;
    Treasure[] treasureChests;

    public MinerSenseEffect(float duration, Character targetCharacter)
    {

        TargetCharacter = targetCharacter;
        name = "POWER UP: MINER SENSE";
        Type = EffectType.MINER_SENSE;
        Duration = duration;
        //TargetCharacter = targetCharacter;

        treasureChests = GameObject.FindObjectsOfType<Treasure>() ;

        originalSortOrder = 0;
        // Assuming that the sprite renderer is on the same GameObject as the Character
        foreach(Treasure t in treasureChests ) {
            t.sr.sortingOrder = 3;
            //TargetCharacter.spriteRenderer.sortingOrder += sortOrderIncrease;
        }
    }


    public override void UpdateEffect(float deltaTime)
    {
        base.UpdateEffect(deltaTime);

        Debug.Log(originalSortOrder);
        if(Duration <= 0)
        {
            foreach (Treasure t in treasureChests)
            {
                t.sr.sortingOrder = originalSortOrder;
                //TargetCharacter.spriteRenderer.sortingOrder += sortOrderIncrease;
            }
            Debug.Log("GEM WISDOM OVER");
        }
        // Apply projectile damage increase
        //TargetCharacter.projectileDamage += projectileDamageIncrease;
        //Debug.Log($"GemWisdom: Projectile damage increased to {TargetCharacter.projectileDamage}.");
    }

    //public override void OnExpired()
    //{
    //    // Revert sorting order when the effect expires
    //    if (TargetCharacter.spriteRenderer != null)
    //    {
    //        TargetCharacter.spriteRenderer.sortingOrder = originalSortOrder;
    //        Debug.Log("MinerSense: Sorting order reverted to original value.");
    //    }
    //}
}

// GHOST effect
public class GhostEffect : Effect
{
    //private List<Collider2D> collidersToDisable;
    Player[] playerScripts;

    public GhostEffect(float duration)
    {
        name = "POWER UP: GHOST";
        Type = EffectType.GHOST;
        Duration = duration;
        //this.collidersToDisable = collidersToDisable;
        //TargetCharacter = targetCharacter;
        playerScripts = GameObject.FindObjectsOfType<Player>() ;

    }

    // Method to ignore collisions during the initialization
    private void IgnoreCollisions()
    {
        if (playerScripts != null)
        {
            Collider2D[] wallTilemapColliders = GameObject.FindObjectsOfType<Collider2D>()
                .Where(collider => collider.gameObject.layer == LayerMask.NameToLayer("WallTilemap"))
                .ToArray();

            foreach (Player p in playerScripts)
            {
                Collider2D playerCollider = p.GetComponent<Collider2D>();

                foreach (Collider2D wallCollider in wallTilemapColliders)
                {
                    Physics2D.IgnoreCollision(playerCollider, wallCollider);
                }
            }
        }
    }


    void bringbackCollission()
    {
        if (playerScripts != null)
        {
            Collider2D[] wallTilemapColliders = GameObject.FindObjectsOfType<Collider2D>()
                .Where(collider => collider.gameObject.layer 
                == LayerMask.NameToLayer("WallTilemap"))
                .ToArray();

            foreach (Player p in playerScripts)
            {
                Collider2D playerCollider = p.GetComponent<Collider2D>();

                foreach (Collider2D wallCollider in wallTilemapColliders)
                {
                    Physics2D.IgnoreCollision(playerCollider, wallCollider, false); // Resetting collision state
                }

                Debug.Log("COLLISSION BROUGHT BACK");
            }
        }
    }


    public override void UpdateEffect(float deltaTime)
    {
        if(playerScripts == null)
        {
            return;
        }

        Duration -= deltaTime;

        IgnoreCollisions();

        // Disable colliders of game objects with "WallTileMap" tag
        if (Duration <= 0)
        {
            //bool allPlayersValid = true;
            bringbackCollission();
            foreach (Player p in playerScripts)
            {
                if (!IsPositionValid(p.transform.position))
                {
                    //Debug.Log("DEDUCTING HEALTH");
                    p.health = -100;
                    //allPlayersValid = false;
                }
            }
            IsExpired = true;
        }
        //else
        //{
        //    Player playerChosen = null;
        //    foreach (Player p in playerScripts)
        //    {
        //        if (p.activeEffects.Count > 0)
        //        {
        //            playerChosen = p;
        //            Collider2D[] col = Physics2D.OverlapCircleAll(playerChosen.transform.position, 1.0f, LayerMask.GetMask("WallTilemap"));
        //            Debug.Log($"LENGTH IS {col.Length}");
        //            break;
        //        }
        //    }
        //}

        //Debug.Log("Ghost: Colliders of WallTileMap objects disabled.");
    }


    //if the player is stuck in wall
    public bool IsPositionValid(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, .6f, LayerMask.GetMask("WallTilemap"));
        //Debug.Log($"LENGTH IS {colliders.Length}");
        if (colliders.Length > 0)
        {
            Debug.Log("Position is inside or too close to a wall. STUCK IN WALL.");
            // There is a wall tile at or near the position
            return false;
        }
        Debug.Log("Position is inside or too close to a wall. NOT STUCK IN WALL.");
        return true;
    }


    //public override void OnExpired()
    //{
    //    // Re-enable colliders when the effect expires
    //    foreach (var collider in collidersToDisable)
    //    {
    //        collider.enabled = true;
    //    }
    //    Debug.Log("Ghost: Colliders of WallTileMap objects re-enabled.");
    //}
}
