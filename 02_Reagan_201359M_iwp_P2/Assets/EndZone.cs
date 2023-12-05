using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndZone : MonoBehaviour
{

    GameObject player;
    public GameObject GameCompletePanel;

    Inventory invmanager;

    float countdown;
    bool countdown_started;
    public float money;
    private void Awake()
    {
        money = 0;
        countdown = 0;
        countdown_started = false;
        player = GameObject.FindWithTag("Player");
        GameCompletePanel = GameObject.FindWithTag("GameCompletePanel");
        GameCompletePanel.SetActive(false);

         invmanager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<Inventory>();

    }


    public void Update()
    {
        //if (countdown_started)
        //{
        //    countdown += 1 * Time.deltaTime;

        //    if(countdown >= 2.0f)
        //    {
        //        Debug.Log("QUIT");
        //        //Application.Quit();
        //    }
        //}
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")
        && !collision.gameObject.GetComponent<Player>().AIMode
        && !GameCompletePanel.activeSelf)
        {
            PlayerPrefs.SetInt("RoundsCompleted",
           PlayerPrefs.GetInt("RoundsCompleted") + 1);
            int roundsCompleted = PlayerPrefs.GetInt("RoundsCompleted");

            //REDUCE DAYS IF CERTAIN AMOUNT OF ROUNDS COMPLETED
            if (roundsCompleted >= 2)
            {
                PlayerPrefs.SetInt("RoundsCompleted", 0);
                PlayerPrefs.SetInt("DaysLeft",
                PlayerPrefs.GetInt("DaysLeft") - 1);
            }

            //COUNT DOWN THE MONEY
            //foreach(var slot in invmanager.slots)
            //{
            //    for(int i = 0; i < slot.Quantity; i++)
            //    {
            //money += (int)slot.CurrentItem.money;
            money = PlayerPrefs.GetFloat("MoneyEarned");
            PlayerPrefs.SetFloat("GrossMoney", PlayerPrefs.GetFloat("GrossMoney") + money);
            GameCompletePanel.GetComponentInChildren<TextMeshProUGUI>().text = $"GAME OVER, YOU EARNED {money}\nDAYS LEFT:\n{PlayerPrefs.GetInt("DaysLeft")}";
            PlayerPrefs.SetInt("MoneyEarned", 0);

            //    }
            //}

            GameCompletePanel.SetActive(true);
            //countdown_started = true;


            Time.timeScale = 0;



            

        }
    }
}
