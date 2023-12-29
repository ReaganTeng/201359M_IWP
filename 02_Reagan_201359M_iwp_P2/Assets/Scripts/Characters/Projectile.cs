using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Projectile : MonoBehaviour
{
    //public static Projectile Instance;

    public enum ProjectileType
    {
        GREEN_GEM,
        RED_GEM,
        NORMAL
    }

    public ProjectileType projectiletype;

    SpriteRenderer projectileSprite;

    public List<Sprite> sprites = new List<Sprite>();

    GameObject projectilesource;

    int Damage = 0;
    GameObject player;
    Vector3 direction;
    float projectilespeed = 0;
    float timer;

    bool hitsomething;

    private void Awake()
    {
        projectileSprite = GetComponent<SpriteRenderer>();
        hitsomething = false;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void setdata(int damage, float speed, Vector3 dir, GameObject source)
    {
        Damage = damage;
        projectilespeed = speed;
        direction = dir;
        projectilesource = source;
        projectileSprite.sprite = sprites[(int)projectiletype];

    }

    //DIRECTION WHEN ENEMY SHOOTS
    //(player.transform.position - transform.position).normalized


    //DIRECTION WHEN PLAYER SHOOTS
    //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //Vector3 direction = (mousePosition - transform.position).normalized;
    //(player.transform.position - transform.position).normalized

    private void Update()
    {
        transform.position += direction * projectilespeed * Time.deltaTime;
        timer += 1 * Time.deltaTime;
        //if (timer >= 2)
        //{
        //    Destroy(gameObject);
        //}
    }


    //SHOOT PROJECTILE BASED ON MOUSE'S DIRECTION
    //void ShootProjectile()
    //{
    //    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    Vector3 direction = (mousePosition - transform.position).normalized;

    //    // Instantiate the projectile at the player's position.
    //    GameObject projectile = Instantiate(gameObject, transform.position, Quaternion.identity);

    //    // Set the projectile's direction.
    //    projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
    //}



    // Handle collisions with other objects
    void OnTriggerEnter2D(Collider2D collision)
    {
        //damage either player or enemy
        if (
            (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
            && collision.gameObject.tag != projectilesource.tag
            && !hitsomething)
        //&& collision.gameObject.GetComponent<Player>().immunity_timer <= 0.0f)
        {
            Character collisionCharacter = collision.gameObject.GetComponent<Character>();

            if (projectiletype == ProjectileType.GREEN_GEM)
            {
                projectileSprite.color = Color.white;
                collisionCharacter.ApplyEffect(EffectType.POISON);
            }
            if (projectiletype == ProjectileType.RED_GEM)
            {
                projectileSprite.color = Color.white;
                collisionCharacter.ApplyEffect(EffectType.BURN);
            }
            //Debug.Log("HIT Player");
            //DAMAGE PLAYER
            //collision.gameObject.GetComponent<Player>().immunity_timer = .5f;

            //SHIELD
            if (collisionCharacter.playerShield != null
                && collisionCharacter.playerShield.shieldActive)
            {
                for (int i = 0; i < collisionCharacter.activeEffects.Count; i++)
                {
                    if(collisionCharacter.activeEffects[i].Type == EffectType.SHIELD)
                    {
                        //collisionCharacter.activeEffects[i].
                        collisionCharacter.activeEffects.Remove(collisionCharacter.activeEffects[i]);
                    }
                }
                collisionCharacter.playerShield.shieldActive = false;
                //collisionCharacter.activeEffects
            }
            else
            {
                if (collisionCharacter.audioSource != null)
                {
                    collisionCharacter.audioSource.clip = collisionCharacter.audioclips[0];
                    collisionCharacter.audioSource.Play();
                }
                collisionCharacter.health -= Damage;
            }
            hitsomething = true;

            Destroy(gameObject);
            Debug.Log($"{collision.gameObject.name} HEALTH " + collision.gameObject.GetComponent<Character>().health);
        }

        //FOR THE EXPLOSION OF RED PROJECTILE
        if ((collision.CompareTag("WallTilemap") || collision.CompareTag("UnbreakableWall"))
            && projectiletype == ProjectileType.RED_GEM)
        {
            Explode(transform.position, 3, collision.gameObject);
            //direction = new Vector3(0, 0, 0);
            Destroy(gameObject);
        }

    }


    //THIS IS SPECIFICALLY USED FOR RED GEM
    public void Explode(Vector3 explosionPosition, float explosionRadius, GameObject wallTilemapObject)
    {
        // Find the GameObject with the "WallTilemap" tag.
        //GameObject wallTilemapObject = GameObject.FindWithTag("WallTilemap");

        // Check if the GameObject with the tag was found.
        if (wallTilemapObject != null
            && wallTilemapObject.CompareTag("WallTilemap"))
        {
            //Debug.Log("FOUND OBJECT");
            // Get the Tilemap component from the found GameObject.
            Tilemap wallTilemap = wallTilemapObject.GetComponent<Tilemap>();

            if (wallTilemap != null)
            {
                //Debug.Log("BEGIN ERASING");
                // Convert world position to tilemap cell position.
                Vector3Int cellPosition = wallTilemap.WorldToCell(
                    new Vector3(explosionPosition.x, explosionPosition.y, 0)
                    );
                Debug.Log($"CELL POS {cellPosition}");
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
                            Debug.Log("ERASING");
                            wallTilemap.SetTile(currentCell, null);
                        }
                        //else
                        //{
                        //    Debug.Log("TILE IS NULL");
                        //}
                    }
                }
            }
        }

    }
}
