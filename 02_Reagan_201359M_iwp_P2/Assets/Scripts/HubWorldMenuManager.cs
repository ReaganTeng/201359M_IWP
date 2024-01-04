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
    public GameObject DialoguePanel;


    [HideInInspector]
    public GameObject ShopPanel;
    [HideInInspector]
    public GameObject ATMPanel;
    [HideInInspector]
    public GameObject DayPanel;

    // Start is called before the first frame update
    void Awake()
    {
        DialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");
        //DialoguePanel.SetActive(false);
        ShopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        ATMPanel = GameObject.FindGameObjectWithTag("ATMPanel");
        DayPanel = GameObject.FindGameObjectWithTag("DayPanel");
        togglePanel(ShopPanel);
        togglePanel(ATMPanel);
        togglePanel(DayPanel);
        togglePanel(DialoguePanel);

        HelpPanel = GameObject.FindGameObjectWithTag("HelpPanel");
        if (HelpPanel != null)
        {
            HelpPanel.SetActive(true);
        }

        StoryPanel = GameObject.FindGameObjectWithTag("StoryScreen");

        if (!PlayerPrefs.HasKey("GrossMoney"))
        {
            PlayerPrefs.SetFloat("GrossMoney", 100000);
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


    public void togglePanel(GameObject panel)
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
    }


    public void ResetPrefs()
    {
        if (PlayerPrefs.HasKey("GrossMoney"))
        {
            PlayerPrefs.SetFloat("GrossMoney", 100000);
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
        if(HelpPanel != null &&
            HelpPanel.activeSelf)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1.0f;
        }


        grossMoney.text = $"${PlayerPrefs.GetFloat("GrossMoney")}";
        daysLeft.text = $"{daysLeft}";

        float moneydonated = PlayerPrefs.GetFloat("MoneyDonated");
        ATMText.text = $"DONATED: {moneydonated}\nGOAL:{goalamount}";

        daysLeft.text = $"DAYS LEFT\n{PlayerPrefs.GetInt("DaysLeft")}";

        if(PlayerPrefs.GetInt("DaysLeft") > 0)
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


    public void toggleHelpPanel()
    {
        HelpPanel.SetActive(!HelpPanel.activeSelf);
    }

    public void TogglePanel()
    {
        StoryPanel.SetActive(!StoryPanel.activeSelf);
    }


    public void Endday()
    {
        PlayerPrefs.SetInt("DaysLeft", 0);
    }
}
