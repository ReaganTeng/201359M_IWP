using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI grossMoney;
    public TextMeshProUGUI daysLeft;
    public TextMeshProUGUI ATMText;
    public TMP_InputField ATMInputField;

    float goalamount;


    public GameObject GameOverPanel;
    public GameObject WinPanel;


    // Start is called before the first frame update
    void Start()
    {
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
        grossMoney.text = $"${PlayerPrefs.GetFloat("GrossMoney")}";
        daysLeft.text = $"{daysLeft}";

        float moneydonated = PlayerPrefs.GetFloat("MoneyDonated");
        ATMText.text = $"{moneydonated}/{goalamount}";


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
            Debug.Log("MONEY DONATED");
        }
    }



    public void Endday()
    {
        PlayerPrefs.SetInt("DaysLeft", 0);
    }
}
