using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector] public float rotationSpeed; // Speed of rotation in degrees per second
    [HideInInspector] public float rotationDuration; // Duration of rotation in seconds

    [HideInInspector] public bool isRotating;
    float rotationTimer = 0.0f;


    public GameObject handle;
    public GameObject blade;

   [HideInInspector] public Player playerscript;


   

    private void Awake()
    {
        isRotating = false;
        rotationSpeed = 360;
        rotationDuration = 1.0f;
        playerscript = GetComponentInParent<Player>();
        blade.SetActive(false);



        //gameObject.SetActive(true);
        //gameObject.SetActive(false);
    }


    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetAxis("Right Trigger") > .1f)
            && !isRotating
            && !playerscript.AIMode
            && playerscript.health > 0)
        {
           BeginAttack();
        }

        Attacking();
    }

    void BeginAttack()
    {
        Debug.Log("BEGIN ATTACK");
        isRotating = true;
        rotationTimer = 0.0f;
        blade.SetActive(true);
    }

    void Attacking()
    {
        if (isRotating)
        {
            // Rotate the weapon around the player
            handle.transform.RotateAround(transform.parent.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            // Increase the timer
            rotationTimer += Time.deltaTime;

            // Check if the rotation duration is reached
            if (rotationTimer >= rotationDuration)
            {
                // Rotation completed, stop rotating
                isRotating = false;
                // Hide or destroy the weapon GameObject
                blade.SetActive(false);
                // Or you can destroy it if needed: Destroy(gameObject);
                // Reset the rotation of the weapon holder
                transform.rotation = Quaternion.identity;
            }
        }
    }
}
