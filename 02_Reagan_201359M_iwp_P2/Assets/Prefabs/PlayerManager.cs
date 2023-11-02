using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
   
    public List<GameObject> players; // List of player characters
    private int currentPlayerIndex = 0;
    int numberOfPlayers;
    public float cameraFollowSpeed; // Adjust the camera follow speed as needed
    public Camera mainCamera; // Reference to the main camera


    bool stuckmode;

    private void Start()
    {
        stuckmode = false;
        numberOfPlayers = players.Count;
        ActivatePlayer(currentPlayerIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchPlayer();
        }

        cameraFollow();
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
                //Time.timeScale = 0f;
                //Debug.Log("STUCKMODE FALSE");
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraFollowSpeed);

               
            }
            else
            {
                //Time.timeScale = 1.0f;
                //Debug.Log("STUCKMODE TRUE");
                mainCamera.transform.position = targetPosition;
            }
        }
    }

    private void SwitchPlayer()
    {
        int nextPlayerIndex = (currentPlayerIndex + 1) % numberOfPlayers;

        DeactivatePlayer(currentPlayerIndex);
        ActivatePlayer(nextPlayerIndex);

        stuckmode = false;

        currentPlayerIndex = nextPlayerIndex;
    }

    private void ActivatePlayer(int index)
    {
        players[index].GetComponent<Player>().AIMode = false;
    }

    private void DeactivatePlayer(int index)
    {
        players[index].GetComponent<Player>().AIMode = true;
    }
}

