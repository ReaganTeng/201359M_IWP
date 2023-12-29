using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class collider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collision is with a specific tag or object.
        if (other.CompareTag("Player"))
        {
            // Perform actions or logic when the trigger collider collides with the specified object.
            Debug.Log("Triggered");
        }
    }
}
