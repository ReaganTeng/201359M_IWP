using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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


    // Start is called before the first frame update
    void Awake()
    {
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");
        ShopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        ATMPanel = GameObject.FindGameObjectWithTag("ATMPanel");
        DayPanel = GameObject.FindGameObjectWithTag("DayPanel");
        GamePlayPanel = GameObject.FindGameObjectWithTag("GamePlayPanel");
        SettingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");

        togglePanel(ShopPanel);
        togglePanel(ATMPanel);
        togglePanel(DayPanel);
        togglePanel(dialoguePanel);
        togglePanel(SettingsPanel);

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
            PlayerPrefs.SetInt("DaysLeft", 100);
        }
        PlayerPrefs.SetInt("DaysLeft", 100);


        if (!PlayerPrefs.HasKey("MoneyDonated"))
        {
            PlayerPrefs.SetFloat("MoneyDonated", 0);
        }
        if (!PlayerPrefs.HasKey("RoundsCompleted"))
        {
            PlayerPrefs.SetInt("RoundsCompleted", 0);
        }

        goalamount = 100000;
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


    public void ResetPrefs()
    {
        if (PlayerPrefs.HasKey("GrossMoney"))
        {
            PlayerPrefs.SetFloat("GrossMoney", 0);
        }
        if (PlayerPrefs.HasKey("DaysLeft"))
        {
            PlayerPrefs.SetInt("DaysLeft", 100);
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
            togglePanel(SettingsPanel);
        }


        grossMoney.text = $"${PlayerPrefs.GetFloat("GrossMoney")}";
        daysLeft.text = $"{daysLeft}";
        float moneydonated = PlayerPrefs.GetFloat("MoneyDonated");
        ATMText.text = $"DONATED: {moneydonated}\nGOAL:{goalamount}";
        daysLeft.text = $"DAYS LEFT\n{PlayerPrefs.GetInt("DaysLeft")}";



        //DISABLE/ENABLE GAMEPLAY PANEL DEPENDING IF OTHER PANELS ARE ACTIVE
        if (!dialoguePanel.GetComponent<CanvasGroup>().interactable
        && !ShopPanel.GetComponent<CanvasGroup>().interactable
        && !ATMPanel.GetComponent<CanvasGroup>().interactable
        && !DayPanel.GetComponent<CanvasGroup>().interactable
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
            GameOverPanel.SetActive(false);
            WinPanel.SetActive(false);
        }

        if (PlayerPrefs.GetFloat("MoneyDonated") < goalamount
            && PlayerPrefs.GetInt("DaysLeft") <= 0)
        {
            GameOverPanel.SetActive(true);
        }

        if (PlayerPrefs.GetFloat("MoneyDonated") >= goalamount
            && PlayerPrefs.GetInt("DaysLeft") <= 0)
        {
            WinPanel.SetActive(true);
        }
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

    //public void TogglePanel()
    //{
    //    StoryPanel.SetActive(!StoryPanel.activeSelf);
    //}


    public void Endday()
    {
        PlayerPrefs.SetInt("DaysLeft", 0);
    }
}
