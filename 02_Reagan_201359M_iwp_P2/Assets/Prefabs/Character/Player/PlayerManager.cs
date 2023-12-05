using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerManager : MonoBehaviour
{
   //GAMEOBJECTS TO DRAG IN INSPECTOR
    public List<GameObject> players; // List of player characters
    int currentPlayerIndex = 0;
    int numberOfPlayers;
    public Camera mainCamera; // Reference to the main camera

    public GameObject characterSwitchpanel;

    bool stuckmode;


    public Slider healthbar;

    [HideInInspector]
    public bool finishedSpawning;
    public float cameraFollowSpeed; // Adjust the camera follow speed as needed

    //TEMPORARY
    private void Awake()
    {
        finishedSpawning = false;
        //StartItself();
    }


    public void StartItself()
    {


        stuckmode = false;
        numberOfPlayers = players.Count;
        SetPlayerToPlayerMode(currentPlayerIndex);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            players[i].GetComponent<Player>().leadingPlayer = players[currentPlayerIndex];
        }

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i != currentPlayerIndex)
            {
                //SET PLAYERS IN AI MODE
                SetPlayerToAIMode(i);
            }
        }



        healthbar = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Slider>();
        healthbar.minValue = 0;
        healthbar.maxValue = players[currentPlayerIndex].GetComponent<Player>().health;
        healthbar.value = players[currentPlayerIndex].GetComponent<Player>().health;

        finishedSpawning = true;


        SwitchPlayer();

        Debug.Log("START ITSELF FINISH");
    }

    private void Update()
    {
        healthbar.value = players[currentPlayerIndex].GetComponent<Player>().health;


        

        if (characterSwitchpanel.activeSelf == true)
        {
            //SLOW DOWN TIME
            Time.timeScale = .1f;
        }

        if (players.Count >= 0)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                //characterSwitchpanel.SetActive(true);
                SwitchPlayer();
            }
            cameraFollow();
        }
    }


    void cameraFollow()
    {
        // Keep the camera following the currently selected player
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            // Get the current player's position
            Vector3 playerPosition = players[currentPlayerIndex].transform.position;
            // Set the camera's position to follow the player horizontally and vertically
            float newX = playerPosition.x;
            float newY = playerPosition.y;
            // You can add an offset to control how much of the player is shown at the center of the camera
            float offsetX = 0f; // Adjust this value as needed
            float offsetY = 0f; // Adjust this value as needed
            // Calculate the target position for the camera
            Vector3 targetPosition = new Vector3(newX + offsetX, newY + offsetY, mainCamera.transform.position.z);
            Vector2 playerpos = new Vector2(newX + offsetX, newY + offsetY);
            Vector2 camerapos = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y);
            // Set a tolerance value for position equality check
            float positionTolerance = 0.01f; // You can adjust this tolerance as needed
            // Calculate the distance between player and camera positions
            float distance = Vector2.Distance(playerpos, camerapos);

            if (distance <= positionTolerance)
            {
                stuckmode = true;
            }

            // Move the camera smoothly to the target position
            if (!stuckmode)
            {
                //Time.timeScale = 1.0f;
                //Debug.Log("STUCKMODE FALSE");
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraFollowSpeed);
            }
            else
            {
                //Time.timeScale = 0.0f;
                //Debug.Log("STUCKMODE TRUE");
                mainCamera.transform.position = targetPosition;
            }
        }
    }

    public void SwitchPlayer()
    {
        int nextPlayerIndex = (currentPlayerIndex + 1) % numberOfPlayers;
        stuckmode = false;
        currentPlayerIndex = nextPlayerIndex;
        Debug.Log($"Switching to Player {currentPlayerIndex + 1}");
        //SET PLAYERS IN PLAYER MODE
        SetPlayerToPlayerMode(currentPlayerIndex);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i != currentPlayerIndex)
            {
                //SET PLAYERS TO AI MODE
                SetPlayerToAIMode(i);
            }
            players[i].GetComponent<Player>().leadingPlayer = players[currentPlayerIndex];

        }

        characterSwitchpanel.SetActive(false);
    }

    void SetPlayerToPlayerMode(int index)
    {
        //Debug.Log("ACTIVATE ");
        players[index].GetComponent<Player>().AIMode = false;   
    }

    void SetPlayerToAIMode(int index)
    {
        //Debug.Log("DEACTIVATE ");
        players[index].GetComponent<Player>().AIMode = true;
    }
}

