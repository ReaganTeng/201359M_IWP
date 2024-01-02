using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;


public class PlayerManager : MonoBehaviour
{

    public CharacterUnlockManager characterUnlockManager;


    //public enum CharacterType
    //{
    //    JOE,
    //    PROFESSOR,
    //    VETERAN,
    //}
    //MAKE SURE THE SAME ORDER AS THE COMMENTED ENUM ABOVE, EXAMPLE SPRITE OF JOE THE SAME INDEX AS JOE
    public List<Sprite> playerSprites = new List<Sprite>();


    //GAMEOBJECTS TO DRAG IN INSPECTOR

    [HideInInspector]
    public List<GameObject> players; // List of player characters
    int currentPlayerIndex = 0;
    int numberOfPlayers;
    public Camera mainCamera; // Reference to the main camera

    public GameObject characterSwitchpanel;

    //public AnimatorController animcon;



    bool stuckmode;

    [HideInInspector]
    public Slider healthbar;

    [HideInInspector]
    public bool finishedSpawning;
    public float cameraFollowSpeed; // Adjust the camera follow speed as needed

    //TEMPORARY
    [HideInInspector]
    public GameObject gameOverPanel;

    //[HideInInspector]
    //public GameObject dialoguePanel;


    public GameObject playerPrefab;


    float aiTimer;


    //bool effectapplied = false;
    void Awake()
    {
        aiTimer = 0;

        //dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");        
        finishedSpawning = false;
        //StartItself();
        gameOverPanel = GameObject.FindGameObjectWithTag("GameOverPanel");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        //Debug.Log("AWAKE");
    }

