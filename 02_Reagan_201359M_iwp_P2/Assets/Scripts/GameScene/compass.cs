using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class compass : MonoBehaviour
{
    Image arrowImage; // The image representing the arrow on the compass


    [HideInInspector]
    public GameObject[] playerlist;
    [HideInInspector]
    public GameObject player; // The target the arrow should point to
    [HideInInspector]
    public GameObject endzoneTarget; // The target the arrow should point to
    [HideInInspector]
    public bool compassSet;

    public TextMeshProUGUI Distance;

    private void Awake()
    {

        compassSet = false;
        arrowImage = GetComponent<Image>();

    }


    public void StartItself()
    {
        if (!compassSet)
        {
            playerlist = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < playerlist.Length; i++)
            {
                if (!playerlist[i].GetComponent<Player>().AIMode)
                {
                    player = playerlist[i];
                    break;
                }
            }
            endzoneTarget = GameObject.FindGameObjectWithTag("EndZone");
            compassSet = true;
        }
    }

    void Update()
    {
        if (player != null && endzoneTarget != null && arrowImage != null)
        {
            for (int i = 0; i < playerlist.Length; i++)
            {
                if (!playerlist[i].GetComponent<Player>().AIMode)
                {
                    player = playerlist[i];
                    break;
                }
            }


            // Calculate the direction from the player to the target
            Vector3 directionToTarget = endzoneTarget.transform.position - player.transform.position;
            //Vector3 directionToTarget = player.position - target.position;

            // Convert the direction to an angle in degrees
            float angle = 90 + Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg; //+ 180;

            // Set the rotation of the arrow image to point towards the target
            arrowImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);


            //DISTANCE BETWEEN PLAYER AND ENDZONE
            float distance = Vector2.Distance(player.transform.position, endzoneTarget.transform.position);
            Distance.text = $"{(int)distance}M\nfrom exit";
        }
    }
}
