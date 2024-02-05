
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor.Animations;


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
    //public List<Sprite> playerSprites = new List<Sprite>();


    //GAMEOBJECTS TO DRAG IN INSPECTOR

    [HideInInspector]
    public List<GameObject> players; // List of player characters
    int currentPlayerIndex = 0;
    int numberOfPlayers;
    public Camera mainCamera; // Reference to the main camera

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

    [HideInInspector]
    public Image playerIcon;

    [HideInInspector]
    public MenuManager MM;
    [HideInInspector]
    public GameObject characterSwitchPanel;


    public GameObject playerPrefab;

    float aiTimer;
    public GameObject playerSwitchButtonPrefab;  // Reference to the UI button prefab


    public GameObject PlayerParent;

    GameObject leadingPlayer;

    List<CharacterUnlockManager.CharacterType> selectedCharacters;
    //bool effectapplied = false;
    void Awake()
    {
        aiTimer = 0;

        MM = GetComponent<MenuManager>();

        playerIcon = GameObject.FindGameObjectWithTag("PlayerIcon").GetComponent<Image>();

        //dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");        
        finishedSpawning = false;
        //StartItself();
        gameOverPanel = GameObject.FindGameObjectWithTag("GameOverPanel");

        //if (gameOverPanel != null)
        //{
        //    gameOverPanel.SetActive(false);
        //}
        //Debug.Log("AWAKE");
    }


    void SwitchPlayerByButton(CharacterUnlockManager.CharacterType character)
    {
        // Find the index of the selected character
        int nextPlayerIndex = selectedCharacters.IndexOf(character);
        currentPlayerIndex = nextPlayerIndex;

        // Switch to the selected player
        SwitchPlayer(currentPlayerIndex);

        MM.togglePanel(characterSwitchPanel);
    }

    public void StartItself()
    {
        if (characterSwitchPanel == null)
        {
            characterSwitchPanel = MM.CharacterSwitchPanel;
        }

        //INSTANTIATE PLAYERS FIRST
        selectedCharacters = characterUnlockManager.selectedCharacters;
        //Debug.Log($"NUMBER OF CHARACTERS {selectedCharacters.Count}");
        foreach (CharacterUnlockManager.CharacterType character in selectedCharacters)
        {
            // Instantiate characters in the game scene based on unlocked characters
            GameObject p = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
            p.transform.SetParent(PlayerParent.transform);

            Character charScript = p.GetComponent<Character>();
            Player playerScript = p.GetComponent<Player>();
            charScript.characterType = character;
            playerScript.characterType = character;


            RuntimeAnimatorController overrideController = playerScript.characterAnimations.Find(entry => 
            entry.characterType == playerScript.characterType).animcon;
            if (overrideController != null)
            {
                // Assign the new override controller to the animatorComponent
                playerScript.animatorComponent.runtimeAnimatorController = overrideController;
            }


            playerScript.icon 
                = playerScript.playericon[(int)character].spriteIcon;

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

            players.Add(p);
            Debug.Log("PLAYER ADDED");
        }

        //stuckmode = false;
        numberOfPlayers = players.Count;
        SetPlayerToPlayerMode(currentPlayerIndex);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            //SET THE LEADING PLAYER
            players[i].GetComponent<Player>().leadingPlayer = players[currentPlayerIndex];
        }

        //INSTANTIATE BUTTONS to SWITCH CHARACTERS
        for (int i = 0; i < numberOfPlayers; i++)
        {
            float angleStep = 360f / numberOfPlayers;
            float angle = i * angleStep;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * 90.0f;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * 90.0f;
            GameObject playerSwitchButton = Instantiate(playerSwitchButtonPrefab, characterSwitchPanel.GetComponent<RectTransform>());
            playerSwitchButton.GetComponent<RectTransform>().sizeDelta *= .1f;
            playerSwitchButton.GetComponentInChildren<Image>().sprite = players[i].GetComponent<Player>().icon;
            // Set the position of the button
            playerSwitchButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            int currentIndex = i;  // Capture the current index in the lambda
            playerSwitchButton.GetComponentInChildren<Button>().onClick.AddListener(() => SwitchPlayerByButton(selectedCharacters[currentIndex]));
        }
        //

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i != currentPlayerIndex)
            {
                //SET PLAYERS IN AI MODE
                SetPlayerToAIMode(i);
            }
        }

        //SET UP PLAYER'S HEALTHBAR
        healthbar = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Slider>();
        healthbar.minValue = 0;
        healthbar.maxValue = players[currentPlayerIndex].GetComponent<Player>().health;
        healthbar.value = players[currentPlayerIndex].GetComponent<Player>().health;

        finishedSpawning = true;

        SwitchPlayer(0);

        //Debug.Log("START ITSELF FINISH");
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

        if(aiTimer > 2.0)
        {

           GameObject[] AIPlayers = players
            .Where(template => template.GetComponent<Player>() != null 
            && template.GetComponent<Player>().AIMode)
            .ToArray();
            int chosenPlayerIdx = Random.Range(0, AIPlayers.Length);

            //for (int i = 0; i < players.Count;i++)
            {

                //float distance = Vector3.Distance(
                //    leadingPlayer.transform.position,
                //    AIPlayers[chosenPlayerIdx].transform.position);

                if (//AIPlayers[chosenPlayerIdx].GetComponent<Player>().AIMode
                //&& 
                AIPlayers[chosenPlayerIdx].GetComponent<Player>().currentstate != Player.PlayerState.ATTACK
                //&& distance < 5
                )
                {

                    //if(AIPlayers[chosenPlayerIdx].GetComponent<Player>().nearestEnemy == null)
                    //{
                    //    //FIND NEAREST ENEMY
                    //    AIPlayers[chosenPlayerIdx].GetComponent<Player>().nearestEnemy
                    //        = AIPlayers[chosenPlayerIdx].GetComponent<Player>().
                    //        FindNearestEnemy(AIPlayers[chosenPlayerIdx].GetComponent<Player>().playerTransform.position);
                    //    //return;
                    //}

                    //if (AIPlayers[chosenPlayerIdx].GetComponent<Player>().nearestEnemy != null)
                    {
                        AIPlayers[chosenPlayerIdx].GetComponent<Player>().currentstate = Player.PlayerState.ATTACK;
                    }
                }
            }

            aiTimer = 0;
        }


        healthbar.value = players[currentPlayerIndex].GetComponent<Player>().health;

        bool allDead = players.All(p => p.GetComponent<Player>().health <= 0);
        if (allDead)
        {
            Inventory invmanager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<Inventory>();
            foreach (InventorySlot slot in invmanager.slots)
            {
                if (slot.itemtype != Item.ItemType.BOMB
                    && slot.itemtype != Item.ItemType.POTION
                    && slot.itemtype != Item.ItemType.BULLET)
                {
                    slot.itemtype = Item.ItemType.NOTHING;
                    slot.Quantity = 0;
                    slot.quantityText.text = "";
                    slot.CurrentItem = null;
                    slot.slotImage.sprite = null;
                }
            }
            invmanager.ChangesInInventory();

            MM.togglePanel(gameOverPanel);
        }

        //if (gameOverPanel.activeSelf)
        //{
        //    Time.timeScale = 0;
        //}
        //if(playerGameObject)

        //if (characterSwitchpanel.activeSelf == true)
        //{
        //    //SLOW DOWN TIME
        //    Time.timeScale = .1f;
        //}

        if (players.Count >= 0)
        {
            if (!allDead
                && players[currentPlayerIndex].GetComponent<Player>().health <= 0
                )
            {
                //characterSwitchpanel.SetActive(true);

                SwitchPlayerWhenDied();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                MM.togglePanel(characterSwitchPanel);
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

    public void SwitchPlayerWhenDied()
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
            playerIcon.sprite = players[currentPlayerIndex].GetComponent<Player>().icon;
            //characterSwitchpanel.SetActive(false);
        }
        
    }


    public void SwitchPlayer(int playerclicked)
    {
        int nextPlayerIndex = playerclicked;
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
            playerIcon.sprite = players[currentPlayerIndex].GetComponent<Player>().icon;

            leadingPlayer = players[currentPlayerIndex];
            //characterSwitchpanel.SetActive(false);
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

