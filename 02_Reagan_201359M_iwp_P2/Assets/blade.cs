using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blade : MonoBehaviour
{


    public AudioClip HitClip;
    [HideInInspector] AudioSource AS;


    void Awake()
    {
        AS = GetComponent<AudioSource>();

    }

    // Handle collisions with other objects
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Enemy")
            && collision.gameObject.GetComponent<Enemy>().immunity_timer <= 0.0f)
        {
            Debug.Log("HIT ENEMY");

            Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();
            Player playerscript = GetComponentInParent<Transform>().GetComponentInParent<Weapon>().playerscript;

            // Calculate the direction from this object to the enemy
            Vector2 directionToEnemy = 
                (collision.transform.position - playerscript.playerTransform.position).normalized;
            // Set a force to launch the object in the opposite direction
            float launchForce = .1f * Time.deltaTime; // Adjust the force as needed
            enemyScript.enemyrb.AddForce(directionToEnemy * launchForce, ForceMode2D.Impulse);

            //DAMAGE ENEMY
            int playerDamage = playerscript.damage;
            enemyScript.health -= 1000000;
            //enemyScript.health -= playerDamage;

            //SET ENEMY TO HURT STATE
            enemyScript.currentState = Enemy.EnemyState.HURT;
            enemyScript.immunity_timer = .5f;
            enemyScript.hurt_timer = 0.0f;

            Debug.Log("ENEMY HEALTH " + enemyScript.health);


            AS.clip = HitClip;
            AS.Play();
        }

    }

}
