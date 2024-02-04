using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Item;
using UnityEngine.SceneManagement;
using System.Linq;

public class HubWorldMenuManager : MonoBehaviour
{
    public TextMeshProUGUI grossMoney;
    public TextMeshProUGUI daysLeft;
    public TextMeshProUGUI ATMText;
    public TMP_InputField ATMInputField;

    float goalamount;


    public GameObject GameOverPanel;
    public GameObject WinPanel;

    [HideInInspector]
    public GameObject HelpPanel;
    [HideInInspector]
    public GameObject StoryPanel;
    [HideInInspector]
    public GameObject dialoguePanel;


    [HideInInspector]
    public GameObject ShopPanel;
    [HideInInspector]
    public GameObject ATMPanel;
    [HideInInspector]
    public GameObject DayPanel;
    [HideInInspector]
    public GameObject GamePlayPanel;
    [HideInInspector]
    public GameObject SettingsPanel;


    [HideInInspector]
    public GameObject PausePanel;
    //WHEN THESE PANELS ARE UP, FREEZE THE GAME
    [HideInInspector]
    public List<GameObject> panelsToFreezegame = new List<GameObject>();


    public GameObject shopkeeperPrefab;

    public Upgrades upgrades;
    public CharacterUnlockManager characterUnlockManager;


    public DialogueRealTime winDialogue;
    public DialogueRealTime loseDialogue;
    DialogueManager dialogueManager;




    // Start is called before the first frame update
    void Awake()
    {
        //PlayerPrefs.DeleteAll();

        GamePlayPanel = GameObject.FindGameObjectWithTag("GamePlayPanel");

        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");
        ShopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        ATMPanel = GameObject.FindGameObjectWithTag("ATMPanel");
        DayPanel = GameObject.FindGameObjectWithTag("DayPanel");
        SettingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");
        PausePanel = GameObject.FindGameObjectWithTag("PausePanel");

        dialogueManager = GetComponent<DialogueManager>();


        toggleAndPanelToList(ShopPanel);
        toggleAndPanelToList(ATMPanel);
        toggleAndPanelToList(DayPanel);
        toggleAndPanelToList(dialoguePanel);
        toggleAndPanelToList(SettingsPanel);
        toggleAndPanelToList(PausePanel);



        HelpPanel = GameObject.FindGameObjectWithTag("HelpPanel");
        if (HelpPanel != null)
        {
            HelpPanel.SetActive(true);
        }

        //PlayerPrefs.SetFloat("GrossMoney", 0);

        StoryPanel = GameObject.FindGameObjectWithTag("StoryScreen");

        if (!PlayerPrefs.HasKey("GrossMoney"))
        {
            PlayerPrefs.SetFloat("GrossMoney", 0);
        }
        if (!PlayerPrefs.HasKey("MoneyEarned"))
        {
            PlayerPrefs.SetFloat("MoneyEarned", 0);
        }
        if (!PlayerPrefs.HasKey("DaysLeft"))
        {
            PlayerPrefs.SetInt("DaysLeft", 7);
        }
        //PlayerPrefs.SetInt("DaysLeft", 100);


        if (!PlayerPrefs.HasKey("MoneyDonated"))
        {
            PlayerPrefs.SetFloat("MoneyDonated", 0);
        }
        if (!PlayerPrefs.HasKey("RoundsCompleted"))
        {
            PlayerPrefs.SetInt("RoundsCompleted", 0);
        }


        Instantiate(shopkeeperPrefab, new Vector3(-13.0599995f, -0.409999847f, 0), Quaternion.identity);

        goalamount = 100000;
    }



    public void toggleAndPanelToList(GameObject panel)
    {
        Debug.Log("TOGGLING PANEL");
        CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
        panelCG.interactable = !panelCG.interactable;
        panelCG.blocksRaycasts = !panelCG.blocksRaycasts;
        if (panelCG.interactable
           && panelCG.blocksRaycasts)
        {
            panelCG.alpha = 1;
        }
        else
        {
            panelCG.alpha = 0;
        }

        panelsToFreezegame.Add(panel);

    }

    public void togglePanel_CloseButton(GameObject closeButtonGO)
    {
        CanvasGroup panelCG = closeButtonGO.GetComponentInParent<CanvasGroup>();
        panelCG.interactable = !panelCG.interactable;
        panelCG.blocksRaycasts = !panelCG.blocksRaycasts;
        if (panelCG.interactable
           && panelCG.blocksRaycasts)
        {
            panelCG.alpha = 1;
        }
        else
        {
            panelCG.alpha = 0;
        }

    }


    public void togglePanel(GameObject panel)
    {
        //Debug.Log("TOGGLING PANEL");
        CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
        panelCG.interactable = !panelCG.interactable;
        panelCG.blocksRaycasts = !panelCG.blocksRaycasts;
        if (panelCG.interactable
           && panelCG.blocksRaycasts)
        {
            panelCG.alpha = 1;
        }
        else
        {
            panelCG.alpha = 0;
        }
    }


    public void ResetGame()
    {
        if (PlayerPrefs.HasKey("GrossMoney"))
        {
            PlayerPrefs.SetFloat("GrossMoney", 0);
        }
        if (PlayerPrefs.HasKey("DaysLeft"))
        {
            PlayerPrefs.SetInt("DaysLeft", 7);
        }
        if (PlayerPrefs.HasKey("MoneyDonated"))
        {
            PlayerPrefs.SetFloat("MoneyDonated", 0);
        }
        if (PlayerPrefs.HasKey("MoneyEarned"))
        {
            PlayerPrefs.SetFloat("MoneyEarned", 0);
        }
        if (PlayerPrefs.HasKey("RoundsCompleted"))
        {
            PlayerPrefs.SetInt("RoundsCompleted", 0);
        }


        WinPanel.SetActive(false);
        GameOverPanel.SetActive(false);

        //FindObjectOfType<Inventory>().EmptyInventory();
        upgrades.Reset();



    }

