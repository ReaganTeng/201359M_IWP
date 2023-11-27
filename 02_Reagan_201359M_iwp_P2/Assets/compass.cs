using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class compass : MonoBehaviour
{
    public Transform target; // The target the arrow should point to
    Image arrowImage; // The image representing the arrow on the compass
    public Transform player; // The target the arrow should point to

    private void Awake()
    {
        arrowImage = GetComponent<Image>();
    }

    void Update()
    {
        if (player != null && target != null && arrowImage != null)
        {
            // Calculate the direction from the player to the target
            Vector3 directionToTarget = target.position - player.position;
            //Vector3 directionToTarget = player.position - target.position;

            // Convert the direction to an angle in degrees
            float angle = 90 + Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg; //+ 180;

            // Set the rotation of the arrow image to point towards the target
            arrowImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
