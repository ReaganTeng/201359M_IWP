using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [HideInInspector]
    public bool disabled;
    // Start is called before the first frame update








    public TextMeshProUGUI pressEText;








    void Start()
    {
        AdjustVolumes();
    }

    void AdjustVolumes()
    {
        float Volume = PlayerPrefs.GetFloat("MasterVolume"); // Adjust this value as needed

        // Find all AudioSource components in the scene
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        // Adjust the volume of each AudioSource
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = Volume;
        }
    }



    public void InteractableInteraction()
    {
        Interactables nearestInteractable = null;
        float nearestDistance = float.MaxValue;

        // Find all objects with the Interactable.cs script
        Interactables[] interactables = GameObject.FindObjectsOfType<Interactables>();

        foreach (Interactables interactable in interactables)
        {
            // Calculate the distance to the current interactable
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (interactable.gameObject.GetComponent<SpriteRenderer>() != null)
            {
                interactable.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }


            // Check if the distance is within the search radius and closer than the current nearest
            if (distance < 2.0f && distance < nearestDistance)
            {

                nearestInteractable = interactable;
                if (nearestInteractable.gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    nearestInteractable.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
                nearestDistance = distance;
            }
            //else
            //{
            //    pressEText.enabled = false;
            //}
        }

        // Check if a nearest interactable was found
        if (nearestInteractable != null)
        {
            pressEText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                nearestInteractable.Interact();
            }
        }
        else
        {
            pressEText.enabled = false;
        }
        //else
        //{

        //}
        //else
        //{
        //    Debug.Log("No interactables found within the search radius.");
        //}


    }

    void Awake()
    {
        Time.timeScale = 1;
        disabled = false;
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
        if(disabled)
        {
            return;
        }
        GetComponent<Animator>().Play(currentAnimState);
        Movement();
        InteractableInteraction();
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


    //public void IncreaseHealth(int val)
    //{
    //    PlayerPrefs.SetInt("HealthUpgradePercentage", PlayerPrefs.GetInt("HealthUpgradePercentage") + val);
    //}
}
