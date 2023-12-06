using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHubWorld : MonoBehaviour
{
    public GameObject ATMPanel;
    public GameObject shopPanel;
    public GameObject dayPanel;

    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1;

        //ATMPanel = GameObject.FindGameObjectWithTag("ATMPanel");
        //shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        //dayPanel = GameObject.FindGameObjectWithTag("DayPanel");
    }

    // Update is called once per frame
    void Update()
    {
        if (!ATMPanel.activeSelf
            && !shopPanel.activeSelf
            && !dayPanel.activeSelf)
        {
            Movement();
        }
    }


    


    private void Movement()
    {
        // Get input from the player
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // Calculate the movement vector
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f).normalized * 5.0f * Time.deltaTime;
        // Calculate the player's potential new position
        Vector3 newPosition = transform.position + movement;
        // Calculate the rotation angle based on input (adjust the rotation speed as needed)
        float rotationAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg;
        // Apply rotation to the player
        transform.rotation = Quaternion.Euler(0f, 0f, -rotationAngle);
        // Apply movement
        transform.position = newPosition;
    }
}
