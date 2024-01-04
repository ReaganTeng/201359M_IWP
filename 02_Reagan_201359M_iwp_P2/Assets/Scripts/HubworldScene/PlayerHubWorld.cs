using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHubWorld : MonoBehaviour
{
    public GameObject ATMPanel;
    public GameObject shopPanel;
    public GameObject dayPanel;


    [HideInInspector]
    public string IDLE;
    [HideInInspector]
    public string WALK_FRONT;
    [HideInInspector]
    public string WALK_BACK;
    [HideInInspector]
    public string WALK_LEFT;
    [HideInInspector]
    public string WALK_RIGHT;


    string currentAnimState;

    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1;

        currentAnimState = IDLE;
        IDLE = "player_idle";
        WALK_BACK = "player_WalkBack";
        WALK_FRONT = "player_WalkFront";
        WALK_LEFT = "player_WalkLeft";
        WALK_RIGHT = "player_WalkRight";


        //PlayerPrefs.SetInt("HealthUpgradePercentage", 0);

        if (!PlayerPrefs.HasKey("HealthUpgrade"))
        {
            PlayerPrefs.SetInt("HealthUpgradePercentage", 0);
        }
        
        //ATMPanel = GameObject.FindGameObjectWithTag("ATMPanel");
        //shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        //dayPanel = GameObject.FindGameObjectWithTag("DayPanel");
    }

    // Update is called once per frame
    void Update()
    {

        GetComponent<Animator>().Play(currentAnimState);
        //if (!ATMPanel.activeSelf
        //    && !shopPanel.GetComponent<CanvasGroup>().interactable
        //    && !dayPanel.activeSelf)
        //{
            Movement();
        //}
    }

    void Movement()
    {
        // Get input from the player
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        if (verticalInput < 0)
        {
            currentAnimState = WALK_FRONT;
        }
        else if (verticalInput > 0)
        {
            currentAnimState = WALK_BACK;
        }

        if (horizontalInput < 0)
        {
            currentAnimState = WALK_LEFT;
        }
        else if (horizontalInput > 0)
        {
            currentAnimState = WALK_RIGHT;
        }
        //}

        if(horizontalInput == 0 &&
            verticalInput == 0)
        {
            currentAnimState = IDLE;
        }

        // Calculate the movement vector
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f).normalized * 6.0f * Time.deltaTime;
        // Calculate the player's potential new position
        Vector3 newPosition = transform.position + movement;
        // Calculate the rotation angle based on input (adjust the rotation speed as needed)
        float rotationAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg;
        // Apply rotation to the player
        transform.rotation = Quaternion.Euler(0f, 0f, -rotationAngle);
        // Apply movement
        transform.position = newPosition;
    }


    public void IncreaseHealth(int val)
    {
        PlayerPrefs.SetInt("HealthUpgradePercentage", PlayerPrefs.GetInt("HealthUpgradePercentage") + val);
    }
}