    // Update is called once per frame
    void Update()
    {
        //if(HelpPanel != null &&
        //    HelpPanel.activeSelf)
        //{
        //    Time.timeScale = 0;
        //}
        //else
        //{
        //    Time.timeScale = 1.0f;
        //}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            togglePanel(PausePanel);
        }


        grossMoney.text = $"${PlayerPrefs.GetFloat("GrossMoney")}";
        daysLeft.text = $"{daysLeft}";
        float moneydonated = PlayerPrefs.GetFloat("MoneyDonated");
        ATMText.text = $"DONATED: {moneydonated}\nGOAL:{goalamount}";
        daysLeft.text = $"DAYS LEFT\n{PlayerPrefs.GetInt("DaysLeft")}";



        //DISABLE/ENABLE GAMEPLAY PANEL DEPENDING IF OTHER PANELS ARE ACTIVE
        if (panelsToFreezegame.All(
            template => !template.GetComponent<CanvasGroup>().interactable)
        && !GameOverPanel.activeSelf
        && !WinPanel.activeSelf
        )
        {
            if (!GamePlayPanel.GetComponent<CanvasGroup>().interactable)
            {

                PlayerHubWorld[] charactersInScene = FindObjectsOfType<PlayerHubWorld>();
                foreach (PlayerHubWorld character in charactersInScene)
                {
                    character.disabled = false;
                }
                GamePlayPanel.GetComponent<CanvasGroup>().interactable = true;
                GamePlayPanel.GetComponent<CanvasGroup>().alpha = 1;
                GamePlayPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }

        }
        else
        {
            if (GamePlayPanel.GetComponent<CanvasGroup>().interactable)
            {

                PlayerHubWorld[] charactersInScene = FindObjectsOfType<PlayerHubWorld>();
                foreach (PlayerHubWorld character in charactersInScene)
                {
                    character.disabled = true;
                }

                GamePlayPanel.GetComponent<CanvasGroup>().interactable = false;
                GamePlayPanel.GetComponent<CanvasGroup>().alpha = 0;
                GamePlayPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;

                
            }
        }


        if (PlayerPrefs.GetInt("DaysLeft") > 0)
        {
            //GameOverPanel.SetActive(false);
            //WinPanel.SetActive(false);
           
        }

        //LOSING CONDITION
        if (PlayerPrefs.GetFloat("MoneyDonated") < goalamount
            && PlayerPrefs.GetInt("DaysLeft") <= 0
            && !GameOverPanel.activeSelf
            && !upgrades.WonGame)
        {

            dialogueManager.StartDialogue(loseDialogue);
        }

        //DISABLE IN FINAL BUILD
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.SetFloat("MoneyDonated",
                PlayerPrefs.GetFloat("MoneyDonated") + 100000);
            Debug.Log($"MONEY DONATED {PlayerPrefs.GetFloat("MoneyDonated")}");
        }

        //if (PlayerPrefs.GetFloat("MoneyDonated") >= goalamount)
        //{
        //    Debug.Log("YOU WON");
        //}

        //WINNING CONDITION
        if (PlayerPrefs.GetFloat("MoneyDonated") >= goalamount
            && PlayerPrefs.GetInt("DaysLeft") > 0
            && !WinPanel.activeSelf
            && !upgrades.WonGame)
        {
            Debug.Log("YOU WON");
            dialogueManager.StartDialogue(winDialogue);
            upgrades.WonGame = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (dialogueManager.currentDialogue == winDialogue
                && dialogueManager.currentSentenceidx == dialogueManager.currentDialogue.sentences.Count - 1)
            {
                upgrades.SaveUpgrades();
                WinPanel.SetActive(true);
            }
            if (dialogueManager.currentDialogue == loseDialogue
                && dialogueManager.currentSentenceidx == dialogueManager.currentDialogue.sentences.Count - 1)
            {
                GameOverPanel.SetActive(true);
            }
        }



    }


    public void QuitGame()
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
        SceneManager.LoadScene("MainMenu");
    }



    public void Deposit()
    {
        if (PlayerPrefs.HasKey("MoneyDonated")
            && ATMInputField.text != ""
            && float.Parse(ATMInputField.text) <= PlayerPrefs.GetFloat("GrossMoney"))
        {
            float deposit = float.Parse(ATMInputField.text);
            PlayerPrefs.SetFloat("MoneyDonated"
                , PlayerPrefs.GetFloat("MoneyDonated") + deposit
                );

            PlayerPrefs.SetFloat("GrossMoney"
                , PlayerPrefs.GetFloat("GrossMoney") - deposit
                );

            ATMInputField.text = "";

            Debug.Log("MONEY DONATED");
        }
    }


    //public void toggleHelpPanel()
    //{
    //    HelpPanel.SetActive(!HelpPanel.activeSelf);
    //}

    public void TogglePanelActive(GameObject panel)
    {
       panel.SetActive(!panel.activeSelf);
    }


    public void Endday()
    {
        PlayerPrefs.SetInt("DaysLeft", 0);
    }
}