    public void StartItself()
    {
        //INSTANTIATE PLAYERS FIRST
        List<CharacterUnlockManager.CharacterType> selectedCharacters = characterUnlockManager.selectedCharacters;
        foreach (CharacterUnlockManager.CharacterType character in selectedCharacters)
        {
            // Instantiate characters in the game scene based on unlocked characters
            GameObject p = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
            Character charScript = p.GetComponent<Character>();
            Player playerScript = p.GetComponent<Player>();

            charScript.characterType = character;
            playerScript.characterType = character;


            //GET THE CLIP YOU WANT TO PLAY
            int idx = 0;
            CharacterAnimationClips characterAnimationEntry =
            playerScript.characterAnimations.Find(entry => entry.characterType == playerScript.characterType);
            AnimationClip clipToPlay = characterAnimationEntry.animationClips[idx];
            //
            playerScript.AssignClipToState(playerScript.IDLE, clipToPlay);
            idx++;
            clipToPlay = characterAnimationEntry.animationClips[idx];
            playerScript.AssignClipToState(playerScript.WALK_FRONT, clipToPlay);
            idx++;
            clipToPlay = characterAnimationEntry.animationClips[idx];
            playerScript.AssignClipToState(playerScript.WALK_BACK, clipToPlay);
            idx++;
            clipToPlay = characterAnimationEntry.animationClips[idx];
            playerScript.AssignClipToState(playerScript.WALK_LEFT, clipToPlay);
            idx++;
            clipToPlay = characterAnimationEntry.animationClips[idx];
            playerScript.AssignClipToState(playerScript.WALK_RIGHT, clipToPlay);
            idx++;
            clipToPlay = characterAnimationEntry.animationClips[idx];
            playerScript.AssignClipToState(playerScript.ATTACK, clipToPlay);
            idx++;
            clipToPlay = characterAnimationEntry.animationClips[idx];
            playerScript.AssignClipToState(playerScript.HURT, clipToPlay);



            //Professor - weak in basic attacks but can deal higher damage with Gems
            //Veteran - weak in gem attacks but can absorb more damage and damage dealt

            // Get an array of all enum values
            //CharacterUnlockManager.CharacterType[] enumValues 
            //    = (CharacterUnlockManager.CharacterType[])Enum.GetValues(typeof(CharacterUnlockManager.CharacterType));
            // Get the index using Array.IndexOf
            //int index = Array.IndexOf(enumValues, character);
            //playerScript.spriteRenderer.sprite = playerSprites[index];

            //METHOD 1
            // Instantiate a new instance of the AnimatorController (RuntimeAnimatorController)
            AnimatorController newController
                = playerScript.characterAnimations.Find(entry 
                => entry.characterType == playerScript.characterType).animcon;
            if (newController != null)
            {
                // Assign the new controller to the animatorComponent
                playerScript.animatorComponent.runtimeAnimatorController = newController;
            }


            //METHOD 2 - INSTANTIATE ANIMCONTROLLERS
            //int index = selectedCharacters.IndexOf(character);
            //Debug.Log($"CHARACTER CHOSEN {character}");
            //AnimatorController newController
            //= new AnimatorController();
            //AnimatorController playercon = newController = playerScript.characterAnimations.Find(entry
            //=> entry.characterType == playerScript.characterType).animcon;
            //// Iterate through states in the source controller
            //foreach (ChildAnimatorState state in playercon.layers[0].stateMachine.states)
            //{
            //    // Create a new state in the destination controller
            //    ChildAnimatorState newState = new ChildAnimatorState
            //    {
            //        state = new AnimatorState { name = state.state.name }
            //    };
            //    newController.layers[0].stateMachine.AddState(state.state.name);
            //}
            //newController.name = $"PLAYER{index}";
            //if (newController != null)
            //{
            //    // Assign the new controller to the animatorComponent
            //    playerScript.animatorComponent.runtimeAnimatorController = newController;
            //}
            //




            switch (character)
            {
                //case CharacterUnlockManager.CharacterType.JOE:
                //    {
                //        break;
                //    }
                case CharacterUnlockManager.CharacterType.PROFESSOR:
                    {
                        playerScript.projectileDamage = playerScript.projectileDamage * 2;
                        playerScript.meleedamage = playerScript.meleedamage / 2;
                        break;
                    }
                case CharacterUnlockManager.CharacterType.VETERAN:
                    {
                        playerScript.projectileDamage = playerScript.projectileDamage/ 2;
                        playerScript.meleedamage = playerScript.meleedamage * 2;
                        break;
                    }
                default:
                    break;
            }

            //Character script = p.GetComponent<Character>();
            //if (playerScript.animatorComponent.runtimeAnimatorController == null)
            //{
            //    //AnimatorController newController
            //    //    = Instantiate(script.characterAnimations.Find(entry
            //    //    => entry.characterType == script.characterType).animcon)
            //    AnimatorController newController
            //    = Instantiate(animcon);
            //    newController.name = $"PLAYER{selectedCharacters.IndexOf(character)}";
            //    if (newController != null)
            //    {
            //        // Assign the new controller to the animatorComponent
            //        playerScript.animatorComponent.runtimeAnimatorController = newController;
            //    }
            //    //return;
            //}


            players.Add(p);
            Debug.Log("PLAYER ADDED");
        }

        //stuckmode = false;
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

        //dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!finishedSpawning
             ||
             players == null
             || 
             (players != null &&
             players[0].GetComponent<Character>().disabled)
             )
        {
            return;
        }

        aiTimer += 1 * Time.deltaTime;

        if(aiTimer > 5.0)
        { 
            for(int i = 0; i < players.Count;i++)
            {
                if (players[i].GetComponent<Player>().AIMode
                    && players[i].GetComponent<Player>().currentstate != Player.PlayerState.ATTACK)
                {
                    players[i].GetComponent<Player>().currentstate = Player.PlayerState.ATTACK;
                }
            }

            aiTimer = 0;
        }


        healthbar.value = players[currentPlayerIndex].GetComponent<Player>().health;

        bool allDead = players.All(p => p.GetComponent<Player>().health <= 0);
        if (allDead)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverPanel.activeSelf)
        {
            Time.timeScale = 0;
        }
        //if(playerGameObject)

        if (characterSwitchpanel.activeSelf == true)
        {
            //SLOW DOWN TIME
            Time.timeScale = .1f;
        }

        if (players.Count >= 0)
        {
            if (Input.GetKeyDown(KeyCode.Tab)
                || (!allDead
                && players[currentPlayerIndex].GetComponent<Player>().health <= 0)
                )
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

        if (players[nextPlayerIndex].GetComponent<Character>().health > 0)
        {
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

