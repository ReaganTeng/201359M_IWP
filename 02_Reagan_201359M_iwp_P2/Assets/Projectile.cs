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
        if(timer >= 2)
        {
            Destroy(gameObject);
        }
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
        //use too damage either player or enemy
        if (
            (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
            && collision.gameObject != projectilesource
            && !hitsomething)
            //&& collision.gameObject.GetComponent<Player>().immunity_timer <= 0.0f)
        {
            if (projectiletype == ProjectileType.GREEN_GEM)
            {
                collision.gameObject.GetComponent<Character>().ApplyEffect(EffectType.POISON);
            }
            if (projectiletype == ProjectileType.RED_GEM)
            {
                collision.gameObject.GetComponent<Character>().ApplyEffect(EffectType.BURN);
            }
            //Debug.Log("HIT Player");
            //DAMAGE PLAYER
            collision.gameObject.GetComponent<Character>().health -= Damage;
            //collision.gameObject.GetComponent<Player>().immunity_timer = .5f;
            hitsomething = true;

            Destroy(gameObject);
            Debug.Log($"{collision.gameObject.name} HEALTH " + collision.gameObject.GetComponent<Character>().health);
        }


        if (collision.gameObject.CompareTag("WallTilemap") && projectiletype == ProjectileType.RED_GEM)
        {
            //Explode()
        }
    }


    //THIS IS SPECIFICALLY USED FOR RED GEM
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
}
