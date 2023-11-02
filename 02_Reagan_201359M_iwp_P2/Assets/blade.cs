using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blade : MonoBehaviour
{
    

    // Handle collisions with other objects
    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("HIT ENEMY");

            // Calculate the direction from this object to the enemy
            Vector2 directionToEnemy = (collision.transform.position - transform.position).normalized;

            // Set a force to launch the object in the opposite direction
            float launchForce = 1.0f; // Adjust the force as needed
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.AddForce(-directionToEnemy * launchForce, ForceMode2D.Impulse);

        }

    }

}
